namespace System.Web.Mvc.Html {
    using System;
    using System.Web.Script.Serialization;
    using System.Globalization;

    public class MvcForm : IDisposable {

        private const string _clientValidationScript = @"<script type=""text/javascript"">
//<![CDATA[
{0}({1}, {2});
//]]>
</script>";

        private bool _disposed;

        private readonly HttpResponseBase _httpResponse;
        private readonly FormContext _originalFormContext;
        private readonly ViewContext _viewContext;

        public MvcForm(HttpResponseBase httpResponse) {
            if (httpResponse == null) {
                throw new ArgumentNullException("httpResponse");
            }
            _httpResponse = httpResponse;
        }

        internal MvcForm(ViewContext viewContext) {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }

            _viewContext = viewContext;
            _httpResponse = viewContext.HttpContext.Response;

            // push the new FormContext
            _originalFormContext = viewContext.FormContext;
            viewContext.FormContext = new FormContext();
        }

        public void Dispose() {
            Dispose(true /* disposing */);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                _disposed = true;
                _httpResponse.Write("</form>");

                // output client validation and restore the original form context
                if (_viewContext != null) {
                    OutputClientValidation(_httpResponse, _viewContext.FormContext);
                    _viewContext.FormContext = _originalFormContext;
                }
            }
        }

        public void EndForm() {
            Dispose(true);
        }

        private static void OutputClientValidation(HttpResponseBase response, FormContext formContext) {
            if (!formContext.ClientValidationEnabled) {
                return; // do nothing
            }

            // output a call that resembles:
            // _clientValidationFunction(validationObject, userContext);

            string validationJson = formContext.GetJsonValidationMetadata();

            string userContextJson = (formContext.ClientValidationState != null)
                ? new JavaScriptSerializer().Serialize(formContext.ClientValidationState)
                : "null";

            string scriptWithCorrectNewLines = _clientValidationScript.Replace("\r\n", Environment.NewLine);
            string formatted = String.Format(CultureInfo.InvariantCulture, scriptWithCorrectNewLines,
                formContext.ClientValidationFunction, validationJson, userContextJson);

            response.Write(formatted);
        }

    }
}
