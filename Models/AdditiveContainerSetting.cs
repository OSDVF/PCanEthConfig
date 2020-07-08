using EthCanConfig.Conversion;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(AdditiveContainerFormatter))]
    public class AdditiveContainerSetting : ContainerSetting
    {
        [IgnoreDataMember]
        public SettingsTemplate ItemTemplate;
        public AdditiveContainerSetting(string name, SettingsTemplate itemTemplate) : base(name, new ChildObservableCollection<IConfigurationSetting>())
        {
            ItemTemplate = itemTemplate;
        }
        public void AddSetting()
        {
            var clone = ItemTemplate.Clone();
            InnerSettings.Add(clone);
        }

        public void DeleteAll()
        {
            InnerSettings.Clear();
        }
    }
}
