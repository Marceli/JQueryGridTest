﻿namespace Microsoft.Web.Mvc {
    using System;

    // This class is used to wait for both a signal and a continuation. When both have been provided,
    // the continuation is called.

    internal sealed class ContinuationListener {

        private volatile Action _continuation;
        private volatile bool _continuationSet;

        private volatile bool _wasSignaled;

        private readonly SingleEntryGate _firedGate = new SingleEntryGate();

        private void Fire() {
            if (_continuationSet && _wasSignaled) {
                if (_firedGate.TryEnter()) {
                    _continuation();
                }
            }
        }

        public void SetContinuation(Action continuation) {
            _continuationSet = true;
            _continuation = continuation;

            Fire();
        }

        public void Signal() {
            _wasSignaled = true;

            Fire();
        }

    }
}
