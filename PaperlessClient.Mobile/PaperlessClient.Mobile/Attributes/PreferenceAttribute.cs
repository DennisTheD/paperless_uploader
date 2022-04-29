using System;
using System.Collections.Generic;
using System.Text;

namespace PaperlessClient.Mobile.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PreferenceAttribute : Attribute
    {
        public Type Type { get; set; }
        public object DefaultValue { get; set; }
        public PreferenceAttribute(Type type, object defaultValue)
        {
            Type = type;
            DefaultValue = defaultValue;
        }
    }
}
