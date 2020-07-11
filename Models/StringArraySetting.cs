using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;
using EthCanConfig.Conversion;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(StringArraySettingsFormatter))]
    public class StringArraySetting : ArraySetting<string>
    {
        public override object Value
        {
            get { return TypedValue; }
            set
            {
                if (value is string str)
                    StringValue = str;
                else if (value is List<object> list)
                {
                    if (TypedValue == null)
                        TypedValue = new string[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        TypedValue[i] = list[i].ToString();
                    }
                }
            }
        }
        public StringArraySetting(string name) : base(name, null)
        {
        }
        public StringArraySetting(string name, string[] values) : base(name, values)
        {
        }
    }
}
