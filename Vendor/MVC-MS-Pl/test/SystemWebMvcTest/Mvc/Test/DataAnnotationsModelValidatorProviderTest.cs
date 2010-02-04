namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DataAnnotationsModelValidatorProviderTest {

        // Adapter registration

        // TODO: Need a post-Preview 2 way to reset the adapter list for testing purposes

        class MyValidationAttribute : ValidationAttribute {
            public override bool IsValid(object value) {
                throw new NotImplementedException();
            }
        }

        class MyValidationAdapter : DataAnnotationsModelValidator<MyValidationAttribute> {
            public MyValidationAdapter(ModelMetadata metadata, ControllerContext context, MyValidationAttribute attribute)
                : base(metadata, context, attribute) { }
        }

        class MyValidationAdapterBadCtor : ModelValidator {
            public MyValidationAdapterBadCtor(ModelMetadata metadata, ControllerContext context)
                : base(metadata, context) { }

            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void GuardClauses() {
            // Validation attribute must derive from ValidationAttribute
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(object), typeof(MyValidationAdapter)),
                "The type System.Object must derive from System.ComponentModel.DataAnnotations.ValidationAttribute\r\nParameter name: attributeType");

            // Adapter must derive from ModelValidator
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MyValidationAttribute), typeof(object)),
                "The type System.Object must derive from System.Web.Mvc.ModelValidator\r\nParameter name: adapterType");

            // Adapter must have the expected constructor
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MyValidationAttribute), typeof(MyValidationAdapterBadCtor)),
                "The type System.Web.Mvc.Test.DataAnnotationsModelValidatorProviderTest+MyValidationAdapterBadCtor must have a public constructor which accepts three parameters of types System.Web.Mvc.ModelMetadata, System.Web.Mvc.ControllerContext, and System.Web.Mvc.Test.DataAnnotationsModelValidatorProviderTest+MyValidationAttribute\r\nParameter name: adapterType");
        }

        // Client validation rules

        [TestMethod]
        public void ClientRulesWithNoAttributes() {
            // Arrange
            DataAnnotationsModelValidatorProvider provider = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = GetMetadataForProperty("RegularStringProperty");

            // Act
            ModelClientValidationRule[] rules = provider.GetValidators(metadata, new ControllerContext())
                                                        .SelectMany(v => v.GetClientValidationRules())
                                                        .ToArray();

            // Assert
            Assert.AreEqual(0, rules.Length);
        }

        [TestMethod]
        public void ClientRulesWithRangeAttribute() {
            // Arrange
            DataAnnotationsModelValidatorProvider provider = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = GetMetadataForProperty("RangedDecimalProperty");

            // Act
            ModelClientValidationRule[] rules = provider.GetValidators(metadata, new ControllerContext())
                                                        .SelectMany(v => v.GetClientValidationRules())
                                                        .OrderBy(r => r.ValidationType)
                                                        .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("range", rules[0].ValidationType);
            Assert.AreEqual(2, rules[0].ValidationParameters.Count);
            Assert.AreEqual(0m, rules[0].ValidationParameters["minimum"]);
            Assert.AreEqual(100m, rules[0].ValidationParameters["maximum"]);
            Assert.AreEqual(@"The field RangedDecimalProperty must be between 0 and 100.", rules[0].ErrorMessage);

            // Temporarily disabled for DDB #226223
            //Assert.AreEqual("required", rules[1].ValidationType);
            //Assert.AreEqual(0, rules[1].ValidationParameters.Count);
        }

        [TestMethod]
        public void ClientRulesWithRegexAttribute() {
            // Arrange
            DataAnnotationsModelValidatorProvider provider = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = GetMetadataForProperty("StringPropertyWithRegexAttribute");

            // Act
            ModelClientValidationRule[] rules = provider.GetValidators(metadata, new ControllerContext())
                                                        .SelectMany(v => v.GetClientValidationRules())
                                                        .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("regularExpression", rules[0].ValidationType);
            Assert.AreEqual(1, rules[0].ValidationParameters.Count);
            Assert.AreEqual("the_pattern", rules[0].ValidationParameters["pattern"]);
            Assert.AreEqual(@"The field StringPropertyWithRegexAttribute must match the regular expression 'the_pattern'.", rules[0].ErrorMessage);
        }

        [TestMethod]
        public void ClientRulesWithRequiredAttribute() {
            // Arrange
            DataAnnotationsModelValidatorProvider provider = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = GetMetadataForProperty("RequiredStringProperty");

            // Act
            ModelClientValidationRule[] rules = provider.GetValidators(metadata, new ControllerContext())
                                                        .SelectMany(v => v.GetClientValidationRules())
                                                        .OrderBy(r => r.ValidationType)
                                                        .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);
            Assert.AreEqual("required", rules[0].ValidationType);
            Assert.AreEqual(0, rules[0].ValidationParameters.Count);
            Assert.AreEqual(@"The RequiredStringProperty field is required.", rules[0].ErrorMessage);
        }

        // Temporarily disabled for DDB #226223
        [TestMethod, Ignore]
        public void ClientRulesWithImplicitRequiredAttributeFromIsRequiredMetadata() {
            // Arrange
            DataAnnotationsModelValidatorProvider provider = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = GetMetadataForProperty("RegularIntProperty");

            // Act
            ModelClientValidationRule[] rules = provider.GetValidators(metadata, new ControllerContext())
                                                        .SelectMany(v => v.GetClientValidationRules())
                                                        .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);
            Assert.AreEqual("required", rules[0].ValidationType);
            Assert.AreEqual(0, rules[0].ValidationParameters.Count);
            Assert.AreEqual("The RegularIntProperty field is required.", rules[0].ErrorMessage);
        }

        [TestMethod]
        public void ClientRulesWithStringLengthAttribute() {
            // Arrange
            DataAnnotationsModelValidatorProvider provider = new DataAnnotationsModelValidatorProvider();
            ModelMetadata metadata = GetMetadataForProperty("StringPropertyWithStringLengthAttribute");

            // Act
            ModelClientValidationRule[] rules = provider.GetValidators(metadata, new ControllerContext())
                                                        .SelectMany(v => v.GetClientValidationRules())
                                                        .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("stringLength", rules[0].ValidationType);
            Assert.AreEqual(2, rules[0].ValidationParameters.Count);
            Assert.AreEqual(0, rules[0].ValidationParameters["minimumLength"]);
            Assert.AreEqual(4, rules[0].ValidationParameters["maximumLength"]);
            Assert.AreEqual(@"The field StringPropertyWithStringLengthAttribute must be a string with a maximum length of 4.", rules[0].ErrorMessage);
        }

        private static ModelMetadata GetMetadataForProperty(string propertyName) {
            return new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null /* model */, typeof(MyModel), propertyName);
        }

        private class MyModel {

            [Range(typeof(decimal), "0", "100")]
            public decimal RangedDecimalProperty { get; set; }

            public int RegularIntProperty { get; set; }

            public string RegularStringProperty { get; set; }

            [Required]
            public string RequiredStringProperty { get; set; }

            [RegularExpression("the_pattern")]
            public string StringPropertyWithRegexAttribute { get; set; }

            [StringLength(4)]
            public string StringPropertyWithStringLengthAttribute { get; set; }

        }

    }
}
