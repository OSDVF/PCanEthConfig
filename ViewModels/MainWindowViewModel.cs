using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EthCanConfig.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Net.Http.Headers;
using EthCanConfig.Conversion;

namespace EthCanConfig.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static int canChannelsCount = 0;
        public string Greeting => "Welcome to Avalonia!";
        public IPAddress ConnectedDevice;
        public string ConnectedDeviceIP => ConnectedDevice == null ? "Unavaliable" : ConnectedDevice.ToString();
        public ContainerSetting SettingsObject = new ContainerSetting(null, new ChildObservableCollection<IConfigurationSetting>() {
            new StringSetting("logFile","ethCanRouter.log"),
            new EnumSetting("logLevel",LogLevel.normal),
            new ContainerSetting("net",new ChildObservableCollection<IConfigurationSetting>(new IConfigurationSetting[]{
                new StringSetting("ip","10.10.53.200"),
                new StringSetting("mask","255.0.0.0"),
                new StringSetting("gateway","10.255.255.254"),
                })),
            new AdditiveContainerSetting("channels",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                new StringSetting("name","can"+(canChannelsCount++).ToString()),
                new UnsignedNumberSetting("bitrate",250000)
            })),
            new MultipleAdditiveContainerSetting("routes",new Collection<SettingsTemplate>(){
                new SettingsTemplate("Ethernet->CAN",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name",string.Empty){IsRequired=false, IsEnabled=false},
                    new HardCodedSetting("type",RouteType.canin),
                    new AdditiveContainerSetting("listeners",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                        new StringSetting("channel","can0"),
                        new HexadecimalSetting("filter",0x1FFFFFFF,8){ IsEnabled=false,IsRequired=false},
                        new HexadecimalSetting("filterMask",0x1FFFFFFF,8){ IsEnabled=false,IsRequired=false},
                        new ContainerSetting("converters",Converters.converters)
                    })){IsRequired=false}
                }),
                new SettingsTemplate("CAN->Ethernet",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name",string.Empty){IsRequired=false, IsEnabled=false},
                    new HardCodedSetting("type",RouteType.canout),
                    new AdditiveContainerSetting("listeners",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                        new UnsignedNumberSetting("port",1234),
                        new EnumSetting("protocol",Protocol.udp),
                        new RegexSetting("startsWith","$PMACO"){ IsEnabled=false,IsRequired=false},
                        new RegexSetting("endsWith","/\\*[0-9a-fA-F]+/"){ IsEnabled=false,IsRequired=false},
                        new BoolSetting("includeBorders",false),
                        new RegexSetting("filter","/.*/"){ IsEnabled=false,IsRequired=false},
                        new ContainerSetting("converters",Converters.converters)
                    })){IsRequired=false}
                })
            })
        });
        public ChildObservableCollection<IConfigurationSetting> Settings => SettingsObject.InnerSettings;

        public IConfigurationSetting SelectedSetting { get; set; }

        public string JSONPreview => Utf8Json.JsonSerializer.PrettyPrint(ToJSON.Serialize(SettingsObject));

        private void AddCustomSetting()
        {
            var newCustomSetting = new CustomSetting();
            if (SelectedSetting == null)
            {//Add top level setting
                Settings.Add(newCustomSetting);
            }
            else
            {
                var parent = SelectedSetting.Parent;
                if(SelectedSetting is CustomSetting)
                {
                    (SelectedSetting as IContainerSetting).InnerSettings.Add(newCustomSetting);
                }
                else if (parent is IContainerSetting)
                {
                    parent.InnerSettings.Add(newCustomSetting);
                }
                else
                {
                    if (SelectedSetting is IContainerSetting)
                    {
                        (SelectedSetting as IContainerSetting).InnerSettings.Add(newCustomSetting);
                    }
                }
            }
        }
    }
}
