using Avalonia.Controls;
using Avalonia.Data.Converters;
using EthCanConfig.ViewModels;
using MessageBox.Avalonia.DTO;
using ReactiveUI;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EthCanConfig.Models
{
    public class DeviceInfo : ReactiveObject
    {
        private readonly string binaryPath = "/mnt/user/eth-can-router";
        private readonly string scriptPath = "/etc/init.d/08eth-can-router.sh";
        public bool Connected { get => connected; set => connected = value; }
        public string IPToConnect { get => iPToConnect; set => iPToConnect = value; }
        public string Password { get; set; }
        public string Username { get; set; }

        public EventHandler<ScpUploadEventArgs> Uploading;
        SshClient ssh;
        ScpClient scp;
        private bool connected = false;
        private string iPToConnect = string.Empty;



        public void ShowInfo()
        {
            var msg = new StringBuilder("Not connected to any device.\n");
            try
            {
                var c = ssh.RunCommand("cat /proc/version");
                msg.Clear();
                msg.Append("Linux version: ");
                msg.Append(c.Result);
                msg.Append("Router software version: ");
                msg.Append(ssh.RunCommand($"{binaryPath} -v").Result);//Get system and installed app binary version
            }
            catch (Exception e)
            {
                if (ssh == null || scp == null || !ssh.IsConnected || !scp.IsConnected)
                {
                    msg.Append("Connect to a device first.\n");
                }
                msg.Append(e.Message);
                Connected = false;
            }
            var m = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams()
            {
                ContentTitle = "Connected Device Information",
                ContentMessage = msg.ToString()
            });
            m.Show();
        }

        public void Connect(Window parentWindow)
        {
            try
            {
                ssh = new SshClient(IPToConnect, Username, Password);
                scp = new ScpClient(IPToConnect, Password, Password);
                scp.Uploading += Uploading;
                scp.Connect();
                ssh.Connect();

                Connected = true;
            }
            catch (Exception e)
            {
                Connected = false;
                var m = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams()
                {
                    ContentTitle = "Error Connecting",
                    ContentMessage = e.Message
                });
                m.ShowDialog(parentWindow);
            }
        }

        public void UploadSoftware(Window parentWindow)
        {
            var msg = new StringBuilder();
            if (ssh == null || scp == null || !ssh.IsConnected || !scp.IsConnected)
            {
                msg.Append("Connect to a device first.\n");
            }
            try
            {
                PrepareRemoteDirectories();
                var str = new MemoryStream(Properties.Resources.eth_can_router);
                var str2 = new MemoryStream(Properties.Resources.startup);
                Task.Run(() =>
                {
                    scp.Upload(str, binaryPath);
                    ssh.RunCommand($"chmod +x {binaryPath}");
                    scp.Upload(str2, scriptPath);
                    ssh.RunCommand(scriptPath + " start");
                    str.Close();
                    str2.Close();
                }
                );
            }
            catch (Exception e)
            {
                Connected = false;
                msg.Append(e.Message);
                var m = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams()
                {
                    ContentTitle = "Error Uploading",
                    ContentMessage = msg.ToString()
                });
                m.ShowDialog(parentWindow);
            }
        }

        private void PrepareRemoteDirectories()
        {
            ssh.RunCommand($"rm -rf {binaryPath} {scriptPath}");//Remove because they could be directories or something...
            ssh.RunCommand($"mkdir -p `dirname {binaryPath}` `dirname {scriptPath}`");//Create all needed directories
        }

        public void UploadSettings(Window parentWindow)
        {
            var msg = new StringBuilder();
            if (ssh == null || scp == null || !ssh.IsConnected || !scp.IsConnected)
            {
                msg.Append("Connect to a device first.\n");
            }
            try
            {
                PrepareRemoteDirectories();
                var str = new MemoryStream(Encoding.UTF8.GetBytes(((MainWindowViewModel)parentWindow.DataContext).JSONPreview));
                Task.Run(() =>
                {
                    scp.Upload(str, "/mnt/user/default.json");
                    str.Close();
                });
            }
            catch (Exception e)
            {
                Connected = false;
                msg.Append(e.Message);
                var m = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams()
                {
                    ContentTitle = "Error Uploading",
                    ContentMessage = msg.ToString()
                });
                m.ShowDialog(parentWindow);
            }
        }
    }
    public class BoolToConnectedConverter : FuncValueConverter<bool, string>
    {
        public BoolToConnectedConverter() : base((x => x ? "Connected 🚠" : "Disconnected 🔌"))
        {
        }
    }
}