using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EthCanConfig.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Net.Http.Headers;
using EthCanConfig.Conversion;
using Avalonia.Controls;
using ReactiveUI;

namespace EthCanConfig.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public IPAddress ConnectedDevice;
        public string ConnectedDeviceIP => ConnectedDevice == null ? "Unavaliable" : ConnectedDevice.ToString();
        public ContainerSetting SettingsObject { get => settingsObject; set => this.RaiseAndSetIfChanged(ref settingsObject, value); }
        private bool settingsDirty = true;
        private string savedFileName;

        private ContainerSetting settingsObject = DefaultSettingsObject.defaultSettingsObject;

        public bool SettingsDirty { get => settingsDirty; set => this.RaiseAndSetIfChanged(ref settingsDirty, value); }
        public string SavedFileName { get => savedFileName; set => this.RaiseAndSetIfChanged(ref savedFileName, value); }
        public ChildObservableCollection<IConfigurationSetting> Settings => SettingsObject.InnerSettings;

        public IConfigurationSetting SelectedSetting { get; set; }

        public string JSONPreview => JsonHelper.FormatJson(ToJSON.Serialize(SettingsObject));
        public string JSONHTMLPreview => JsonHelper.ToHTMLPreview(JsonHelper.SyntaxHighlightJson(JSONPreview));

        private void AddCustomSetting()
        {
            var newCustomSetting = new CustomSetting();
            if (SelectedSetting == null)
            {//Add top level setting
                Settings.Add(newCustomSetting);
            }
            else
            {
                var parent = SelectedSetting.Parent;
                if (SelectedSetting is CustomSetting)
                {
                    (SelectedSetting as IContainerSetting).InnerSettings.Add(newCustomSetting);
                }
                else if (parent is IContainerSetting)
                {
                    parent.InnerSettings.Add(newCustomSetting);
                }
                else
                {
                    if (SelectedSetting is IContainerSetting)
                    {
                        (SelectedSetting as IContainerSetting).InnerSettings.Add(newCustomSetting);
                    }
                }
            }
        }
    }
}
