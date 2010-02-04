namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// Extension methods that facilitate support for content negotiation and HTTP method overload.
    /// The results for GetHttpMethod() GetRequestFormat() and GetResponseFormats() are cached on the RouteData dictionary.
    /// </summary>
    public static class RequestContextExtensions {
        const string requestFormatKey = "requestFormat";
        const string responseFormatKey = "responseFormat";

        /// <summary>
        /// Returns the format of a given request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The format of the request.</returns>
        /// <exception cref="HttpException">If the format is unrecognized or not supported.</exception>
        public static ContentType GetRequestFormat(this RequestContext requestContext) {
            return RequestContextExtensions.GetAndCacheObject(requestContext, requestFormatKey, HttpHelper.GetRequestFormat) as ContentType;
        }

        /// <summary>
        /// Returns a collection of formats that should be used to render a response to a given request, sorted in priority order.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The formats to use for rendering a response.</returns>
        public static List<ContentType> GetResponseFormats(this RequestContext requestContext) {
            return RequestContextExtensions.GetAndCacheObject(requestContext, responseFormatKey, HttpHelper.GetResponseFormats) as List<ContentType>;
        }

        /// <summary>
        /// Determines whether the specified HTTP request was sent by a Browser.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>true if the specified HTTP request is a Browser request; otherwise, false.</returns>
        public static bool IsBrowserRequest(this RequestContext requestContext) {
            return requestContext.HttpContext.Request.IsBrowserRequest();
        }

        static object GetAndCacheObject(RequestContext requestContext, string key, Func<HttpRequestBase, object> getter) {
            object value;
            if (!requestContext.HttpContext.Items.Contains(key)) {
                value = getter.Invoke(requestContext.HttpContext.Request);
                requestContext.HttpContext.Items.Add(key, value);
            }
            else {
                value = requestContext.HttpContext.Items[key];
            }
            return value;
        }
    }
}
