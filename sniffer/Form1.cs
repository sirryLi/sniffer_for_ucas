using SharpPcap;
using PacketDotNet;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpPcap.WinPcap;
using SharpPcap.LibPcap;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace sniffer
{
    public partial class NetworkSnifferForm : Form
    {
        private WinPcapDeviceList devices;
        private ICaptureDevice selectedDevice;
        private List<EthernetPacket> captured = new List<EthernetPacket>();
        public NetworkSnifferForm()
        {
            InitializeComponent();
        }
        private void NetworkSnifferForm_Load(object sender, EventArgs e)
        {
            init_info();
            devices = WinPcapDeviceList.Instance;
            if (devices.Count == 0)
            {
                MessageBox.Show("未检测到网络适配器。");
                return;
            }

            foreach (var dev in devices)
            {
                Regex regex = new Regex("'([^']*)'");
                string name;
                if ((name = dev.Interface.FriendlyName) == null)
                {
                    if (dev.Description.Contains("loopback"))
                    {
                        name = "本地环回";
                    }
                    else
                    {
                        name = "无名称";
                    }

                }

                var str = string.Format("【{0}】{1}", name, regex.Match(dev.Description).Groups[1].Value);
                deviceselectBox.Items.Add(str);
            }
        }
        private void init_info()
        {

            startButton.Enabled = false;
            stopButton.Enabled = false;
        }

        private void deviceselectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDevice = devices[deviceselectBox.SelectedIndex];
            startButton.Enabled = true;

        }

        private void startButton_Click(object sender, EventArgs e)
        {

            //if (selectedDevice == null)
            //{
            //    MessageBox.Show("请选择一个网络适配器。");
            //    return;
            //}          
            if (captured.Count != 0)
            {
                saveFileDialog.ShowDialog();
                packetlistBox.Items.Clear();
            }
            startCapture();

            startButton.Enabled = false;
            stopButton.Enabled = true;
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            selectedDevice.StopCapture();
            selectedDevice.Close();

            startButton.Enabled = true;
            stopButton.Enabled = false;
        }


        private void startCapture()
        {
            int no = 1;
            var captureStartTime = DateTime.Now;
            selectedDevice.OnPacketArrival += (Device, e) =>
            {
                TimeSpan timearrival = DateTime.Now - captureStartTime;
                PacketHandler(no, timearrival, Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data));
                no++;
            };
            selectedDevice.Open(DeviceMode.Promiscuous);
            selectedDevice.StartCapture();
        }
        private void PacketHandler(int no, TimeSpan time, Packet packet)
        {
            if (packet != null)
            {
                if (packet is EthernetPacket ethernetPacket)
                {
                    captured.Add(ethernetPacket);
                    IpPacket ipPacket = ethernetPacket.PayloadPacket as IpPacket;

                    if (ipPacket != null)
                    {
                        string TimeString = $"{time.TotalSeconds:F3}";

                        string sourceIp = ipPacket.SourceAddress.ToString();
                        string destinationIp = ipPacket.DestinationAddress.ToString();
                        string ipversion = ipPacket.Version.ToString();
                        string protocol = ipPacket.Protocol.ToString();

                        //string data = ipPacket.PayloadData.ToString();
                        ListViewItem item = new ListViewItem(new[] { no.ToString(), TimeString, sourceIp, destinationIp, ipversion, protocol });

                        UpdateTextBox(item);
                    }
                }
            }

        }

        private void UpdateTextBox(ListViewItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => UpdateTextBox(item)));
            }
            else
            {
                packetlistBox.Items.Add(item);
            }
        }

        private void packetlistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packetlistBox.SelectedItems.Count > 0)
            {
                int selectedIndex = packetlistBox.SelectedItems[0].Index;
                if (selectedIndex >= 0 && selectedIndex < captured.Count)
                {
                    // 获取选中项对应的包的详细信息
                    EthernetPacket selectedPacket = captured[selectedIndex];

                    // 在文本框中显示包的详细信息
                    infoBox.Text = selectedPacket.Bytes;
                }
            }
        }

        private void saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //string fileName = saveFileDialog.FileName;

            // 创建一个 PCAP 写入器
            //CaptureFileWriterDevice pcapWriter = new CaptureFileWriterDevice(fileName);


            //
            //pcapWriter.Write(captured);


            // 关闭 PCAP 文件
            // pcapWriter.Close();

            //MessageBox.Show("数据包已保存为 PCAP 文件：" + fileName, "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
    }
}
