using EthCanConfig.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EthCanConfig.Conversion
{
    static class Converters
    {
        public static ChildObservableCollection<IConfigurationSetting> converters = new ChildObservableCollection<IConfigurationSetting>(){
            new MultipleAdditiveContainerSetting("input",new List<SettingsTemplate>()
            {
                new SettingsTemplate("Plain",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name","plainParser") { IsRequired = false },
                }),
                new SettingsTemplate("Scanf",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name","scanfParser") { IsRequired = false },
                    new StringSetting("scanf","%d,%d"),
                    new UnsignedNumberArraySetting("shuffle") { IsRequired = false,IsEnabled = false},
                }),
                new SettingsTemplate("Separator",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name","separatorParser") { IsRequired = false },
                    new StringSetting("separator",","),
                    new UnsignedNumberArraySetting("shuffle") { IsRequired = false,IsEnabled = false},
                }),
                new SettingsTemplate("Regex",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name","regexParser") { IsRequired = false },
                    new RegexSetting("regex","/.*/"),
                    new UnsignedNumberArraySetting("shuffle") { IsRequired = false,IsEnabled = false},
                }),
                new SettingsTemplate("Bit Separator",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name","plainParser") { IsRequired = false },
                    new StringSetting("bits","0-24u,25-32"),
                    new EnumSetting("byteOrder", ByteOrder.bigEndian),
                    new EnumSetting("bitOrder", BitOrder.MSB),
                    new UnsignedNumberArraySetting("shuffle") { IsRequired = false,IsEnabled = false},
                }),
            }),
            new MultipleAdditiveContainerSetting("actions",new List<SettingsTemplate>()
            {
                new SettingsTemplate("not",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","not"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false}
                }),
                new SettingsTemplate("mask",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","mask"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false},
                    new StringSetting("mask","FF"),
                    new EnumSetting("op", MaskOperations.OR)
                }),
                new SettingsTemplate("lshift",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","lshift"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false},
                    new UnsignedNumberSetting("amount",0)
                }),
                new SettingsTemplate("rshift",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","rshift"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false},
                    new UnsignedNumberSetting("amount",0)
                }),
                new SettingsTemplate("concat",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","concat"),
                    new StringSetting("firstName", string.Empty),
                    new UnsignedNumberSetting("firstIndex",0),
                    new StringSetting("secondName", string.Empty),
                    new UnsignedNumberSetting("secondIndex",0),
                }),
                new SettingsTemplate("shuffle",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","shuffle"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberArraySetting("shuffle")
                }),
                new SettingsTemplate("swap",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","swap"),
                    new StringSetting("firstName", string.Empty),
                    new UnsignedNumberSetting("firstIndex",0),
                    new StringSetting("secondName", string.Empty),
                    new UnsignedNumberSetting("secondIndex",0),
                }),
                new SettingsTemplate("printf",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","printf"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberArraySetting("inputIndexes"),
                    new StringSetting("format", string.Empty),
                    new StringSetting("destinationName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("destinationIndex",0) { IsRequired = false},
                }),
                new SettingsTemplate("sed",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","sed"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false},
                }),
                new SettingsTemplate("regex",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","regex"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false},
                    new StringSetting("regex", string.Empty),
                    new StringSetting("destinationName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("destinationIndex",0) { IsRequired = false},
                }),
                new SettingsTemplate("nmeacc",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("action","nmeacc"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("inputIndex",0) { IsRequired = false},
                    new StringSetting("destinationName", string.Empty) { IsRequired = false},
                    new UnsignedNumberSetting("destinationIndex",0) { IsRequired = false},
                }),
            }),
            new MultipleAdditiveContainerSetting("output",new List<SettingsTemplate>()
            {
                new SettingsTemplate("printf",new ChildObservableCollection<IConfigurationSetting>()
                {
                    new HardCodedSetting("type","printf"),
                    new StringSetting("inputName", string.Empty) { IsRequired = false},
                    new StringSetting("format", string.Empty),
                })
            })
        };
    }
}
