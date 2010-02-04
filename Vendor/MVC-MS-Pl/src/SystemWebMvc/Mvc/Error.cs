namespace System.Web.Mvc {
    using System;
    using System.Globalization;
    using System.Web.Mvc.Resources;

    internal static class Error {

        public static ArgumentException ParameterCannotBeNullOrEmpty(string parameterName) {
            return new ArgumentException(MvcResources.Common_NullOrEmpty, parameterName);
        }

        public static InvalidOperationException PropertyCannotBeNullOrEmpty(string propertyName) {
            string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.Common_PropertyCannotBeNullOrEmpty,
                propertyName);
            return new InvalidOperationException(message);
        }

        public static InvalidOperationException ViewDataDictionary_WrongTModelType(Type valueType, Type modelType) {
            string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.ViewDataDictionary_WrongTModelType,
                valueType, modelType);
            return new InvalidOperationException(message);
        }

        public static InvalidOperationException ViewDataDictionary_ModelCannotBeNull(Type modelType) {
            string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.ViewDataDictionary_ModelCannotBeNull,
                modelType);
            return new InvalidOperationException(message);
        }

    }
}
