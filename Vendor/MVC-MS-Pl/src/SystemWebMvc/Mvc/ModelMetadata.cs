namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web.Mvc.ExpressionUtil;
    using System.Web.Mvc.Resources;

    public class ModelMetadata {
        // Explicit backing store for the things we want true by default, so don't have to call
        // the protected virtual setters of an auto-generated property
        private readonly Type _containerType;
        private bool _convertEmptyStringToNull = true;
        private bool _isRequired;
        private object _model;
        private Func<object> _modelAccessor;
        private readonly Type _modelType;
        private IEnumerable<ModelMetadata> _properties;
        private readonly string _propertyName;
        private Type _realModelType;
        private bool _showForDisplay = true;
        private bool _showForEdit = true;
        private string _simpleDisplayText;

        public ModelMetadata(ModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName) {
            if (provider == null) {
                throw new ArgumentNullException("provider");
            }
            if (modelType == null) {
                throw new ArgumentNullException("modelType");
            }

            Provider = provider;

            _containerType = containerType;
            _isRequired = !TypeHelpers.TypeAllowsNullValue(modelType);
            _modelAccessor = modelAccessor;
            _modelType = modelType;
            _propertyName = propertyName;
        }

        public Type ContainerType {
            get {
                return _containerType;
            }
        }

        public virtual bool ConvertEmptyStringToNull {
            get {
                return _convertEmptyStringToNull;
            }
            set {
                _convertEmptyStringToNull = value;
            }
        }

        public virtual string DataType {
            get;
            set;
        }

        public virtual string Description {
            get;
            set;
        }

        public virtual string DisplayFormatString {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The method is a delegating helper to choose among multiple property values")]
        public virtual string DisplayName {
            get;
            set;
        }

        public virtual string EditFormatString {
            get;
            set;
        }

        public virtual bool HideSurroundingChrome {
            get;
            set;
        }

        public virtual bool IsComplexType {
            get {
                return !(TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string)));
            }
        }

        public bool IsNullableValueType {
            get {
                return TypeHelpers.IsNullableValueType(ModelType);
            }
        }

        public virtual bool IsReadOnly {
            get;
            set;
        }

        public virtual bool IsRequired {
            get {
                return _isRequired;
            }
            set {
                _isRequired = value;
            }
        }

        public object Model {
            get {
                if (_modelAccessor != null) {
                    _model = _modelAccessor();
                    _modelAccessor = null;
                }
                return _model;
            }
            set {
                _model = value;
                _modelAccessor = null;
            }
        }

        public Type ModelType {
            get {
                return _modelType;
            }
        }

        public virtual string NullDisplayText { get; set; }

        public virtual IEnumerable<ModelMetadata> Properties {
            get {
                if (_properties == null) {
                    _properties = Provider.GetMetadataForProperties(Model, ModelType);
                }
                return _properties;
            }
        }

        public string PropertyName {
            get {
                return _propertyName;
            }
        }

        protected ModelMetadataProvider Provider {
            get;
            set;
        }

        internal Type RealModelType {
            get {
                if (_realModelType == null) {
                    _realModelType = ModelType;

                    // Don't call GetType() if the model is Nullable<T>, because it will
                    // turn Nullable<T> into T for non-null values
                    if (Model != null && !TypeHelpers.IsNullableValueType(ModelType)) {
                        _realModelType = Model.GetType();
                    }
                }

                return _realModelType;
            }
        }

        public virtual string ShortDisplayName {
            get;
            set;
        }

        public virtual bool ShowForDisplay {
            get {
                return _showForDisplay;
            }
            set {
                _showForDisplay = value;
            }
        }

        public virtual bool ShowForEdit {
            get {
                return _showForEdit;
            }
            set {
                _showForEdit = value;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "This property delegates to the method when the user has not yet set a simple display text value.")]
        public virtual string SimpleDisplayText {
            get {
                if (_simpleDisplayText == null) {
                    _simpleDisplayText = GetSimpleDisplayText();
                }
                return _simpleDisplayText;
            }
            set {
                _simpleDisplayText = value;
            }
        }

        public virtual string TemplateHint {
            get;
            set;
        }

        public virtual string Watermark {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static ModelMetadata FromLambdaExpression<TParameter, TValue>(Expression<Func<TParameter, TValue>> expression,
                                                                             ViewDataDictionary<TParameter> viewData) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }
            if (viewData == null) {
                throw new ArgumentNullException("viewData");
            }
            if (expression.Body.NodeType == ExpressionType.Parameter) {    // Expression of "model => model"
                return FromModel(viewData);
            }
            if (expression.Body.NodeType != ExpressionType.MemberAccess) {
                throw new InvalidOperationException(MvcResources.TemplateHelpers_TemplateLimitations);
            }

            TParameter container = viewData.Model;
            Func<object> modelAccessor = () =>
            {
                try {
                    return CachedExpressionCompiler.Process(expression)(container);
                }
                catch (NullReferenceException) {
                    return null;
                }
            };

            MemberExpression memberExpression = (MemberExpression)expression.Body;
            string propertyName = memberExpression.Member is PropertyInfo ? memberExpression.Member.Name : null;
            Type containerType = memberExpression.Member.DeclaringType;

            return GetMetadataFromProvider(modelAccessor, typeof(TValue), propertyName, containerType);
        }

        private static ModelMetadata FromModel(ViewDataDictionary viewData) {
            return viewData.ModelMetadata ?? GetMetadataFromProvider(null, typeof(string), null, null);
        }

        public static ModelMetadata FromStringExpression(string expression, ViewDataDictionary viewData) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }
            if (viewData == null) {
                throw new ArgumentNullException("viewData");
            }
            if (expression.Length == 0) {    // Empty string really means "model metadata for the current model"
                return FromModel(viewData);
            }

            // Start by looking for a property hanging off the existing ModelMetadata
            if (viewData.ModelMetadata != null) {
                ModelMetadata propertyMetadata = viewData.ModelMetadata.Properties.Where(p => p.PropertyName == expression).FirstOrDefault();
                if (propertyMetadata != null) {
                    return propertyMetadata;
                }
            }

            // If we couldn't find it as a property on ModelMetadata, let the ViewData
            // expression parser find us a match
            ViewDataInfo vdi = viewData.GetViewDataInfo(expression);
            Type containerType = null;
            Type modelType = null;
            object model = null;
            string propertyName = null;

            if (vdi != null) {
                if (vdi.Container != null) {
                    containerType = vdi.Container.GetType();
                }

                if (vdi.PropertyDescriptor != null) {
                    propertyName = vdi.PropertyDescriptor.Name;
                    modelType = vdi.PropertyDescriptor.PropertyType;
                }

                model = vdi.Value;
                if (model != null && modelType == null) {
                    modelType = model.GetType();
                }
            }

            return GetMetadataFromProvider(() => model, modelType ?? typeof(string), propertyName, containerType);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The method is a delegating helper to choose among multiple property values")]
        public string GetDisplayName() {
            return DisplayName ?? PropertyName ?? ModelType.Name;
        }

        private static ModelMetadata GetMetadataFromProvider(Func<object> modelAccessor, Type modelType, string propertyName, Type containerType) {
            if (containerType != null && !String.IsNullOrEmpty(propertyName)) {
                return ModelMetadataProviders.Current.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            }
            return ModelMetadataProviders.Current.GetMetadataForType(modelAccessor, modelType);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method is used to resolve the simple display text when it was not explicitly set through other means.")]
        protected virtual string GetSimpleDisplayText() {
            if (Model == null) {
                return NullDisplayText;
            }

            string toStringResult = Convert.ToString(Model, CultureInfo.CurrentCulture);
            if (!toStringResult.Equals(Model.GetType().FullName, StringComparison.Ordinal)) {
                return toStringResult;
            }

            ModelMetadata firstProperty = Properties.FirstOrDefault();
            if (firstProperty == null) {
                return String.Empty;
            }

            if (firstProperty.Model == null) {
                return firstProperty.NullDisplayText;
            }

            return Convert.ToString(firstProperty.Model, CultureInfo.CurrentCulture);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method may perform non-trivial work.")]
        public virtual IEnumerable<ModelValidator> GetValidators(ControllerContext context) {
            return ModelValidatorProviders.Current.GetValidators(this, context);
        }
    }
}
