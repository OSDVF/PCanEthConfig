using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EthCanConfig.Models;
using System.Collections.ObjectModel;

namespace EthCanConfig.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public IPAddress ConnectedDevice;
        public string ConnectedDeviceIP => ConnectedDevice == null ? "Unavaliable" : ConnectedDevice.ToString();
        public ObservableCollection<ConfigurationSetting> Settings { get; } = new ObservableCollection<ConfigurationSetting>() {
            new ConfigurationSetting("logFile","ethCanRouter.log"),

        };
    }
}
