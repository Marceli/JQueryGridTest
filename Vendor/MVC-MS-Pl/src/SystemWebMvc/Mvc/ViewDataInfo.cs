namespace System.Web.Mvc {
    using System;
    using System.ComponentModel;

    public class ViewDataInfo {

        public object Container {
            get;
            set;
        }

        public PropertyDescriptor PropertyDescriptor {
            get;
            set;
        }

        public object Value {
            get;
            set;
        }

    }
}
