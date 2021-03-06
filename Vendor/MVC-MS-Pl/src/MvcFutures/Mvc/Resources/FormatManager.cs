namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Mime;
    using System.Web.Mvc;

    /// <summary>
    /// Class that maintains a registration of handlers for
    /// request and response formats
    /// </summary>
    public class FormatManager {
        public const string UrlEncoded = "application/x-www-form-urlencoded";

        static FormatManager current = new DefaultFormatManager();

        Collection<IRequestFormatHandler> requestHandlers;
        Collection<IResponseFormatHandler> responseHandlers;

        public FormatManager() {
            this.requestHandlers = new Collection<IRequestFormatHandler>();
            this.responseHandlers = new Collection<IResponseFormatHandler>();
        }

        /// <summary>
        /// The list of handlers that can parse the request body
        /// </summary>
        public Collection<IRequestFormatHandler> RequestFormatHandlers { get { return this.requestHandlers; } }

        /// <summary>
        /// The list of handlers that can serialize the response body
        /// </summary>
        public Collection<IResponseFormatHandler> ResponseFormatHandlers { get { return this.responseHandlers; } }

        public static FormatManager Current {
            get { return current; }
            set {
                if (value == null) throw new ArgumentNullException("value");
                current = value;
            }
        }

        public bool TryDeserialize(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestFormat, out object model) {
            for (int i = 0; i < this.RequestFormatHandlers.Count; ++i) {
                if (this.RequestFormatHandlers[i].CanDeserialize(requestFormat)) {
                    model = this.RequestFormatHandlers[i].Deserialize(controllerContext, bindingContext, requestFormat);
                    return true;
                }
            }
            model = null;
            return false;
        }

        public bool CanDeserialize(ContentType contentType) {
            for (int i = 0; i < this.RequestFormatHandlers.Count; ++i) {
                if (this.RequestFormatHandlers[i].CanDeserialize(contentType)) {
                    return true;
                }
            }
            return false;
        }

        public bool CanSerialize(ContentType responseFormat) {
            for (int i = 0; i < this.ResponseFormatHandlers.Count; ++i) {
                if (this.ResponseFormatHandlers[i].CanSerialize(responseFormat)) {
                    return true;
                }
            }
            return false;
        }

        public void Serialize(ControllerContext context, object model, ContentType responseFormat) {
            for (int i = 0; i < this.ResponseFormatHandlers.Count; ++i) {
                if (this.ResponseFormatHandlers[i].CanSerialize(responseFormat)) {
                    this.ResponseFormatHandlers[i].Serialize(context, model, responseFormat);
                    return;
                }
            }
            throw new NotSupportedException();
        }

        public bool TryMapFormatFriendlyName(string formatName, out ContentType contentType) {
            for (int i = 0; i < this.ResponseFormatHandlers.Count; ++i) {
                if (this.ResponseFormatHandlers[i].TryMapFormatFriendlyName(formatName, out contentType)) {
                    return true;
                }
            }
            contentType = null;
            return false;
        }

        class DefaultFormatManager : FormatManager {
            public DefaultFormatManager() {
                XmlFormatHandler xmlHandler = new XmlFormatHandler();
                JsonFormatHandler jsonHandler = new JsonFormatHandler();
                this.RequestFormatHandlers.Add(xmlHandler);
                this.RequestFormatHandlers.Add(jsonHandler);
                this.ResponseFormatHandlers.Add(xmlHandler);
                this.ResponseFormatHandlers.Add(jsonHandler);
            }
        }
    }
}
