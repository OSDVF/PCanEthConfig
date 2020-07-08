using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class CustomSetting : UniversalSetting
    {
        public CustomSetting():base("name","value")
        { }

        void MoveBack()
        {
            var oldIndex = Parent.InnerSettings.IndexOf(this);
            var newIndex = Math.Max(0, oldIndex - 1);
            Parent.InnerSettings.Move(oldIndex, newIndex);
        }
        void MoveForward()
        {
            var set = Parent.InnerSettings;
            var oldIndex = set.IndexOf(this);
            var newIndex = Math.Min(set.Count - 1, oldIndex + 1);
            set.Move(oldIndex, newIndex);
        }

        void Delete()
        {
            Parent.InnerSettings.Remove(this);
        }
    }
}
