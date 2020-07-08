using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EthCanConfig.Models
{
    class NumberArraySetting : TypedSetting<int[]>
    {
        public override string StringValue
        {
            get
            {
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
                string[] tokens = RemoveWhitespace(value).Split(',');
                int[] result = new int[tokens.Length];
                for(int i = 0;i<tokens.Length;i++)
                {
                    result[i] = int.Parse(tokens[i]);
                }
                TypedValue = result;
            }
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
        public NumberArraySetting(string name) : base(name, null)
        {
        }
        public NumberArraySetting(string name, int[] values) : base(name, values)
        {
        }
    }
}
