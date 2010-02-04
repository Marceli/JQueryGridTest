namespace Microsoft.Web.Mvc {
    using System;
    using System.Data.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Microsoft.Web.Mvc.Internal;
    using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

    public static class BinaryHtmlExtensions {

        public static MvcHtmlString HiddenFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, byte[]>> expression) {
            string inputName = ExpressionHelper.GetInputName(expression);
            byte[] value = ExpressionInputExtensions.GetValue(htmlHelper, expression);
            return htmlHelper.Hidden(inputName, value);
        }

        public static MvcHtmlString HiddenFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, Binary>> expression) {
            string inputName = ExpressionHelper.GetInputName(expression);
            Binary value = ExpressionInputExtensions.GetValue(htmlHelper, expression);
            return htmlHelper.Hidden(inputName, value.ToArray());
        }
    }
}
