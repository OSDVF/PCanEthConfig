using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EthCanConfig.Conversion;
using EthCanConfig.ViewModels;

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
            viewModel.Settings.CollectionChanged += Settings_CollectionChanged;
        }

        private void Settings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            previewTextBox.Text = viewModel.JSONPreview;
        }
    }

}
