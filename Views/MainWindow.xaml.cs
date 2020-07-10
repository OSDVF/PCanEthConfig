using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Html;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using EthCanConfig.Conversion;
using EthCanConfig.Models;
using EthCanConfig.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
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
            this.AttachDevTools(new Avalonia.Input.KeyGesture(Avalonia.Input.Key.F12, Avalonia.Input.KeyModifiers.Control));
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
            viewModel.SettingsObject.PropertyChanged += SettingsObject_PropertyChanged;
        }

        private void SettingsObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            viewModel.Settings.InnerItemChanged += Settings_InnerItemChanged;
        }

        private void Settings_InnerItemChanged(IEnumerable<Models.IConfigurationSetting> item, Models.ChildObservableCollection<Models.IConfigurationSetting> collection)
        {
            previewTextBox.Text = viewModel.JSONPreview;
            viewModel.SettingsDirty = true;
        }

        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filters = {
                    new FileDialogFilter() { Name = "JSON", Extensions = { "json" } },
                    new FileDialogFilter() { Name = "All", Extensions = { "*" } }
                },
                Title = "Open Configuration File"
            };
            var result = await dialog.ShowAsync(this);
            if (result != null)
            {
                using var stream = new FileStream(result[0], FileMode.Open);
                viewModel.SettingsObject = JsonHelper.Deserialize(stream);
                viewModel.SettingsDirty = false;
                viewModel.SavedFileName = result[0];
            }
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            viewModel.SettingsObject = DefaultSettingsObject.GetDefaultSettingsObject;
            viewModel.SettingsDirty = false;
            viewModel.SavedFileName = null;
        }

        private async void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filters = {
                    new FileDialogFilter() { Name = "JSON", Extensions = { "json" } },
                    new FileDialogFilter() { Name = "All", Extensions = { "*" } }
                },
                Title = "Save Configuration File",
                InitialFileName = viewModel.SavedFileName
            };
            var result = await dialog.ShowAsync(this);
            if (result != null)
            {
                viewModel.SavedFileName = result;
                Save(null, null);
            }
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            if (viewModel.SavedFileName == null)
            {
                SaveAs(null, null);
            }
            else
            {
                using var stream = new StreamWriter(viewModel.SavedFileName);
                await stream.WriteAsync(viewModel.JSONPreview);
                viewModel.SettingsDirty = false;
            }
        }

        private new void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyModifiers)
            {
                case KeyModifiers.Control:
                    switch (e.Key)
                    {
                        case Key.S:
                            Save(null, null);
                            e.Handled = true;
                            break;
                        case Key.O:
                            OpenFile(null, null);
                            e.Handled = true;
                            break;
                        case Key.R://Refresh preview
                            viewModel.RaisePropertyChanged();
                            e.Handled = true;
                            break;
                    }
                    break;
            }
        }
    }
    public class BooleanToDirtyTextConverter : FuncValueConverter<bool, string>
    {
        public BooleanToDirtyTextConverter() : base((x => x ? "❌ Unsaved" : "✅ Saved"))
        {
        }
    }
}
