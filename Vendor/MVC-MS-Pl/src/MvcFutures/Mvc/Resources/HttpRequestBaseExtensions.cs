namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Extension methods that facilitate support for content negotiation and HTTP method overload.
    /// </summary>
    public static class HttpRequestBaseExtensions {
        /// <summary>
        /// Returns the format of a given request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The format of the request.</returns>
        /// <exception cref="HttpException">If the format is unrecognized or not supported.</exception>
        public static ContentType GetRequestFormat(this HttpRequestBase request) {
            return HttpHelper.GetRequestFormat(request);
        }

        /// <summary>
        /// Returns the preferred content type to use for the response, based on the request, according to the following
        /// rules:
        /// 1. If the query string contains a key called "format", its value is returned as the content type
        /// 2. Otherwise, if the request has an Accepts header, the list of content types in order of preference is returned
        /// 3. Otherwise, if the request has a content type, its value is returned
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The formats to use for rendering a response.</returns>
        public static List<ContentType> GetResponseFormats(this HttpRequestBase request) {
            return HttpHelper.GetResponseFormats(request);
        }

        internal static bool HasBody(this HttpRequestBase request) {
            return request.ContentLength > 0 || string.Compare("chunked", request.Headers["Transfer-Encoding"], StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Determines whether the specified HTTP request was sent by a Browser. A request is considered to be from the browser
        /// if it's a GET or POST and has a known User-Agent header (as determined by the request's BrowserCapabilities property),
        /// and does not have a non-HTML entity format (XML/JSON)
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>true if the specified HTTP request is a Browser request; otherwise, false.</returns>
        public static bool IsBrowserRequest(this HttpRequestBase request) {
            return request.IsBrowserRequest(false);
        }

        internal static bool IsBrowserRequest(this HttpRequestBase request, bool skipRequestFormatCheck) {
            if (!request.IsHttpMethod(HttpVerbs.Get) && !request.IsHttpMethod(HttpVerbs.Post)) {
                return false;
            }
            if (!skipRequestFormatCheck) {
                ContentType requestFormat = request.GetRequestFormat();
                if (requestFormat == null || string.Compare(requestFormat.MediaType, FormatManager.UrlEncoded, StringComparison.OrdinalIgnoreCase) != 0) {
                    if (FormatManager.Current.CanDeserialize(requestFormat)) {
                        return false;
                    }
                }
            }
            HttpBrowserCapabilitiesBase browserCapabilities = request.Browser;
            if (browserCapabilities != null && !string.IsNullOrEmpty(request.Browser.Browser) && request.Browser.Browser != "Unknown") {
                return true;
            }
            return false;
        }

        public static bool IsHttpMethod(this HttpRequestBase request, HttpVerbs httpMethod) {
            return request.IsHttpMethod(httpMethod, false);
        }

        public static bool IsHttpMethod(this HttpRequestBase request, string httpMethod) {
            return request.IsHttpMethod(httpMethod, false);
        }

        // CODEREVIEW: this impleemntation kind of misses the point of HttpVerbs
        // by falling back to string comparison, consider something better
        // also, how do we keep this switch in sync?
        public static bool IsHttpMethod(this HttpRequestBase request, HttpVerbs httpMethod, bool allowOverride) {
            switch (httpMethod) {
                case HttpVerbs.Get:
                    return request.IsHttpMethod("GET", allowOverride);
                case HttpVerbs.Post:
                    return request.IsHttpMethod("POST", allowOverride);
                case HttpVerbs.Put:
                    return request.IsHttpMethod("PUT", allowOverride);
                case HttpVerbs.Delete:
                    return request.IsHttpMethod("DELETE", allowOverride);
                case HttpVerbs.Head:
                    return request.IsHttpMethod("HEAD", allowOverride);
                default:
                    // CODEREVIEW: does this look reasonable?
                    return request.IsHttpMethod(httpMethod.ToString().ToUpperInvariant(), allowOverride);
            }
        }

        public static bool IsHttpMethod(this HttpRequestBase request, string httpMethod, bool allowOverride) {
            string requestHttpMethod = allowOverride ? request.GetHttpMethodOverride() : request.HttpMethod;
            return String.Equals(requestHttpMethod, httpMethod, StringComparison.OrdinalIgnoreCase);
        }
    }
}
