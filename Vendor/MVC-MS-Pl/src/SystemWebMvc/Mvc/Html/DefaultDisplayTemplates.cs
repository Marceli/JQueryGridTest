namespace System.Web.Mvc.Html {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

    internal static class DefaultDisplayTemplates {
        internal static string BooleanTemplate(HtmlHelper html) {
            bool? value = null;
            if (html.ViewContext.ViewData.Model != null) {
                value = Convert.ToBoolean(html.ViewContext.ViewData.Model, CultureInfo.InvariantCulture);
            }

            return html.ViewContext.ViewData.ModelMetadata.IsNullableValueType
                        ? BooleanTemplateDropDownList(value)
                        : BooleanTemplateCheckbox(value ?? false);
        }

        private static string BooleanTemplateCheckbox(bool value) {
            TagBuilder inputTag = new TagBuilder("input");
            inputTag.AddCssClass("check-box");
            inputTag.Attributes["disabled"] = "disabled";
            inputTag.Attributes["type"] = "checkbox";
            if (value) {
                inputTag.Attributes["checked"] = "checked";
            }

            return inputTag.ToString(TagRenderMode.SelfClosing);
        }

        private static string BooleanTemplateDropDownList(bool? value) {
            StringBuilder builder = new StringBuilder();

            TagBuilder selectTag = new TagBuilder("select");
            selectTag.AddCssClass("list-box");
            selectTag.AddCssClass("tri-state");
            selectTag.Attributes["disabled"] = "disabled";
            builder.Append(selectTag.ToString(TagRenderMode.StartTag));

            foreach (SelectListItem item in DefaultEditorTemplates.TriStateValues(value)) {
                builder.Append(SelectExtensions.ListItemToOption(item));
            }

            builder.Append(selectTag.ToString(TagRenderMode.EndTag));
            return builder.ToString();
        }

        internal static string EmailAddressTemplate(HtmlHelper html) {
            return String.Format(CultureInfo.InvariantCulture,
                                 "<a href=\"mailto:{0}\">{1}</a>",
                                 html.AttributeEncode(html.ViewContext.ViewData.Model),
                                 html.Encode(html.ViewContext.ViewData.TemplateInfo.FormattedModelValue));
        }

        internal static string HiddenInputTemplate(HtmlHelper html) {
            if (html.ViewContext.ViewData.ModelMetadata.HideSurroundingChrome) {
                return String.Empty;
            }
            return StringTemplate(html);
        }

        internal static string HtmlTemplate(HtmlHelper html) {
            return html.ViewContext.ViewData.TemplateInfo.FormattedModelValue.ToString();
        }

        internal static string ObjectTemplate(HtmlHelper html) {
            return ObjectTemplate(html, TemplateHelpers.TemplateHelper);
        }

        internal static string ObjectTemplate(HtmlHelper html, TemplateHelpers.TemplateHelperDelegate templateHelper) {
            ViewDataDictionary viewData = html.ViewContext.ViewData;
            TemplateInfo templateInfo = viewData.TemplateInfo;
            ModelMetadata modelMetadata = viewData.ModelMetadata;
            StringBuilder builder = new StringBuilder();

            if (modelMetadata.Model == null) {    // DDB #225237
                return modelMetadata.NullDisplayText;
            }

            if (templateInfo.TemplateDepth > 1) {    // DDB #224751
                return modelMetadata.SimpleDisplayText;
            }

            foreach (ModelMetadata propertyMetadata in modelMetadata.Properties.Where(pm => pm.ShowForDisplay && !templateInfo.Visited(pm))) {
                if (!propertyMetadata.HideSurroundingChrome) {
                    string label = propertyMetadata.GetDisplayName();
                    if (!String.IsNullOrEmpty(label)) {
                        builder.AppendFormat(CultureInfo.InvariantCulture, "<div class=\"display-label\">{0}</div>", label);
                        builder.AppendLine();
                    }

                    builder.Append("<div class=\"display-field\">");
                }

                builder.Append(templateHelper(html, propertyMetadata, propertyMetadata.PropertyName, null /* templateName */, DataBoundControlMode.ReadOnly));

                if (!propertyMetadata.HideSurroundingChrome) {
                    builder.AppendLine("</div>");
                }
            }

            return builder.ToString();
        }

        internal static string StringTemplate(HtmlHelper html) {
            return html.Encode(html.ViewContext.ViewData.TemplateInfo.FormattedModelValue);
        }

        internal static string UrlTemplate(HtmlHelper html) {
            return String.Format(CultureInfo.InvariantCulture,
                                 "<a href=\"{0}\">{1}</a>",
                                 html.AttributeEncode(html.ViewContext.ViewData.Model),
                                 html.Encode(html.ViewContext.ViewData.TemplateInfo.FormattedModelValue));
        }
    }
}
