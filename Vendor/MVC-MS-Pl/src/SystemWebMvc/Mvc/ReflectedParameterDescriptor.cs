namespace System.Web.Mvc {
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public class ReflectedParameterDescriptor : ParameterDescriptor {

        private readonly ActionDescriptor _actionDescriptor;
        private readonly ReflectedParameterBindingInfo _bindingInfo;

        public ReflectedParameterDescriptor(ParameterInfo parameterInfo, ActionDescriptor actionDescriptor) {
            if (parameterInfo == null) {
                throw new ArgumentNullException("parameterInfo");
            }
            if (actionDescriptor == null) {
                throw new ArgumentNullException("actionDescriptor");
            }

            ParameterInfo = parameterInfo;
            _actionDescriptor = actionDescriptor;
            _bindingInfo = new ReflectedParameterBindingInfo(parameterInfo);
        }

        public override ActionDescriptor ActionDescriptor {
            get {
                return _actionDescriptor;
            }
        }

        public override ParameterBindingInfo BindingInfo {
            get {
                return _bindingInfo;
            }
        }

        public override object DefaultValue {
            get {
                // this will get the default value as seen by the VB / C# compilers
                // if no value was baked in, RawDefaultValue returns DBNull.Value
                object rawDefaultValue = ParameterInfo.RawDefaultValue;
                if (rawDefaultValue != DBNull.Value) {
                    return rawDefaultValue;
                }

                // if the compiler did not bake in a default value, check the [DefaultValue] attribute
                DefaultValueAttribute[] attrs = (DefaultValueAttribute[])ParameterInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attrs == null || attrs.Length == 0) {
                    return base.DefaultValue;
                }
                else {
                    return attrs[0].Value;
                }
            }
        }

        public ParameterInfo ParameterInfo {
            get;
            private set;
        }

        public override string ParameterName {
            get {
                return ParameterInfo.Name;
            }
        }

        public override Type ParameterType {
            get {
                return ParameterInfo.ParameterType;
            }
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return ParameterInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return ParameterInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return ParameterInfo.IsDefined(attributeType, inherit);
        }

    }
}
