using EthCanConfig.Conversion;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Utf8Json;

namespace EthCanConfig.Models
{
    public class TypedSetting<T> : IConfigurationSetting, ITypedSetting
    {
        public virtual string Name { get; set; }
        public virtual object Value
        {
            get { return TypedValue; }
            set
            {
                if (value is string str)
                    StringValue = str;
                else
                    TypedValue = (T)Convert.ChangeType(value, typeof(T));
            }
        }
        [IgnoreDataMember]
        public T TypedValue
        {
            get => typedValue; set
            {
                typedValue = value;
                OnChanged();
            }
        }

        public Type GetSettingType() => typeof(T);
        [IgnoreDataMember]
        public virtual string StringValue
        {
            get
            {
                return TypedValue.ToString();
            }
            set
            {
                if (typeof(T) == typeof(string))
                {
                    TypedValue = (T)Convert.ChangeType(value,typeof(T));
                }
                else if (typeof(T) == typeof(int))
                {
                    TypedValue = (T)Convert.ChangeType(int.Parse(value), typeof(T));
                }
                else if (typeof(T) == typeof(uint))
                {
                    TypedValue = (T)Convert.ChangeType(uint.Parse(value), typeof(T));
                }
            }
        }
        [IgnoreDataMember]
        public IContainerSetting Parent { get; set; }

        public event SettingChangedEventHandler Changed;
        protected virtual void OnChanged() => Changed?.Invoke(this);
        [IgnoreDataMember]
        private bool _isRequired = true;
        [IgnoreDataMember]
        private bool _isEnabled = true;
        private T typedValue;

        [IgnoreDataMember]
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
        [IgnoreDataMember]
        public bool IsEnabled
        {
            get => _isEnabled; set
            {
                _isEnabled = value;
                Changed?.Invoke(this);
            }
        }
        public TypedSetting(string name, T value)
        {
            Name = name;
            TypedValue = value;
        }
        public IConfigurationSetting Clone()
        {
            var clone = MemberwiseClone() as TypedSetting<T>;
            if(TypedValue is IList coll)//T implements ICollection
            {
                IList newColl = (T)Activator.CreateInstance(typeof (T)) as IList;
                foreach(var item in coll)
                {
                    newColl.Add(item);
                }
                clone.TypedValue = (T)newColl;
            }
            return clone;
        }

        public TEnum StringToEnum<TEnum>(string value, TEnum defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }
    }

    public interface ITypedSetting
    {
        public Type GetSettingType();
    }
}
