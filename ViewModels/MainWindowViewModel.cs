using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EthCanConfig.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace EthCanConfig.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static int canChannelsCount = 0;
        public string Greeting => "Welcome to Avalonia!";
        public IPAddress ConnectedDevice;
        public string ConnectedDeviceIP => ConnectedDevice == null ? "Unavaliable" : ConnectedDevice.ToString();
        public ContainerSetting SettingsObject = new ContainerSetting("configuration", new ChildObservableCollection<IConfigurationSetting>() {
            new StringSetting("logFile","ethCanRouter.log"),
            new EnumSetting("logLevel",LogLevel.normal),
            new ContainerSetting("net",new ChildObservableCollection<IConfigurationSetting>(new IConfigurationSetting[]{
                new StringSetting("ip","10.10.53.200"),
                new StringSetting("mask","255.0.0.0"),
                new StringSetting("gateway","10.255.255.254"),
                })),
            new AdditiveContainerSetting("channels",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                new StringSetting("name","can"+(canChannelsCount++).ToString()),
                new NumberSetting("bitrate",250000)
            }))

        });
        public ChildObservableCollection<IConfigurationSetting> Settings => SettingsObject.InnerSettings;

        public IConfigurationSetting SelectedSetting { get; set; }

        private void AddCustomSetting()
        {
            var newCustomSetting = new CustomSetting();
            if(SelectedSetting == null)
            {//Add top level setting
                Settings.Add(newCustomSetting);
            }
            else
            {
                var parent = SelectedSetting.Parent;
                if (parent is IContainerSetting)
                {
                    (parent as IContainerSetting).InnerSettings.Add(newCustomSetting);
                }
                else
                {
                    if(SelectedSetting is IContainerSetting)
                    {
                        (SelectedSetting as IContainerSetting).InnerSettings.Add(newCustomSetting);
                    }
                }
            }
        }
    }
}
