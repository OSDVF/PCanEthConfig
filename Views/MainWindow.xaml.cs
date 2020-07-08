using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EthCanConfig.Conversion;

namespace EthCanConfig.Views
{
    public class MainWindow : Window
    {
        private TextBox previewTextBox;
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
        }
    }

}
