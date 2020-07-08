﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EthCanConfig.Models
{
    public class CustomSetting : UniversalSetting, IMovableSetting
    {
        public CustomSetting():base("name","value")
        { }
        public void MoveBack() => (this as IMovableSetting).Back();
        public void MoveForward() => (this as IMovableSetting).Forward();
        public void DeleteItem() => (this as IMovableSetting).Delete();
        public void MoveUp()
        {
            if (Parent is IConfigurationSetting parent&&parent.Parent != null)
            {
                parent.Parent.InnerSettings.Add(this);
                Parent.InnerSettings.Remove(this);
            }
        }
    }
}
