using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class ConfigurationSetting
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }
        public ConfigurationSetting(string name, dynamic value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"[{Name}] = {Value}";
        }
    }
}
