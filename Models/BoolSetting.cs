using EthCanConfig.Conversion;
using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(BoolSettingFormatter))]
    class BoolSetting:TypedSetting<bool>
    {
        public BoolSetting(string name, bool value):base(name,value)
        { }
    }
}
