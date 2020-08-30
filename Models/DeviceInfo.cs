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
        private readonly string scriptPath = "/etc/init.d/S100eth-can-router";
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
                    scp.Upload(str2, scriptPath);
                    ssh.RunCommand($"chmod +x {binaryPath} {scriptPath}");
                    str.Close();
                    str2.Close();
                    ssh.RunCommand("reboot");
                    Reconnect(parentWindow);
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
                //Now pretty ugly setting parsing into platform config
                var context = ((MainWindowViewModel)parentWindow.DataContext);
                var net = context.SettingsObject.InnerSettings["net"] as ContainerSetting;
                var ip = net.InnerSettings["ip"].Value as string;
                var mask = net.InnerSettings["mask"].Value as string;
                var gateway = net.InnerSettings["gateway"].Value as string;
                var channels = (context.SettingsObject.InnerSettings["channels"] as ContainerSetting);
                string bitrate0 = "250000";
                string bitrate1 = "250000";
                try
                {
                    foreach (ContainerSetting bitrateSetting in channels.InnerSettings)
                    {
                        if (bitrateSetting.InnerSettings["name"].Value as string == "can0")
                        {
                            bitrate0 = bitrateSetting.InnerSettings["bitrate"].Value.ToString();
                        }
                        else if (bitrateSetting.InnerSettings["name"].Value as string == "can1")
                        {
                            bitrate1 = bitrateSetting.InnerSettings["bitrate"].Value.ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    //No bitrate settings found
                }
                var str = new MemoryStream(Encoding.UTF8.GetBytes(context.JSONPreview));
                var str2 = new MemoryStream(Encoding.UTF8.GetBytes(//PEAK Configuration files
                    @$"WLAN_DRIVER_REGCODE=
BT_DRIVER=
IP_ADDRESS={ip}
IP_NETMASK={mask}
IP_GATEWAY={gateway}
IP_DNS1=
IP_DNS2=
CAN0_BITRATE={bitrate0}
CAN1_BITRATE={bitrate1}
WLAN_MODE=WLAN
WLAN_SECURITY=/mnt/user/wpa_supplicant.conf
"));
                Task.Run(() =>
                {
                    scp.Upload(str, "/mnt/user/default.json");
                    scp.Upload(str2, "/mnt/user/platform.config");
                    ssh.RunCommand("reboot");
                    str.Close();
                    str2.Close();
                    Reconnect(parentWindow);
                });

                IPToConnect = ip;
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

        private void Reconnect(Window parentWindow)
        {
            ssh.Disconnect();
            scp.Disconnect();
            Connected = false;
            Connect(parentWindow);//Reconnect after reboot
        }
    }
    public class BoolToConnectedConverter : FuncValueConverter<bool, string>
    {
        public BoolToConnectedConverter() : base((x => x ? "Connected 🚠" : "Disconnected 🔌"))
        {
        }
    }
}