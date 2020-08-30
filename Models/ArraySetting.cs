using EthCanConfig.Conversion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;

namespace EthCanConfig.Models
{
    public class ArraySetting<T> : TypedSetting<ObservableCollection<T>>
    {
        [IgnoreDataMember]
        public string FancyName => "[ " + Name + " ]";
        [IgnoreDataMember]
        public override string StringValue
        {
            get
            {
                if (TypedValue == null||TypedValue.Count == 0)
                    return string.Empty;

                StringBuilder stringBuilder = new StringBuilder(5);
                foreach (var number in TypedValue)
                {
                    stringBuilder.Append(number);
                    stringBuilder.Append(',');
                }
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

                return stringBuilder.ToString();
            }
            set
            {
                var val = value;
                if (typeof(T) != typeof(string))
                    val = RemoveWhitespace(value);
                string[] tokens = val.Split(',',StringSplitOptions.RemoveEmptyEntries);
                ObservableCollection<T> result = new ObservableCollection<T>();
                for(int i = 0;i<tokens.Length;i++)
                {
                    result.Add((T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(tokens[i]));
                }
                TypedValue = result;
            }
        }

        public override object Value
        {
            get { return TypedValue; }
            set
            {
                if (value is string str)
                    StringValue = str;
                else if (value is IList<object> list)
                {
                    if (TypedValue == null)
                        TypedValue = new ObservableCollection<T>();
                    for(int i=0;i<list.Count;i++)
                    {
                        TypedValue.Add((T)Convert.ChangeType(list[i], typeof(T)));
                    }
                }
                else
                {
                    TypedValue = (ObservableCollection<T>)value;
                }
            }
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
        public ArraySetting(string name) : base(name, null)
        {
        }
        public ArraySetting(string name, ObservableCollection<T> values) : base(name, values)
        {
        }
    }
    [JsonFormatter(typeof(SignedArraySettingsFormatter))]
    public class SignedNumberArraySetting : ArraySetting<int>
    {
        public SignedNumberArraySetting(string name) : base(name)
        {
        }

        public SignedNumberArraySetting(string name, int[] values) : base(name, new ObservableCollection<int>(values))
        {
        }
    }

    [JsonFormatter(typeof(UnsignedArraySettingsFormatter))]
    public class UnsignedNumberArraySetting : ArraySetting<uint>
    {
        public UnsignedNumberArraySetting(string name) : base(name)
        {
        }

        public UnsignedNumberArraySetting(string name, uint[] values) : base(name, new ObservableCollection<uint>(values))
        {
        }
    }
}
