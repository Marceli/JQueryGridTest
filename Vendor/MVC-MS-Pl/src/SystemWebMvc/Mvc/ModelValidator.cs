namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public abstract class ModelValidator {
        protected ModelValidator(ModelMetadata metadata, ControllerContext controllerContext) {
            if (metadata == null) {
                throw new ArgumentNullException("metadata");
            }
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }

            Metadata = metadata;
            ControllerContext = controllerContext;
        }

        protected internal ControllerContext ControllerContext { get; private set; }

        protected internal ModelMetadata Metadata { get; private set; }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method may perform non-trivial work.")]
        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            return Enumerable.Empty<ModelClientValidationRule>();
        }

        public abstract IEnumerable<ModelValidationResult> Validate(object container);
    }
}
