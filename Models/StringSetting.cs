using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class StringSetting : TypedSetting<string>
    {
        public StringSetting(string name, string value):base(name,value)
        { }
    }
}
