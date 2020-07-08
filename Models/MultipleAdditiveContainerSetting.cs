using MessageBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EthCanConfig.Models
{
    class MultipleAdditiveContainerSetting : ContainerSetting
    {
        public ICollection<SettingsTemplate> ItemTemplates;
        public ICollection<string> ItemTemplateNames
        {
            get
            {
                List<string> stringNames = new List<string>();
                foreach(var item in ItemTemplates)
                {
                    stringNames.Add(item.Name);
                }
                return stringNames;
            }
        }
        public SettingsTemplate SelectedTemplate { get; set; }
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
        public MultipleAdditiveContainerSetting(string name, ICollection<SettingsTemplate> itemTemplates) : base(name, new ChildObservableCollection<IConfigurationSetting>())
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

        public void DeleteAll()
        {
            InnerSettings.Clear();
        }
    }
}
