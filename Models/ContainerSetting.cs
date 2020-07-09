using EthCanConfig.Conversion;
using System;
using System.Text;
using Utf8Json;

namespace EthCanConfig.Models
{
    [JsonFormatter(typeof(ContainerConfigurationFormatter))]
    public class ContainerSetting : UniversalSetting
    {
        public override object Value { get => InnerSettings; set => InnerSettings = value as ChildObservableCollection<IConfigurationSetting>; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="innerSettings">Parent will be set automatically</param>
        public ContainerSetting(string name, ChildObservableCollection<IConfigurationSetting> innerSettings) : base(name, innerSettings)
        {
            Name = name;
            if (innerSettings != null)
            {
                InnerSettings = innerSettings;
            }
            InnerSettings.Parent = this;
        }
    }

    public interface IContainerSetting: IConfigurationSetting
    {
        ChildObservableCollection<IConfigurationSetting> InnerSettings { get; }
    }
}
