using ReactiveUI;
using System;

namespace EthCanConfig.Models
{
    public class TypedSetting<T> : ReactiveObject, IConfigurationSetting
    {
        public string Name { get; set; }
        public object Value
        {
            get { return TypedValue; }
            set
            {
                TypedValue = (T) Convert.ChangeType(value,typeof(T));
            }
        }
        public T TypedValue { get; set; }
        public virtual string StringValue
        {
            get
            {
                return TypedValue.ToString();
            }
            set
            {
                if (typeof(T).IsEnum)
                {
                    TypedValue = StringToEnum<T>(value, default);
                }
                else if(typeof(T) == typeof(int))
                {
                    TypedValue = (T) Convert.ChangeType(int.Parse(value),typeof(T));
                }
                else if (typeof(T) == typeof(uint))
                {
                    TypedValue = (T)Convert.ChangeType(uint.Parse(value), typeof(T));
                }
            }
        }

        public IContainerSetting Parent { get; set; }
        private bool _isRequired = true;
        private bool _isEnabled = true;

        public bool IsRequired
        {
            get => _isRequired; set
            {
                if (value)
                {
                    IsEnabled = true;
                }
                _isRequired = value;
            }
        }
        public bool IsEnabled
        {
            get => _isEnabled; set
            {
                _isEnabled = value;
                this.RaiseAndSetIfChanged(ref _isEnabled, value);
            }
        }
        public TypedSetting(string name, T value)
        {
            Name = name;
            TypedValue = value;
        }
        public IConfigurationSetting Clone() => MemberwiseClone() as IConfigurationSetting;

        public TEnum StringToEnum<TEnum>(string value, TEnum defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }
    }
}
