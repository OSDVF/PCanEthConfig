﻿using DynamicData;
using EthCanConfig.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utf8Json;

namespace EthCanConfig.Conversion
{
    public class JsonHelper
    {
        private const string INDENT_STRING = "    ";
        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            if (NewLineIfNotPlainArray(ref str, sb, i))
                                Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();

            static bool NewLineIfNotPlainArray(ref string str, StringBuilder sb, int i)
            {
                if (i < str.Length - 1 && str[i + 1] != '"')
                    return false;
                sb.AppendLine();
                return true;
            }
        }

        public static string SyntaxHighlightJson(string original)
        {
            return Regex.Replace(
              original,
              @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)".Replace('¤', '"'),
              match =>
              {
                  var cls = "number";
                  if (Regex.IsMatch(match.Value, @"^¤".Replace('¤', '"')))
                  {
                      if (Regex.IsMatch(match.Value, ":$"))
                      {
                          cls = "key";
                      }
                      else
                      {
                          cls = "string";
                      }
                  }
                  else if (Regex.IsMatch(match.Value, "true|false"))
                  {
                      cls = "boolean";
                  }
                  else if (Regex.IsMatch(match.Value, "null"))
                  {
                      cls = "null";
                  }
                  return "<span class=\"" + cls + "\">" + match + "</span>";
              });
        }

        public static string ToHTMLPreview(string innerHtml)
        {
            return @"<!DOCTYPE html> 
            <html>
                <body>
            " + innerHtml +
            @"
                </body>
            </html>";
        }

        public static ContainerSetting Deserialize(FileStream stream)
        {
            var input = JsonSerializer.Deserialize<dynamic>((Stream)stream);
            var result = DefaultSettingsObject.GetDefaultSettingsObject;
            var rootInner = result.InnerSettings;

            rootInner["logFile"].Value = input["logFile"];
            rootInner["logLevel"].Value = input["logLevel"];
            var net = rootInner["net"] as IContainerSetting;
            var inputNet = input["net"];
            net.InnerSettings["ip"].Value = inputNet["ip"];
            net.InnerSettings["mask"].Value = inputNet["mask"];
            net.InnerSettings["gateway"].Value = inputNet["gateway"];

            var channels = rootInner["channels"] as IContainerSetting;
            var inputChannels = input["channels"];
            foreach (var inputChannel in inputChannels)
                channels.InnerSettings.Add(new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name",inputChannel["name"]),
                    new UnsignedNumberSetting("bitrate",(uint)inputChannel["bitrate"])
                }));

            var routes = rootInner["routes"] as MultipleAdditiveContainerSetting;
            var inputRoutes = input["routes"];
            foreach (Dictionary<string, object> inputRoute in inputRoutes)
            {
                bool canin = (string)inputRoute["type"] == "canin";
                var listTempate = ((AdditiveContainerSetting)routes.ItemTemplates[canin ? 1 : 0].InnerSettings["listeners"]).ItemTemplate;
                routes.InnerSettings.Add(new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name",inputRoute.ContainsKey("name")? (string)inputRoute["name"]:string.Empty),
                    new HardCodedSetting("type",canin?RouteType.canin:RouteType.canout),
                    canin ? CanInListenersDeserialize(inputRoute["listeners"],listTempate):CanOutListenersDeserialize(inputRoute["listeners"],listTempate)
                }));
            }

            return result;
        }

        private static IConfigurationSetting CanInListenersDeserialize(dynamic listeners, SettingsTemplate listenerTemplate)
        {
            ChildObservableCollection<IConfigurationSetting> outListeners = new ChildObservableCollection<IConfigurationSetting>();
            foreach (var listener in listeners)
            {
                var outputConverters = new ContainerSetting("converters", Converters.converters).Clone() as IContainerSetting;
                outListeners.Add(new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("channel",listener["channel"]),
                    new HexadecimalSetting("filter",GetValOrNull<string>(listener,"filter"),8) { IsEnabled = ContainsKey(listener,"filter"),IsRequired = false},
                    new HexadecimalSetting("filterMask",GetValOrNull<string>(listener,"filterMask"),8) { IsEnabled = ContainsKey(listener,"filterMask"),IsRequired = false},
                    outputConverters
                }));
                DeserializeConverters(listener, outputConverters);

            }
            return new AdditiveContainerSetting("listeners", listenerTemplate) { InnerSettings = outListeners, IsRequired = false };
        }

        private static void AddInnerSettingsByTemplateIndex(int instantiatedTemplateIndex, Dictionary<string, object> converter, ref MultipleAdditiveContainerSetting setting)
        {
            var innerSettings = ((IContainerSetting)setting.InnerSettings[instantiatedTemplateIndex]).InnerSettings;
            foreach (var innerSetting in innerSettings)
            {
                bool keyIsPresent = ContainsKey(converter, innerSetting.Name);
                innerSetting.IsEnabled = keyIsPresent;
                if(keyIsPresent)
                {
                    var val = converter[innerSetting.Name];
                    if (val is string str)
                    {
                        var en = mapStringToEnum(str);
                        if (en == null)
                            innerSetting.Value = str;
                        else innerSetting.Value = en;
                    }
                    else innerSetting.Value = val;
                }
            }
        }

        private static Enum mapStringToEnum(string str)
        {
            switch (str)
            {
                case "uchar":
                    return DataTypes.uchar;
                case "uint4": return DataTypes.uint4;
                case "int4": return DataTypes.int4;
                case "uint8":
                    return DataTypes.uint8;
                case "int8":
                    return DataTypes.int8;
                case "uint16":
                    return DataTypes.uint16;
                case "int16":
                    return DataTypes.int16;
                case "uint32":
                    return DataTypes.uint32;
                case "int32":
                    return DataTypes.int32;

                case "littleEndian":
                    return ByteOrder.littleEndian;
                case "bigEndian":
                    return ByteOrder.bigEndian;

                case "MSB":
                    return BitOrder.MSB;
                case "LSB":
                    return BitOrder.LSB;

                case "AND":
                    return MaskOperations.AND;
                case "OR":
                    return MaskOperations.OR;
            }
            return null;
        }

        private static void DeserializeConverters(dynamic listener, IContainerSetting outputConverters)
        {
            var converterIndex = 0;
            foreach (Dictionary<string, object> inputConverter in listener["converters"]["input"])
            {
                var outputInputConvs = outputConverters.InnerSettings["input"] as MultipleAdditiveContainerSetting;
                if (ContainsKey(inputConverter, "scanf"))
                {
                    outputInputConvs.AddSetting(1);
                }
                else if (inputConverter.ContainsKey("separator"))
                {
                    outputInputConvs.AddSetting(2);
                }
                else if (inputConverter.ContainsKey("regex"))
                {
                    outputInputConvs.AddSetting(3);
                }
                else if (inputConverter.ContainsKey("bits"))
                {
                    outputInputConvs.AddSetting(4);
                }
                else outputInputConvs.AddSetting(0);//Plain parser

                AddInnerSettingsByTemplateIndex(converterIndex, inputConverter, ref outputInputConvs);
                converterIndex++;
            }

            converterIndex = 0;
            foreach (Dictionary<string, object> actionConverter in listener["converters"]["actions"])
            {
                var outputActionConvs = outputConverters.InnerSettings["actions"] as MultipleAdditiveContainerSetting;
                string[] actionTemplates = { "not", "mask", "lshift", "rshift", "concat", "shuffle", "swap", "printf", "dictionary", "sed", "regex", "nmeacc" };
                outputActionConvs.AddSetting(actionTemplates.IndexOf(actionConverter["action"]));
                AddInnerSettingsByTemplateIndex(converterIndex, actionConverter, ref outputActionConvs);
                converterIndex++;
            }

            converterIndex = 0;
            foreach (Dictionary<string, object> outputConverter in listener["converters"]["output"])
            {
                var outputActionConvs = outputConverters.InnerSettings["output"] as MultipleAdditiveContainerSetting;
                string[] outputTemplates = { "printf", "cansend" };
                outputActionConvs.AddSetting(outputTemplates.IndexOf(outputConverter["type"]));
                AddInnerSettingsByTemplateIndex(converterIndex, outputConverter, ref outputActionConvs);
                converterIndex++;
            }
        }

        private static IConfigurationSetting CanOutListenersDeserialize(dynamic listeners, SettingsTemplate listenerTemplate)
        {
            ChildObservableCollection<IConfigurationSetting> outListeners = new ChildObservableCollection<IConfigurationSetting>();
            foreach (var listener in listeners)
            {
                var outputConverters = new ContainerSetting("converters", Converters.converters).Clone() as IContainerSetting;
                outListeners.Add(new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>()
                {
                    new UnsignedNumberSetting("port",(uint)listener["port"]),
                    new EnumSetting("protocol", listener["protocol"] == "udp"?Protocol.udp:Protocol.tcp),
                    new RegexSetting("startsWith",GetValOrNull<string>(listener,"startsWith")) { IsEnabled = ContainsKey(listener,"startsWith"),IsRequired = false},
                    new RegexSetting("endsWith",GetValOrNull<string>(listener,"endsWith")) { IsEnabled = ContainsKey(listener,"endsWith"),IsRequired = false},
                    new BoolSetting("includeBorders",listener["includeBorders"]),
                    new RegexSetting("filter",GetValOrNull<string>(listener,"filter")) { IsEnabled = ContainsKey(listener,"filter"),IsRequired = false},
                    outputConverters
                }));
                DeserializeConverters(listener, outputConverters);

            }
            return new AdditiveContainerSetting("listeners", listenerTemplate) { IsRequired = false, InnerSettings = outListeners };
        }

        public static T GetValOrNull<T>(dynamic obj, string key)
        {
            if (obj is Dictionary<string, object> dict)
            {
                if (dict.ContainsKey(key))
                {
                    return (T)obj[key];
                }
                else return default;
            }
            try
            {
                return (T)obj[key];
            }
            catch (KeyNotFoundException)
            {
                return default;
            }
        }

        public static bool ContainsKey(dynamic obj, string key)
        {
            if (obj is Dictionary<string, object> dict && dict.ContainsKey(key))
            {
                return true;
            }
            return false;
        }
    }

    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }
}
