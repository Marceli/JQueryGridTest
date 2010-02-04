namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelValidatorTest {

        [TestMethod]
        public void ConstructorGuards() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
            ControllerContext context = new ControllerContext();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => new TestableModelValidator(null, context),
                "metadata");
            ExceptionHelper.ExpectArgumentNullException(
                () => new TestableModelValidator(metadata, null),
                "controllerContext");
        }

        [TestMethod]
        public void ValuesSet() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();

            // Act
            TestableModelValidator validator = new TestableModelValidator(metadata, context);

            // Assert
            Assert.AreSame(context, validator.ControllerContext);
            Assert.AreSame(metadata, validator.Metadata);
        }

        [TestMethod]
        public void NoClientRulesByDefault() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();

            // Act
            TestableModelValidator validator = new TestableModelValidator(metadata, context);

            // Assert
            Assert.IsFalse(validator.GetClientValidationRules().Any());
        }

        private class TestableModelValidator : ModelValidator {
            public TestableModelValidator(ModelMetadata metadata, ControllerContext context) : base(metadata, context) {
            }

            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

    }
}
