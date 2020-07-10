using EthCanConfig.Conversion;
using MessageBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(MultipleAdditiveContainerFormatter))]
    class MultipleAdditiveContainerSetting : ContainerSetting
    {
        [IgnoreDataMember]
        public List<SettingsTemplate> ItemTemplates;
        [IgnoreDataMember]
        public ICollection<string> ItemTemplateNames
        {
            get
            {
                List<string> stringNames = new List<string>();
                foreach (var item in ItemTemplates)
                {
                    stringNames.Add(item.Name);
                }
                return stringNames;
            }
        }
        [IgnoreDataMember]
        public SettingsTemplate SelectedTemplate { get; set; }
        [IgnoreDataMember]
        public string SelectedTemplateName
        {
            get
            {
                return SelectedTemplate.Name;
            }
            set
            {
                foreach (var temp in ItemTemplates)
                {
                    if (temp.Name == value)
                    {
                        SelectedTemplate = temp;
                    }
                }
            }
        }
        public MultipleAdditiveContainerSetting(string name, ChildObservableCollection<IConfigurationSetting> innerSettings) : base(name, innerSettings)
        {
        }
        public MultipleAdditiveContainerSetting(string name, List<SettingsTemplate> itemTemplates) : base(name, new ChildObservableCollection<IConfigurationSetting>())
        {
            ItemTemplates = itemTemplates;
            SelectedTemplate = itemTemplates.First();
        }
        public void AddSetting()
        {
            if (SelectedTemplate != null)
            {
                var clone = SelectedTemplate.Clone();
                InnerSettings.Add(clone);
            }
            else
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Select a type of settings to add");
                messageBoxStandardWindow.Show();
            }
        }

        public void AddSetting(int templateIndex)
        {
            var clone = ItemTemplates[templateIndex].Clone();
            InnerSettings.Add(clone);
        }
        public void DeleteAll()
        {
            InnerSettings.Clear();
        }
    }
}
