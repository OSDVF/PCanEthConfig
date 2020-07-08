using EthCanConfig.Conversion;
using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(StringSettingFormatter))]
    public class StringSetting : TypedSetting<string>
    {
        public StringSetting(string name, string value):base(name,value)
        { }
    }
    [JsonFormatter(typeof(StringSettingFormatter))]
    public class RegexSetting : StringSetting
    {
        public RegexSetting(string name, string value) : base(name, value)
        { }
    }
}
