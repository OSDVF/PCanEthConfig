using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EthCanConfig.Models
{
    public class ContainerSetting : UniversalSetting
    {
        public override object Value { get => InnerSettings; set => InnerSettings = value as ChildObservableCollection<IConfigurationSetting>; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="innerSettings">Parent will be set automatically</param>
        public ContainerSetting(string name, ChildObservableCollection<IConfigurationSetting> innerSettings) : base(name, innerSettings)
        {
            Name = name;
            if (innerSettings != null)
            {
                InnerSettings = innerSettings;
            }
            InnerSettings.Parent = this;
        }
    }

    public class ChildObservableCollection<T> : ObservableCollection<T> where T: IConfigurationSetting
    {
        private IContainerSetting _parent;
        public IContainerSetting Parent { get => _parent; set
            {
                _parent = value;
                foreach (var item in this)
                {
                    item.Parent = _parent;
                }
            }
        }
        public ChildObservableCollection(IContainerSetting parent) : base()
        {
            Parent = parent;
        }
        public ChildObservableCollection() : base()
        {
        }
        public ChildObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }
        public ChildObservableCollection(IEnumerable<T> collection, IContainerSetting parent) : base(collection)
        {
            Parent = parent;
        }

        protected override void InsertItem(int index, T item)
        {
            item.Parent = _parent;
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, T item)
        {
            item.Parent = _parent;
            base.SetItem(index, item);
        }
    }

    public interface IContainerSetting
    {
        ChildObservableCollection<IConfigurationSetting> InnerSettings { get; }
    }
}
