using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Html;
using Avalonia.Markup.Xaml;
using EthCanConfig.Conversion;
using EthCanConfig.ViewModels;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EthCanConfig.Views
{
    public class MainWindow : Window
    {
        private TextBox previewTextBox;
        private MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools(new Avalonia.Input.KeyGesture(Avalonia.Input.Key.F12,Avalonia.Input.KeyModifiers.Control));
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            previewTextBox = this.FindControl<TextBox>("previewTextBox");
            DataContextChanged += MainWindow_DataContextChanged;
        }

        private void MainWindow_DataContextChanged(object sender, System.EventArgs e)
        {
            viewModel = DataContext as MainWindowViewModel;
            viewModel.Settings.InnerItemChanged += Settings_InnerItemChanged;
        }

        private void Settings_InnerItemChanged(IEnumerable<Models.IConfigurationSetting> item, Models.ChildObservableCollection<Models.IConfigurationSetting> collection)
        {
            previewTextBox.Text = viewModel.JSONPreview;
        }
    }

}
