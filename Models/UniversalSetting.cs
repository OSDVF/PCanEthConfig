using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace EthCanConfig.Models
{
    public class UniversalSetting : IConfigurationSetting, IContainerSetting
    {
        public string Name { get; set; }
        public virtual dynamic Value { get; set; }
        public ChildObservableCollection<IConfigurationSetting> InnerSettings { get; set; } = new ChildObservableCollection<IConfigurationSetting>();
        public IConfigurationSetting Parent { get; set; }
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
    }

    public interface IConfigurationSetting
    {
        string Name { get; set; }
        abstract object Value {  get; set; }
        IConfigurationSetting Parent { get; set; }
    }
}
