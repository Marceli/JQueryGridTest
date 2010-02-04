namespace Microsoft.Web.Mvc {
    using System;
    using System.Runtime.Serialization;
    using System.Web;

    [Serializable]
    public sealed class AsyncException : HttpException {

        public AsyncException() {
        }

        private AsyncException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public AsyncException(string message)
            : base(message) {
        }

        public AsyncException(string message, Exception innerException)
            : base(message, innerException) {
        }

    }
}
