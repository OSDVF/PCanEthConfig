using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class SettingsTemplate:ContainerSetting
    {
        public SettingsTemplate(ChildObservableCollection<IConfigurationSetting> innerSettings):base(string.Empty, innerSettings)
        {
        }
    }
}
