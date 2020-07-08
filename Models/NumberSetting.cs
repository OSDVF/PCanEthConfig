using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    class NumberSetting : TypedSetting<int>
    {
        public NumberSetting(string name, int value) : base(name, value)
        {
        }
    }
    class UnsignedNumberSetting : TypedSetting<uint>
    {
        public UnsignedNumberSetting(string name, uint value) : base(name, value) { }
    }
}