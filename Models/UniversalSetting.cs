using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReactiveUI;
using System.Runtime.Serialization;
using Utf8Json;
using EthCanConfig.Conversion;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(ConfigurationFormatter))]
    public class UniversalSetting : IConfigurationSetting, IContainerSetting, INotifyPropertyChanged
    {
        public string Name
        {
            get => name; set
            {
                name = value;
                OnChanged();
            }
        }

        protected virtual void OnChanged() => Changed?.Invoke(this);

        public virtual dynamic Value
        {
            get => _value; set
            {
                _value = value;
                OnChanged();
            }
        }
        public ChildObservableCollection<IConfigurationSetting> InnerSettings
        {
            get => innerSettings; set
            {
                innerSettings = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InnerSettings"));
                innerSettings.InvokeCollectionChanged();
            }
        }
        [IgnoreDataMember]
        public IContainerSetting Parent { get; set; }
        public UniversalSetting(string name, dynamic value)
        {
            Name = name;
            Value = value;
            InnerSettings.Parent = this;
        }

        public override string ToString()
        {
            return $"[{Name}] = {Value}";
        }
        public IConfigurationSetting Clone()
        {
            var clone = MemberwiseClone() as UniversalSetting;
            var innerSettings = new ChildObservableCollection<IConfigurationSetting>(clone);
            foreach (var setting in InnerSettings)
            {
                innerSettings.Add(setting.Clone());
            }
            clone.InnerSettings = innerSettings;
            return clone;
        }

        public event SettingChangedEventHandler Changed;
        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreDataMember]
        private bool _isRequired = true;
        [IgnoreDataMember]
        private bool _isEnabled = true;
        private string name;
        private dynamic _value;
        private ChildObservableCollection<IConfigurationSetting> innerSettings = new ChildObservableCollection<IConfigurationSetting>();

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
                OnChanged();
            }
        }
    }

    public class HardCodedSetting : UniversalSetting
    {
        public HardCodedSetting(string name, dynamic value) : base(name, (object)value)
        { }
    }

    [JsonFormatter(typeof(ConfigurationFormatter))]
    public interface IConfigurationSetting : IObservableSetting
    {
        string Name { get; set; }
        abstract object Value { get; set; }
        IContainerSetting Parent { get; set; }
        IConfigurationSetting Clone();
        bool IsRequired { get; set; }
        bool IsEnabled { get; set; }
    }

    public interface IObservableSetting
    {
        event SettingChangedEventHandler Changed;
    }

    public delegate void SettingChangedEventHandler(IObservableSetting sender);
}
