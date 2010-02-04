namespace System.Web.Mvc.Html {
    using System.Linq.Expressions;
    using System.Web.UI.WebControls;

    public static class DisplayExtensions {
        public static MvcHtmlString Display(this HtmlHelper html, string expression) {
            return TemplateHelpers.Template(html, expression, null /* templateName */, null /* htmlFieldName */, DataBoundControlMode.ReadOnly);
        }

        public static MvcHtmlString Display(this HtmlHelper html, string expression, string templateName) {
            return TemplateHelpers.Template(html, expression, templateName, null /* htmlFieldName */, DataBoundControlMode.ReadOnly);
        }

        public static MvcHtmlString Display(this HtmlHelper html, string expression, string templateName, string htmlFieldName) {
            return TemplateHelpers.Template(html, expression, templateName, htmlFieldName, DataBoundControlMode.ReadOnly);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression) {
            return TemplateHelpers.TemplateFor(html, expression, null /* templateName */, null /* htmlFieldName */, DataBoundControlMode.ReadOnly);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string templateName) {
            return TemplateHelpers.TemplateFor(html, expression, templateName, null /* htmlFieldName */, DataBoundControlMode.ReadOnly);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string templateName, string htmlFieldName) {
            return TemplateHelpers.TemplateFor(html, expression, templateName, htmlFieldName, DataBoundControlMode.ReadOnly);
        }

        public static MvcHtmlString DisplayForModel(this HtmlHelper html) {
            return MvcHtmlString.Create(TemplateHelpers.TemplateHelper(html, html.ViewData.ModelMetadata, String.Empty, null /* templateName */, DataBoundControlMode.ReadOnly));
        }

        public static MvcHtmlString DisplayForModel(this HtmlHelper html, string templateName) {
            return MvcHtmlString.Create(TemplateHelpers.TemplateHelper(html, html.ViewData.ModelMetadata, String.Empty, templateName, DataBoundControlMode.ReadOnly));
        }

        public static MvcHtmlString DisplayForModel(this HtmlHelper html, string templateName, string htmlFieldName) {
            return MvcHtmlString.Create(TemplateHelpers.TemplateHelper(html, html.ViewData.ModelMetadata, htmlFieldName, templateName, DataBoundControlMode.ReadOnly));
        }
    }
}
