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
        public string Greeting => "Welcome to Avalonia!";
        public IPAddress ConnectedDevice;
        public string ConnectedDeviceIP => ConnectedDevice == null ? "Unavaliable" : ConnectedDevice.ToString();
        public ObservableCollection<IConfigurationSetting> Settings { get; } = new ObservableCollection<IConfigurationSetting>() {
            new StringSetting("logFile","ethCanRouter.log"),
            new EnumSetting("logLevel",LogLevel.normal),
            new ContainerSetting("net",new ChildObservableCollection<IConfigurationSetting>(new IConfigurationSetting[]{
                new StringSetting("ip","10.10.53.200"),
                new StringSetting("mask","255.0.0.0"),
                new StringSetting("gateway","10.255.255.254"),
                }))
        };

        public IConfigurationSetting SelectedSetting { get; set; }

        private void AddCustomSetting()
        {
            var newCustomSetting = new CustomSetting();
            if(SelectedSetting == null|| SelectedSetting.Parent == null)
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
