﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace EthCanConfig.Models
{
    public class ChildObservableCollection<T> : ObservableCollection<T> where T : IConfigurationSetting
    {
        public delegate void InnerItemChangedhandler(IEnumerable<T> item, ChildObservableCollection<T> collection);
        public event InnerItemChangedhandler InnerItemChanged;
        [IgnoreDataMember]
        private IContainerSetting _parent;
        [IgnoreDataMember]
        public IContainerSetting Parent
        {
            get => _parent; set
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
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnInnerItemChanged(e.NewItems.Cast<T>(), this);
            base.OnCollectionChanged(e);
        }

        protected virtual void OnInnerItemChanged(IEnumerable<T> items, ChildObservableCollection<T> collection)
        {
            InnerItemChanged?.Invoke(items, collection);
            var outer = Parent?.Parent?.InnerSettings;
            outer?.OnInnerItemChanged(items as IEnumerable<IConfigurationSetting>, collection as ChildObservableCollection<IConfigurationSetting>);
        }
    }
}
