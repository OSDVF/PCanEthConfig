using EthCanConfig.Conversion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EthCanConfig.Models
{
    static class DefaultSettingsObject
    {
        private static int canChannelsCount = 0;
        public static ContainerSetting GetDefaultSettingsObject
        {
            get => new ContainerSetting(null, new ChildObservableCollection<IConfigurationSetting>() {
            new StringSetting("logFile","ethCanRouter.log"),
            new EnumSetting("logLevel", LogLevel.normal),
            new ContainerSetting("net",new ChildObservableCollection<IConfigurationSetting>(new IConfigurationSetting[]{
                new StringSetting("ip","10.10.53.200"),
                new StringSetting("mask","255.0.0.0"),
                new StringSetting("gateway","10.255.255.254"),
                })),
            new AdditiveContainerSetting("channels",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                new StringSetting("name","can"+(canChannelsCount++).ToString()),
                new UnsignedNumberSetting("bitrate",250000)
            })),
            new MultipleAdditiveContainerSetting("routes",new List<SettingsTemplate>(){
                new SettingsTemplate("Ethernet->CAN",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name", string.Empty) { IsRequired = false, IsEnabled = false},
                    new HardCodedSetting("type", RouteType.canout),
                    new AdditiveContainerSetting("listeners",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                        new UnsignedNumberSetting("port",1234),
                        new EnumSetting("protocol", Protocol.udp),
                        new RegexSetting("startsWith","$PMACO") { IsEnabled = false,IsRequired = false},
                        new RegexSetting("endsWith","/\\*[0-9a-fA-F]+/") { IsEnabled = false,IsRequired = false},
                        new BoolSetting("includeBorders",false),
                        new RegexSetting("filter","/.*/") { IsEnabled = false,IsRequired = false},
                        new UnsignedNumberSetting("setFlag", 0) { IsRequired = false, IsEnabled = false},
                        new StringSetting("flagStore", "global storename") { IsRequired = false, IsEnabled = false},
                        new ContainerSetting("converters", Converters.converters)
                    })){IsRequired=false}
                }),
                new SettingsTemplate("CAN->Ethernet",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name", string.Empty) { IsRequired = false, IsEnabled = false},
                    new HardCodedSetting("type", RouteType.canin),
                    new AdditiveContainerSetting("listeners",new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>(){
                        new StringSetting("channel","can0"),
                        new StringSetting("endpoint","192.168.1.2") { IsEnabled = false,IsRequired = false},
                        new UnsignedNumberSetting("port", 25555) { IsEnabled = false,IsRequired = false},
                        new EnumSetting("protocol", Protocol.udp) { IsEnabled = false,IsRequired = false},
                        new HexadecimalSetting("filter",0x1FFFFFFF,8) { IsEnabled = false,IsRequired = false},
                        new HexadecimalSetting("filterMask",0x1FFFFFFF,8) { IsEnabled = false,IsRequired = false},
                        new UnsignedNumberSetting("setFlag", 0) { IsRequired = false, IsEnabled = false},
                        new StringSetting("flagStore", "global storename") { IsRequired = false, IsEnabled = false},
                        new ContainerSetting("converters", Converters.converters)
                    })){IsRequired=false}
                })
            })
        });
        }
    }
}
