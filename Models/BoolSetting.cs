using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    class BoolSetting:TypedSetting<bool>
    {
        public BoolSetting(string name, bool value):base(name,value)
        { }
    }
}
