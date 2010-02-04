﻿namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Routing;
    using System.Globalization;
    using System.Web.Mvc.Resources;

    public abstract class ControllerBase : IController {

        private readonly SingleEntryGate _executeWasCalledGate = new SingleEntryGate();

        private TempDataDictionary _tempDataDictionary;
        private bool _validateRequest = true;
        private IDictionary<string, ValueProviderResult> _valueProvider;
        private ViewDataDictionary _viewDataDictionary;

        public ControllerContext ControllerContext {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This property is settable so that unit tests can provide mock implementations.")]
        public TempDataDictionary TempData {
            get {
                if (_tempDataDictionary == null) {
                    _tempDataDictionary = new TempDataDictionary();
                }
                return _tempDataDictionary;
            }
            set {
                _tempDataDictionary = value;
            }
        }

        public bool ValidateRequest {
            get {
                return _validateRequest;
            }
            set {
                _validateRequest = value;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This property is settable so that unit tests can provide mock implementations.")]
        public IDictionary<string, ValueProviderResult> ValueProvider {
            get {
                if (_valueProvider == null) {
                    _valueProvider = new ValueProviderDictionary(ControllerContext);
                }
                return _valueProvider;
            }
            set {
                _valueProvider = value;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "This property is settable so that unit tests can provide mock implementations.")]
        public ViewDataDictionary ViewData {
            get {
                if (_viewDataDictionary == null) {
                    _viewDataDictionary = new ViewDataDictionary();
                }
                return _viewDataDictionary;
            }
            set {
                _viewDataDictionary = value;
            }
        }

        protected virtual void Execute(RequestContext requestContext) {
            if (requestContext == null) {
                throw new ArgumentNullException("requestContext");
            }

            VerifyExecuteCalledOnce();
            Initialize(requestContext);
            ExecuteCore();
        }

        protected abstract void ExecuteCore();

        protected virtual void Initialize(RequestContext requestContext) {
            ControllerContext = new ControllerContext(requestContext, this);
        }

        private void VerifyExecuteCalledOnce() {
            if (!_executeWasCalledGate.TryEnter()) {
                string message = String.Format(CultureInfo.CurrentUICulture, MvcResources.ControllerBase_CannotHandleMultipleRequests, GetType());
                throw new InvalidOperationException(message);
            }
        }

        #region IController Members
        void IController.Execute(RequestContext requestContext) {
            Execute(requestContext);
        }
        #endregion
    }
}
