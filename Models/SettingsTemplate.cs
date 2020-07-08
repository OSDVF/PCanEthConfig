using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class SettingsTemplate : ContainerSetting, IMovableSetting
    {
        public SettingsTemplate(ChildObservableCollection<IConfigurationSetting> innerSettings):base(string.Empty, innerSettings)
        {
        }

        public SettingsTemplate(string name, ChildObservableCollection<IConfigurationSetting> innerSettings) : base(name, innerSettings)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }

        public void MoveBack() => (this as IMovableSetting).Back();
        public void MoveForward() => (this as IMovableSetting).Forward();
        public void DeleteItem() => (this as IMovableSetting).Delete();
    }

    public interface IMovableSetting : IConfigurationSetting
    {
        public void Back()
        {
            var oldIndex = Parent.InnerSettings.IndexOf(this);
            var newIndex = Math.Max(0, oldIndex - 1);
            Parent.InnerSettings.Move(oldIndex, newIndex);
        }
        public void Forward()
        {
            var set = Parent.InnerSettings;
            var oldIndex = set.IndexOf(this);
            var newIndex = Math.Min(set.Count - 1, oldIndex + 1);
            set.Move(oldIndex, newIndex);
        }

        public void Delete()
        {
            Parent.InnerSettings.Remove(this);
        }
    }
}
