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
            var result = DefaultSettingsObject.defaultSettingsObject.Clone() as ContainerSetting;
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

            var routes = rootInner["routes"] as IContainerSetting;
            var inputRoutes = input["routes"];
            foreach (Dictionary<string, object> inputRoute in inputRoutes)
            {
                bool canin = (string)inputRoute["type"] == "canin";
                routes.InnerSettings.Add(new SettingsTemplate(new ChildObservableCollection<IConfigurationSetting>()
                {
                    new StringSetting("name",inputRoute.ContainsKey("name")? (string)inputRoute["name"]:string.Empty),
                    new EnumSetting("type",canin?RouteType.canin:RouteType.canout),
                    canin ? CanInListenersDeserialize(inputRoute["listeners"]):CanOutListenersDeserialize(inputRoute["listeners"])
                }));
            }

            return result;
        }

        private static dynamic CanOutListenersDeserialize(dynamic listeners)
        {
            throw new NotImplementedException();
        }

        private static IConfigurationSetting CanInListenersDeserialize(dynamic listeners)
        {
            throw new NotImplementedException();
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
