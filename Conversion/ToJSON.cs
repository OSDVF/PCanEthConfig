using DynamicData.Annotations;
using EthCanConfig.Models;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Utf8Json;
using Utf8Json.Resolvers;

namespace EthCanConfig.Conversion
{
    class ToJSON
    {
        public static void SerializeToFile<T>(T rootNode, string fileName)
        {
            JsonSerializer.Serialize(new FileStream(fileName, FileMode.Create, FileAccess.Write), rootNode);
        }

        public static string Serialize<T>(T rootNode)
        {
            return Encoding.UTF8.GetString(JsonSerializer.Serialize(rootNode));
        }

        public static string Serialize(ICollection<IConfigurationSetting> nodes)
        {
            return Encoding.UTF8.GetString(JsonSerializer.Serialize(nodes));
        }
    }

    class ConfigurationFormatter : IJsonFormatter<IConfigurationSetting>
    {
        public virtual IConfigurationSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Serialize(ref JsonWriter writer, IConfigurationSetting value, IJsonFormatterResolver formatterResolver)
        {
            writer.WritePropertyName(value.Name);
            writer.WriteString(value.Value.ToString());
            WriteSeparatorIfNotTheLast(ref writer, value);
        }

        public static void WriteSeparatorIfNotTheLast(ref JsonWriter writer, IConfigurationSetting value)
        {
            if (value.Parent != null && value.Parent.InnerSettings.IndexOf(value) != value.Parent.InnerSettings.Count - 1)
                writer.WriteValueSeparator();
        }
    }

    class TypedFormatter<T> : ConfigurationFormatter, IJsonFormatter<TypedSetting<T>>
    {
        public override IConfigurationSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, TypedSetting<T> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WritePropertyName(value.Name);
            writer.WriteString(value.StringValue);
            WriteSeparatorIfNotTheLast(ref writer, value);
        }

        TypedSetting<T> IJsonFormatter<TypedSetting<T>>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }
    }

    class StringSettingFormatter : ConfigurationFormatter, IJsonFormatter<StringSetting>
    {
        public override IConfigurationSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, StringSetting value, IJsonFormatterResolver formatterResolver)
        {
            base.Serialize(ref writer, value, formatterResolver);
        }

        StringSetting IJsonFormatter<StringSetting>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }
    }

    class EnumSettingFormatter : ConfigurationFormatter, IJsonFormatter<EnumSetting>
    {
        public override IConfigurationSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, EnumSetting value, IJsonFormatterResolver formatterResolver)
        {
            base.Serialize(ref writer, value, formatterResolver);
        }

        EnumSetting IJsonFormatter<EnumSetting>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }
    }

    class ContainerConfigurationFormatter : IJsonFormatter<ContainerSetting>
    {
        public virtual ContainerSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, ContainerSetting value, IJsonFormatterResolver formatterResolver)
        {
            if (!string.IsNullOrEmpty(value.Name))
                writer.WritePropertyName(value.Name);
            writer.WriteBeginObject();
            WriteInnerSettings(ref writer,value,formatterResolver);
            writer.WriteEndObject();
            ConfigurationFormatter.WriteSeparatorIfNotTheLast(ref writer, value);
        }

        public static void WriteInnerSettings(ref JsonWriter writer, ContainerSetting value, IJsonFormatterResolver formatterResolver)
        {
            foreach (var inner in value.InnerSettings)
            {
                dynamic genericFormatter = formatterResolver.GetFormatterDynamic(inner.GetType());
                if (inner is StringSetting typed)
                {
                    genericFormatter.Serialize(ref writer, typed, formatterResolver);
                }
                else if (inner is NumberSetting num)
                {
                    genericFormatter.Serialize(ref writer, num, formatterResolver);
                }
                else if (inner is AdditiveContainerSetting adt)
                {
                    genericFormatter.Serialize(ref writer, adt, formatterResolver);
                }
                else if (inner is MultipleAdditiveContainerSetting mlt)
                {
                    genericFormatter.Serialize(ref writer, mlt, formatterResolver);
                }
                else if (inner is ContainerSetting cnt)
                {
                    genericFormatter.Serialize(ref writer, cnt, formatterResolver);
                }
                else if (inner is EnumSetting en)
                {
                    genericFormatter.Serialize(ref writer, en, formatterResolver);
                }
                else
                    genericFormatter.Serialize(ref writer, inner, formatterResolver);
            }
        }
    }

    class AdditiveContainerFormatter : ContainerConfigurationFormatter, IJsonFormatter<AdditiveContainerSetting>
    {
        public override ContainerSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, AdditiveContainerSetting value, IJsonFormatterResolver formatterResolver)
        {
            if (!string.IsNullOrEmpty(value.Name))
                writer.WritePropertyName(value.Name);
            writer.WriteBeginArray();
            WriteInnerSettings(ref writer, value, formatterResolver);
            writer.WriteEndArray();
            ConfigurationFormatter.WriteSeparatorIfNotTheLast(ref writer, value);
        }

        AdditiveContainerSetting IJsonFormatter<AdditiveContainerSetting>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }
    }

    class MultipleAdditiveContainerFormatter : AdditiveContainerFormatter, IJsonFormatter<MultipleAdditiveContainerSetting>
    {
        public override ContainerSetting Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, MultipleAdditiveContainerSetting value, IJsonFormatterResolver formatterResolver)
        {
            base.Serialize(ref writer, value, formatterResolver);
        }

        MultipleAdditiveContainerSetting IJsonFormatter<MultipleAdditiveContainerSetting>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new System.NotImplementedException();
        }
    }
}
