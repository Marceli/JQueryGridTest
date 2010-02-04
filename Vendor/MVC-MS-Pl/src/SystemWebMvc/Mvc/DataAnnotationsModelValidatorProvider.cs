namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc.Resources;

    public delegate ModelValidator DataAnnotationsModelValidationFactory(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute);

    public class DataAnnotationsModelValidatorProvider : AssociatedValidatorProvider {
        private static ReaderWriterLock _adaptersLock = new ReaderWriterLock();
        private static Dictionary<Type, DataAnnotationsModelValidationFactory> _adapters = new Dictionary<Type, DataAnnotationsModelValidationFactory>() {
            { typeof(RangeAttribute),             RangeValidator.Create             },
            { typeof(RegularExpressionAttribute), RegularExpressionValidator.Create },
            { typeof(RequiredAttribute),          RequiredValidator.Create          },
            { typeof(StringLengthAttribute),      StringLengthValidator.Create      },
        };
        private static DataAnnotationsModelValidationFactory _defaultFactory = DataAnnotationsModelValidator.Create;

        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes) {
            _adaptersLock.AcquireReaderLock(Timeout.Infinite);

            try {
                List<ModelValidator> results = new List<ModelValidator>();

                // Temporarily disabled for DDB #226223

                //if (metadata.IsRequired && !attributes.Any(a => a is RequiredAttribute)) {
                //    attributes = attributes.Concat(new[] { new RequiredAttribute() });
                //}

                foreach (ValidationAttribute attribute in attributes.OfType<ValidationAttribute>()) {
                    DataAnnotationsModelValidationFactory factory;
                    if (!_adapters.TryGetValue(attribute.GetType(), out factory)) {
                        factory = _defaultFactory;
                    }
                    results.Add(factory(metadata, context, attribute));
                }

                return results;
            }
            finally {
                _adaptersLock.ReleaseReaderLock();
            }
        }

        public static void RegisterAdapter(Type attributeType, Type adapterType) {
            ValidateAttributeType(attributeType);
            ValidateAdapterType(adapterType);
            ConstructorInfo constructor = GetAdapterConstructor(attributeType, adapterType);

            _adaptersLock.AcquireWriterLock(Timeout.Infinite);

            try {
                _adapters[attributeType] = (metadata, context, attribute) => (ModelValidator)constructor.Invoke(new object[] { metadata, context, attribute });
            }
            finally {
                _adaptersLock.ReleaseWriterLock();
            }
        }

        public static void RegisterAdapterFactory(Type attributeType, DataAnnotationsModelValidationFactory factory) {
            ValidateAttributeType(attributeType);
            ValidateFactory(factory);

            _adaptersLock.AcquireWriterLock(Timeout.Infinite);

            try {
                _adapters[attributeType] = factory;
            }
            finally {
                _adaptersLock.ReleaseWriterLock();
            }
        }

        public static void RegisterDefaultAdapter(Type adapterType) {
            ValidateAdapterType(adapterType);
            ConstructorInfo constructor = GetAdapterConstructor(typeof(ValidationAttribute), adapterType);

            _defaultFactory = (metadata, context, attribute) => (ModelValidator)constructor.Invoke(new object[] { metadata, context, attribute });
        }

        public static void RegisterDefaultAdapterFactory(DataAnnotationsModelValidationFactory factory) {
            ValidateFactory(factory);

            _defaultFactory = factory;
        }

        // Helpers

        private static ConstructorInfo GetAdapterConstructor(Type attributeType, Type adapterType) {
            ConstructorInfo constructor = adapterType.GetConstructor(new[] { typeof(ModelMetadata), typeof(ControllerContext), attributeType });
            if (constructor == null) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.DataAnnotationsModelValidatorProvider_ConstructorRequirements,
                        adapterType.FullName,
                        typeof(ModelMetadata).FullName,
                        typeof(ControllerContext).FullName,
                        attributeType.FullName
                    ),
                    "adapterType"
                );
            }

            return constructor;
        }

        private static void ValidateAdapterType(Type adapterType) {
            if (adapterType == null) {
                throw new ArgumentNullException("adapterType");
            }
            if (!typeof(ModelValidator).IsAssignableFrom(adapterType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.Common_TypeMustDriveFromType,
                        adapterType.FullName,
                        typeof(ModelValidator).FullName
                    ),
                    "adapterType"
                );
            }
        }

        private static void ValidateAttributeType(Type attributeType) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }
            if (!typeof(ValidationAttribute).IsAssignableFrom(attributeType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.Common_TypeMustDriveFromType,
                        attributeType.FullName,
                        typeof(ValidationAttribute).FullName
                    ),
                    "attributeType");
            }
        }

        private static void ValidateFactory(DataAnnotationsModelValidationFactory factory) {
            if (factory == null) {
                throw new ArgumentNullException("factory");
            }
        }

        // Default adapters

        private class RangeValidator : DataAnnotationsModelValidator<RangeAttribute> {
            public RangeValidator(ModelMetadata metadata, ControllerContext context, RangeAttribute attribute)
                : base(metadata, context, attribute) {
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
                return new[] { new ModelClientValidationRangeRule(ErrorMessage, Attribute.Minimum, Attribute.Maximum) };
            }

            internal static new ModelValidator Create(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute) {
                return new RangeValidator(metadata, context, (RangeAttribute)attribute);
            }
        }

        private class RegularExpressionValidator : DataAnnotationsModelValidator<RegularExpressionAttribute> {
            public RegularExpressionValidator(ModelMetadata metadata, ControllerContext context, RegularExpressionAttribute attribute)
                : base(metadata, context, attribute) {
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
                return new[] { new ModelClientValidationRegexRule(ErrorMessage, Attribute.Pattern) };
            }

            internal static new ModelValidator Create(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute) {
                return new RegularExpressionValidator(metadata, context, (RegularExpressionAttribute)attribute);
            }
        }

        private class RequiredValidator : DataAnnotationsModelValidator<RequiredAttribute> {
            public RequiredValidator(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
                : base(metadata, context, attribute) {
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
                return new[] { new ModelClientValidationRequiredRule(ErrorMessage) };
            }

            internal static new ModelValidator Create(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute) {
                return new RequiredValidator(metadata, context, (RequiredAttribute)attribute);
            }
        }

        private class StringLengthValidator : DataAnnotationsModelValidator<StringLengthAttribute> {
            public StringLengthValidator(ModelMetadata metadata, ControllerContext context, StringLengthAttribute attribute)
                : base(metadata, context, attribute) {
            }

            public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
                return new[] { new ModelClientValidationStringLengthRule(ErrorMessage, 0, Attribute.MaximumLength) };
            }

            internal static new ModelValidator Create(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute) {
                return new StringLengthValidator(metadata, context, (StringLengthAttribute)attribute);
            }
        }
    }
}
