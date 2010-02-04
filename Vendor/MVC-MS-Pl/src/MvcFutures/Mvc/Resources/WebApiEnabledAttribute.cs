namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Attribute indicating that the controller supports multiple formats (HTML, XML, JSON etc), HTTP method based dispatch
    /// and HTTP error handling.
    /// </summary>
    public class WebApiEnabledAttribute : ActionFilterAttribute, IExceptionFilter {
        public WebApiEnabledAttribute()
            : base() {
            this.StatusOnNullModel = HttpStatusCode.NotFound;
        }

        /// <summary>
        /// The HTTP status code to use in case a null value is returned from the controller action method.
        /// The default is NotFound
        /// </summary>
        public HttpStatusCode StatusOnNullModel { get; set; }

        public static bool IsDefined(ControllerBase controller) {
            Type controllerType = controller.GetType();
            WebApiEnabledAttribute[] rea = controllerType.GetCustomAttributes(typeof(WebApiEnabledAttribute), true) as WebApiEnabledAttribute[];
            return rea != null && rea.Length > 0;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            MultiFormatActionResult multiFormatResult = filterContext.Result as MultiFormatActionResult;
            if (multiFormatResult == null) {
                ViewResultBase viewResult = filterContext.Result as ViewResultBase;
                if (viewResult != null && viewResult.ViewData != null) {
                    bool handled = false;
                    foreach (ContentType responseFormat in filterContext.RequestContext.GetResponseFormats()) {
                        if (TryGetResult(viewResult, responseFormat, out multiFormatResult)) {
                            if (multiFormatResult != null) {
                                filterContext.Result = new MultiFormatActionResult(viewResult.ViewData.Model, responseFormat);
                            }
                            handled = true;
                            break;
                        }
                    }
                    if (!handled) {
                        // enumeration above should never get to the end, if it does the request is not acceptable
                        throw new HttpException((int)HttpStatusCode.NotAcceptable, "None of the formats specified by the accept header is supported.");
                    }
                }
            }
            base.OnActionExecuted(filterContext);
            RedirectToRouteResult redirectResult = filterContext.Result as RedirectToRouteResult;
            if (redirectResult != null && !filterContext.RequestContext.IsBrowserRequest()) {
                filterContext.Result = new ResourceRedirectToRouteResult(redirectResult);
            }
        }

        public void OnException(ExceptionContext filterContext) {
            if (filterContext.ExceptionHandled) {
                return;
            }
            HttpException he = filterContext.Exception as HttpException;
            if (he != null) {
                ResourceErrorActionResult rear;
                if (WebApiEnabledAttribute.TryGetErrorResult2(filterContext.RequestContext, he, out rear)) {
                    if (rear != null) {
                        filterContext.Result = rear;
                        filterContext.ExceptionHandled = true;
                    }
                    return;
                }
                // enumeration above should never get to the end, if it does the request is not acceptable
                throw new HttpException((int)HttpStatusCode.NotAcceptable, "None of the formats specified by the accept header is supported.");
            }
        }

        public virtual bool TryGetErrorResult(HttpException exception, ContentType responseFormat, out ResourceErrorActionResult actionResult) {
            if (FormatManager.Current.CanSerialize(responseFormat)) {
                actionResult = new ResourceErrorActionResult(exception, responseFormat);
                return true;
            }
            switch (responseFormat.MediaType) {
                case "application/octet-stream":
                case "application/x-www-form-urlencoded":
                case "text/html":
                case "*/*":
                    actionResult = null;
                    return true;
                default:
                    actionResult = null;
                    return false;
            }
        }

        public virtual bool TryGetResult(ViewResultBase viewResult, ContentType responseFormat, out MultiFormatActionResult actionResult) {
            if (FormatManager.Current.CanSerialize(responseFormat)) {
                if (viewResult.ViewData.Model == null) {
                    throw new HttpException((int)this.StatusOnNullModel, this.StatusOnNullModel.ToString());
                }
                actionResult = new MultiFormatActionResult(viewResult.ViewData.Model, responseFormat);
                return true;
            }

            switch (responseFormat.MediaType) {
                case "application/octet-stream":
                case "application/x-www-form-urlencoded":
                case "text/html":
                case "*/*":
                    actionResult = null;
                    return true;
                default:
                    actionResult = null;
                    return false;
            }
        }

        internal static bool TryGetErrorResult2(RequestContext requestContext, HttpException he, out ResourceErrorActionResult actionResult) {
            List<ContentType> responseFormats = requestContext.GetResponseFormats();
            for (int i = 0; i < responseFormats.Count; ++i) {
                ContentType responseFormat = responseFormats[i];
                WebApiEnabledAttribute dummy = new WebApiEnabledAttribute();
                if (dummy.TryGetErrorResult(he, responseFormat, out actionResult)) {
                    return true;
                }
            }
            actionResult = null;
            return false;
        }
    }
}
