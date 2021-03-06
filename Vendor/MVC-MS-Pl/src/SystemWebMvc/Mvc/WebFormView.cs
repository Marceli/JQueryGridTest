﻿namespace System.Web.Mvc {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Web.Mvc.Resources;

    public class WebFormView : IView {

        private IBuildManager _buildManager;

        public WebFormView(string viewPath)
            : this(viewPath, null) {
        }

        public WebFormView(string viewPath, string masterPath) {
            if (String.IsNullOrEmpty(viewPath)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "viewPath");
            }

            ViewPath = viewPath;
            MasterPath = masterPath ?? String.Empty;
        }

        internal IBuildManager BuildManager {
            get {
                if (_buildManager == null) {
                    _buildManager = new BuildManagerWrapper();
                }
                return _buildManager;
            }
            set {
                _buildManager = value;
            }
        }

        public string MasterPath {
            get;
            private set;
        }

        public string ViewPath {
            get;
            private set;
        }

        public virtual void Render(ViewContext viewContext, TextWriter writer) {
            if (viewContext == null) {
                throw new ArgumentNullException("viewContext");
            }

            object viewInstance = BuildManager.CreateInstanceFromVirtualPath(ViewPath, typeof(object));
            if (viewInstance == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        MvcResources.WebFormViewEngine_ViewCouldNotBeCreated,
                        ViewPath));
            }

            ViewPage viewPage = viewInstance as ViewPage;
            if (viewPage != null) {
                RenderViewPage(viewContext, viewPage, writer);
                return;
            }

            ViewUserControl viewUserControl = viewInstance as ViewUserControl;
            if (viewUserControl != null) {
                RenderViewUserControl(viewContext, viewUserControl, writer);
                return;
            }

            throw new InvalidOperationException(
                String.Format(
                    CultureInfo.CurrentUICulture,
                    MvcResources.WebFormViewEngine_WrongViewBase,
                    ViewPath));
        }

        private void RenderViewPage(ViewContext context, ViewPage page, TextWriter textWriter) {
            if (!String.IsNullOrEmpty(MasterPath)) {
                page.MasterLocation = MasterPath;
            }

            page.ViewData = context.ViewData;
            page.SetTextWriter(textWriter);
            page.RenderView(context);
        }

        private void RenderViewUserControl(ViewContext context, ViewUserControl control, TextWriter textWriter) {
            if (!String.IsNullOrEmpty(MasterPath)) {
                throw new InvalidOperationException(MvcResources.WebFormViewEngine_UserControlCannotHaveMaster);
            }

            control.ViewData = context.ViewData;
            control.SetTextWriter(textWriter);
            control.RenderView(context);
        }
    }
}
