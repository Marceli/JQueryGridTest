﻿namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc.Resources;

    public class ModelBindingContext {

        private static readonly Predicate<string> _defaultPropertyFilter = _ => true;

        private string _modelName;
        private ModelStateDictionary _modelState;
        private Predicate<string> _propertyFilter;
        private Dictionary<string, ModelMetadata> _propertyMetadata;

        public bool FallbackToEmptyPrefix {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Cannot remove setter as that's a breaking change")]
        public object Model {
            get {
                return ModelMetadata.Model;
            }
            set {
                throw new InvalidOperationException(MvcResources.ModelMetadata_PropertyNotSettable);
            }
        }

        public ModelMetadata ModelMetadata {
            get;
            set;
        }

        public string ModelName {
            get {
                if (_modelName == null) {
                    _modelName = String.Empty;
                }
                return _modelName;
            }
            set {
                _modelName = value;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The containing type is mutable.")]
        public ModelStateDictionary ModelState {
            get {
                if (_modelState == null) {
                    _modelState = new ModelStateDictionary();
                }
                return _modelState;
            }
            set {
                _modelState = value;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Cannot remove setter as that's a breaking change")]
        public Type ModelType {
            get {
                return ModelMetadata.ModelType;
            }
            set {
                throw new InvalidOperationException(MvcResources.ModelMetadata_PropertyNotSettable);
            }
        }

        public Predicate<string> PropertyFilter {
            get {
                if (_propertyFilter == null) {
                    _propertyFilter = _defaultPropertyFilter;
                }
                return _propertyFilter;
            }
            set {
                _propertyFilter = value;
            }
        }

        public IDictionary<string, ModelMetadata> PropertyMetadata {
            get {
                if (_propertyMetadata == null) {
                    _propertyMetadata = ModelMetadata.Properties.ToDictionary(m => m.PropertyName);
                }

                return _propertyMetadata;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "The containing type is mutable.")]
        public IDictionary<string, ValueProviderResult> ValueProvider {
            get;
            set;
        }

    }
}
