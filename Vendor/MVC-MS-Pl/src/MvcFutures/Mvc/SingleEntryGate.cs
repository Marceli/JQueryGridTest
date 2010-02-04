namespace Microsoft.Web.Mvc {
    using System.Threading;

    public sealed class SingleEntryGate {

        private int _hasFired; // 0 = false, 1 = true

        // returns true if this is the first call to TryEnter(), false otherwise
        public bool TryEnter() {
            int oldValue = Interlocked.Exchange(ref _hasFired, 1);
            return (oldValue == 0);
        }

    }
}
