namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Microsoft.Web.Mvc.Internal;
    using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

    public static class ExpressionInputExtensions {
        private const int TextAreaRows = 2;
        private const int TextAreaColumns = 20;

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            return htmlHelper.TextBoxFor(expression, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) {
            return htmlHelper.TextBoxFor(expression, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes) {
            string inputName = ExpressionHelper.GetInputName(expression);

            // REVIEW:  We may not want to actually use the expression to get the default value.
            //          For example, if the property is an Int32, we may want to render blank, not 0.
            //          Consider checking the modelstate first for a value, before we get the value from the expression.
            TProperty value = GetValue(htmlHelper, expression);
            return htmlHelper.TextBox(inputName, value, htmlAttributes);
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            return htmlHelper.TextAreaFor(expression, TextAreaRows, TextAreaColumns, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) {
            return htmlHelper.TextAreaFor(expression, TextAreaRows, TextAreaColumns, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)  {
            return htmlHelper.TextAreaFor(expression, TextAreaRows, TextAreaColumns, htmlAttributes);
        }

        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int rows, int columns, IDictionary<string, object> htmlAttributes) {
            string inputName = ExpressionHelper.GetInputName(expression);

            // REVIEW:  We may not want to actually use the expression to get the default value.
            //          For example, if the property is an Int32, we may want to render blank, not 0.
            //          Consider checking the modelstate first for a value, before we get the value from the expression.
            TProperty value = GetValue(htmlHelper, expression);
            return htmlHelper.TextArea(inputName, Convert.ToString(value, CultureInfo.CurrentCulture), rows, columns, htmlAttributes);
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            return htmlHelper.HiddenFor(expression, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) {
            return htmlHelper.HiddenFor(expression, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes) {
            string inputName = ExpressionHelper.GetInputName(expression);
            TProperty value = GetValue(htmlHelper, expression);
            return htmlHelper.Hidden(inputName, value, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList) {
            return htmlHelper.DropDownListFor(expression, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes) {
            return htmlHelper.DropDownListFor(expression, selectList, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes) {
            string inputName = ExpressionHelper.GetInputName(expression);
            return htmlHelper.DropDownList(inputName, selectList, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel) {
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes) {
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes) {
            string inputName = ExpressionHelper.GetInputName(expression);
            return htmlHelper.DropDownList(inputName, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            return htmlHelper.ValidationMessageFor(expression, null, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string message) {
            return htmlHelper.ValidationMessageFor(expression, message, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string message, object htmlAttributes) {
            return htmlHelper.ValidationMessageFor(expression, message, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string message, IDictionary<string, object> htmlAttributes) {
            string inputName = ExpressionHelper.GetInputName(expression);
            return htmlHelper.ValidationMessage(inputName, message, htmlAttributes);
        }

        internal static TProperty GetValue<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            TModel model = htmlHelper.ViewData.Model;
            if (model == null) {
                return default(TProperty);
            }
            Func<TModel, TProperty> func = CachedExpressionCompiler.Compile(expression);
            return func(model);
        }
    }
}
