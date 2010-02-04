namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public static class ScriptExtensions {
        public static MvcHtmlString Script(this HtmlHelper helper, string file) {
            if (String.IsNullOrEmpty(file)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "file");
            }

            string src;
            if (IsRelativeToDefaultPath(file)) {
                src = "~/Scripts/" + file;
            }
            else {
                src = file;
            }

            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            TagBuilder scriptTag = new TagBuilder("script");
            scriptTag.MergeAttribute("type", "text/javascript");
            scriptTag.MergeAttribute("src", urlHelper.Content(src));
            return MvcHtmlString.Create(scriptTag.ToString());
        }

        internal static bool IsRelativeToDefaultPath(string file) {
            return !(file.StartsWith("~", StringComparison.Ordinal) ||
                file.StartsWith("../", StringComparison.Ordinal) ||
                file.StartsWith("/", StringComparison.Ordinal) ||
                file.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                file.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }
    }
}
