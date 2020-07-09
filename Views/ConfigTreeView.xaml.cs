using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using EthCanConfig.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace EthCanConfig.Views
{
    public class ConfigTreeView : UserControl
    {
        TreeView mainTree;
        public ConfigTreeView() => InitializeComponent();

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            mainTree = this.FindControl<TreeView>("mainTreeView");
        }

        private void InnerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sendTrv = (TreeView)sender;
            mainTree.UnselectAll();
            foreach (var control in mainTree.GetLogicalDescendants())
            {
                if (control is TreeView trv && trv != sendTrv)
                {
                    trv.UnselectAll();
                }
            }
            e.Handled = true;
            try
            {
                ((MainWindowViewModel)DataContext).SelectedSetting = (Models.IConfigurationSetting)sendTrv.SelectedItem;
            }
            catch (System.Exception)
            {
                Debug.WriteLine("Could not set SelectedSetting");
            }
        }

        private void OnCheckBoxCheck(object sender, RoutedEventArgs e)//Because bindings don't work :(
        {
            var chb = (CheckBox)sender;
            var parent = chb.Parent;
            if (parent != null && parent.Parent != null)
                foreach (var sibling in parent.Parent.LogicalChildren)
                {
                    if (sibling is Control contr && contr.Opacity == 0.5)
                    {
                        contr.Opacity = 1;
                    }
                }
        }

        private void OnCheckBoxUncheck(object sender, RoutedEventArgs e)
        {
            var chb = (CheckBox)sender;
            var parent = chb.Parent;
            if (parent != null && parent.Parent != null)
                foreach (var sibling in parent.Parent.LogicalChildren)
                {
                    if (sibling is Control contr && contr.Opacity == 1.0)
                    {
                        contr.Opacity = 0.5;
                    }
                }
        }
    }
    public class EnabledToOpacityConverter : FuncValueConverter<bool, double>
    {
        public EnabledToOpacityConverter() : base((x => x ? 1.0 : 0.5))
        {
        }
    }
}