using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EthCanConfig.Models
{
    public class TypedSetting<T> : IConfigurationSetting
    {
        public string Name { get; set; }
        public object Value
        {
            get { return TypedValue; }
            set
            {
                TypedValue = (T)value;
            }
        }
        public T TypedValue { get; set; }
        public string StringValue
        {
            get
            {
                return TypedValue.ToString();
            }
            set
            {
                if (typeof(T).IsEnum)
                {
                    TypedValue = StringToEnum<T>(value, default);
                }
            }
        }

        public IConfigurationSetting Parent { get; set; }

        public TypedSetting(string name, T value)
        {
            Name = name;
            TypedValue = value;
        }

        public TEnum StringToEnum<TEnum>(string value, TEnum defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }
    }
}
