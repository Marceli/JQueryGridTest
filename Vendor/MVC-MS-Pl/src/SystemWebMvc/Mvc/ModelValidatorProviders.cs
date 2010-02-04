namespace System.Web.Mvc {
    public static class ModelValidatorProviders {
        private static ModelValidatorProvider _current = new DataAnnotationsModelValidatorProvider();

        public static ModelValidatorProvider Current {
            get {
                return _current;
            }
            set {
                _current = value ?? new EmptyModelValidatorProvider();
            }
        }
    }
}
