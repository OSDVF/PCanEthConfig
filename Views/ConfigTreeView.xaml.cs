using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using EthCanConfig.ViewModels;
using System.Diagnostics;

namespace EthCanConfig.Views
{
    public class ConfigTreeView : UserControl
    {
        TreeView mainTree;
        public ConfigTreeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            mainTree = this.FindControl<TreeView>("mainTreeView");
        }

        private void innerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sendTrv = (TreeView)sender;
            mainTree.UnselectAll();
            foreach (var control in mainTree.GetLogicalDescendants())
            {
                var trv = control as TreeView;
                if (trv != null && trv != sendTrv)
                {
                    trv.UnselectAll();
                }
            }
            e.Handled = true;
            try
            {
                ((MainWindowViewModel)DataContext).SelectedSetting = (Models.IConfigurationSetting)sendTrv.SelectedItem;
            }
            catch(System.Exception)
            {
                Debug.WriteLine("Could not set SelectedSetting");
            }
        }
    }
}