namespace System.Web.Mvc {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Script.Serialization;

    public class FormContext {

        private const string _defaultClientValidationFunction = "EnableClientValidation";

        private string _clientValidationFunction;
        private readonly Dictionary<string, FieldValidationMetadata> _fieldValidators = new Dictionary<string, FieldValidationMetadata>();

        public bool ClientValidationEnabled {
            get;
            set;
        }

        public string ClientValidationFunction {
            get {
                if (String.IsNullOrEmpty(_clientValidationFunction)) {
                    _clientValidationFunction = _defaultClientValidationFunction;
                }
                return _clientValidationFunction;
            }
            set {
                _clientValidationFunction = value;
            }
        }

        public object ClientValidationState {
            get;
            set;
        }

        public IDictionary<string, FieldValidationMetadata> FieldValidators {
            get {
                return _fieldValidators;
            }
        }

        public string FormId {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Performs a potentially time-consuming conversion.")]
        public string GetJsonValidationMetadata() {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            SortedDictionary<string, object> dict = new SortedDictionary<string,object>() {
                { "Fields", FieldValidators.Values },
                { "FormId", FormId }
            };

            return serializer.Serialize(dict);
        }

        public FieldValidationMetadata GetValidationMetadataForField(string fieldName) {
            return GetValidationMetadataForField(fieldName, false /* createIfNotFound */);
        }

        public FieldValidationMetadata GetValidationMetadataForField(string fieldName, bool createIfNotFound) {
            if (String.IsNullOrEmpty(fieldName)) {
                throw Error.ParameterCannotBeNullOrEmpty("fieldName");
            }

            FieldValidationMetadata metadata;
            if (!FieldValidators.TryGetValue(fieldName, out metadata)) {
                if (createIfNotFound) {
                    metadata = new FieldValidationMetadata() {
                        FieldName = fieldName
                    };
                    FieldValidators[fieldName] = metadata;
                }
            }
            return metadata;
        }

    }
}
