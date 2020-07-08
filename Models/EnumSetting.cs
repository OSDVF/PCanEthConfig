using EthCanConfig.Conversion;
using System;
using System.Collections.ObjectModel;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(EnumSettingFormatter))]
    public class EnumSetting : TypedSetting<Enum>, IConfigurationSetting
    {
        public Array Values => Enum.GetValues(TypedValue.GetType());
        public EnumSetting(string name, Enum value) : base(name, value)
        { }
    }

    public interface IEnumSetting : IConfigurationSetting
    {
        Array Values { get; }
    }
}
