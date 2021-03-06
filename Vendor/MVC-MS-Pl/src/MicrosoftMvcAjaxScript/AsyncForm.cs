namespace Sys.Mvc {
    using System;
    using System.DHTML;
    using Sys.UI;

    public static class AsyncForm {

        public static void HandleClick(FormElement form, DomEvent evt) {
            string additionalInput = MvcHelpers.SerializeSubmitButton(evt.Target, evt.OffsetX, evt.OffsetY);
            Type.SetField(form, "_additionalInput", additionalInput);
        }

        public static void HandleSubmit(FormElement form, DomEvent evt, AjaxOptions ajaxOptions) {
            evt.PreventDefault();
            string body = MvcHelpers.SerializeForm(form);
            MvcHelpers.AsyncRequest(form.Action, 
                                    form.Method ?? "post",
                                    body,
                                    form,
                                    ajaxOptions);

        }

    }
}
