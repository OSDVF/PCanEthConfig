using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EthCanConfig.Views
{
    public class ConfigTreeView : UserControl
    {
        public ConfigTreeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}