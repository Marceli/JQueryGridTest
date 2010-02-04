namespace System.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    [FileLevelControlBuilder(typeof(ViewPageControlBuilder))]
    public class ViewPage : Page, IViewDataContainer {

        private string _masterLocation;
        private ViewDataDictionary _viewData;
        private TextWriter _textWriter;

        public AjaxHelper<object> Ajax {
            get;
            set;
        }

        public HtmlHelper<object> Html {
            get;
            set;
        }

        public string MasterLocation {
            get {
                return _masterLocation ?? String.Empty;
            }
            set {
                _masterLocation = value;
            }
        }

        public object Model {
            get {
                return ViewData.Model;
            }
        }

        public TempDataDictionary TempData {
            get {
                return ViewContext.TempData;
            }
        }

        public UrlHelper Url {
            get;
            set;
        }

        public ViewContext ViewContext {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This is the mechanism by which the ViewPage gets its ViewDataDictionary object.")]
        public ViewDataDictionary ViewData {
            get {
                if (_viewData == null) {
                    SetViewData(new ViewDataDictionary());
                }
                return _viewData;
            }
            set {
                SetViewData(value);
            }
        }

        public HtmlTextWriter Writer {
            get;
            private set;
        }

        public virtual void InitHelpers() {
            Ajax = new AjaxHelper<object>(ViewContext, this);
            Html = new HtmlHelper<object>(ViewContext, this);
            Url = new UrlHelper(ViewContext.RequestContext);
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnPreInit(EventArgs e) {
            base.OnPreInit(e);

            if (!String.IsNullOrEmpty(MasterLocation)) {
                MasterPageFile = MasterLocation;
            }
        }

        protected override void Render(HtmlTextWriter writer) {
            Writer = writer;
            try {
                base.Render(writer);
            }
            finally {
                Writer = null;
            }
        }

        public virtual void RenderView(ViewContext viewContext) {
            ViewContext = viewContext;
            InitHelpers();
            // Tracing requires Page IDs to be unique.
            ID = Guid.NewGuid().ToString();
            viewContext.HttpContext.Server.Execute(this, _textWriter, true /* preserveForm */);
        }

        public void SetTextWriter(TextWriter textWriter) {
            _textWriter = textWriter;
        }

        protected virtual void SetViewData(ViewDataDictionary viewData) {
            _viewData = viewData;
        }
    }
}
