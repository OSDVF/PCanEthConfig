using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class AdditiveContainerSetting : ContainerSetting
    {
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
