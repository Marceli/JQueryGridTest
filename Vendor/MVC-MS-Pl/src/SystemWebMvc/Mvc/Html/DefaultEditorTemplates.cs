namespace System.Web.Mvc.Html {
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc.Resources;
    using System.Web.UI.WebControls;

    internal static class DefaultEditorTemplates {
        internal static string BooleanTemplate(HtmlHelper html) {
            bool? value = null;
            if (html.ViewContext.ViewData.Model != null) {
                value = Convert.ToBoolean(html.ViewContext.ViewData.Model, CultureInfo.InvariantCulture);
            }

            return html.ViewContext.ViewData.ModelMetadata.IsNullableValueType
                        ? BooleanTemplateDropDownList(html, value)
                        : BooleanTemplateCheckbox(html, value ?? false);
        }

        private static string BooleanTemplateCheckbox(HtmlHelper html, bool value) {
            return html.CheckBox(String.Empty, value, CreateHtmlAttributes("check-box")).ToHtmlString();
        }

        private static string BooleanTemplateDropDownList(HtmlHelper html, bool? value) {
            return html.DropDownList(String.Empty, TriStateValues(value), CreateHtmlAttributes("list-box tri-state")).ToHtmlString();

        }

        internal static string HiddenInputTemplate(HtmlHelper html) {
            string result;

            if (html.ViewContext.ViewData.ModelMetadata.HideSurroundingChrome) {
                result = String.Empty;
            }
            else {
                result = DefaultDisplayTemplates.StringTemplate(html);
            }

            object model = html.ViewContext.ViewData.Model;

            Binary modelAsBinary = model as Binary;
            if (modelAsBinary != null) {
                model = Convert.ToBase64String(modelAsBinary.ToArray());
            }
            else {
                byte[] modelAsByteArray = model as byte[];
                if (modelAsByteArray != null) {
                    model = Convert.ToBase64String(modelAsByteArray);
                }
            }

            result += html.Hidden(String.Empty, model).ToHtmlString();
            return result;
        }

        internal static string MultilineTextTemplate(HtmlHelper html) {
            return html.TextArea(String.Empty,
                                 html.ViewContext.ViewData.TemplateInfo.FormattedModelValue.ToString(),
                                 0 /* rows */, 0 /* columns */,
                                 CreateHtmlAttributes("text-box multi-line")).ToHtmlString();
        }

        private static IDictionary<string, object> CreateHtmlAttributes(string className) {
            return new Dictionary<string, object>() {
                { "class", className }
            };
        }

        internal static string ObjectTemplate(HtmlHelper html) {
            return ObjectTemplate(html, TemplateHelpers.TemplateHelper);
        }

        internal static string ObjectTemplate(HtmlHelper html, TemplateHelpers.TemplateHelperDelegate templateHelper) {
            ViewDataDictionary viewData = html.ViewContext.ViewData;
            TemplateInfo templateInfo = viewData.TemplateInfo;
            ModelMetadata modelMetadata = viewData.ModelMetadata;
            StringBuilder builder = new StringBuilder();

            if (templateInfo.TemplateDepth > 1) {    // DDB #224751
                return modelMetadata.Model == null ? modelMetadata.NullDisplayText : modelMetadata.SimpleDisplayText;
            }

            foreach (ModelMetadata propertyMetadata in modelMetadata.Properties.Where(pm => pm.ShowForEdit && !templateInfo.Visited(pm))) {
                if (!propertyMetadata.HideSurroundingChrome) {
                    string label = LabelExtensions.LabelHelper(html, propertyMetadata, propertyMetadata.PropertyName).ToHtmlString();
                    if (!String.IsNullOrEmpty(label)) {
                        builder.AppendFormat(CultureInfo.InvariantCulture, "<div class=\"editor-label\">{0}</div>\r\n", label);
                    }

                    builder.Append("<div class=\"editor-field\">");
                }

                builder.Append(templateHelper(html, propertyMetadata, propertyMetadata.PropertyName, null /* templateName */, DataBoundControlMode.Edit));

                if (!propertyMetadata.HideSurroundingChrome) {
                    builder.Append(" ");
                    builder.Append(html.ValidationMessage(propertyMetadata.PropertyName, "*"));
                    builder.Append("</div>\r\n");
                }
            }

            return builder.ToString();
        }

        internal static string PasswordTemplate(HtmlHelper html) {
            return html.Password(String.Empty,
                                 html.ViewContext.ViewData.TemplateInfo.FormattedModelValue,
                                 CreateHtmlAttributes("text-box single-line password")).ToHtmlString();
        }

        internal static string StringTemplate(HtmlHelper html) {
            return html.TextBox(String.Empty,
                                html.ViewContext.ViewData.TemplateInfo.FormattedModelValue,
                                CreateHtmlAttributes("text-box single-line")).ToHtmlString();
        }

        internal static List<SelectListItem> TriStateValues(bool? value) {
            return new List<SelectListItem> {
                new SelectListItem { Text = MvcResources.Common_TriState_NotSet, Value = String.Empty, Selected = !value.HasValue },
                new SelectListItem { Text = MvcResources.Common_TriState_True, Value = "true", Selected = value.HasValue && value.Value },
                new SelectListItem { Text = MvcResources.Common_TriState_False, Value = "false", Selected = value.HasValue && !value.Value },
            };
        }
    }
}
