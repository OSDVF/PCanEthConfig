using EthCanConfig.Conversion;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(EnumSettingFormatter))]
    public class EnumSetting : TypedSetting<Enum>, IConfigurationSetting, IEnumSetting
    {
        public Array Values => Enum.GetValues(TypedValue.GetType());
        public EnumSetting(string name, Enum value) : base(name, value)
        { }

        public override string StringValue
        {
            get => base.StringValue;
            set
            {
                TypedValue = (Enum)Enum.Parse(Values.GetValue(0).GetType(), value, true);
            }
        }
    }

    public interface IEnumSetting : IConfigurationSetting
    {
        Array Values { get; }
    }
    [JsonFormatter(typeof(EnumArraySettingFormatter))]
    public class EnumArraySetting : ArraySetting<Enum>
    {
        private Enum defaultValue;
        public int SelectedIndex { get; set; }
        public Array Values => Enum.GetValues(defaultValue.GetType());
        public EnumArraySetting(string name, Enum defaultValue) : base(name, new ObservableCollection<Enum>(new Enum[] { defaultValue }))
        {
            this.defaultValue = defaultValue;
        }

        public void AddNewItem()
        {
            TypedValue.Add((Enum)Values.GetValue(SelectedIndex));
            OnChanged();
        }

        public void RemoveItem(Enum item)
        {
            TypedValue.Remove(item);
            OnChanged();
        }
    }
}
