using SharpPcap;
using PacketDotNet;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpPcap.WinPcap;
using SharpPcap.LibPcap;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text;
using System.ComponentModel.DataAnnotations;
using PacketDotNet.Ieee80211;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace sniffer
{
    public partial class NetworkSnifferForm : Form
    {
        private WinPcapDeviceList devices;
        private ICaptureDevice selectedDevice;
        private List<packet_details> captured = new List<packet_details>();
        private bool is_saved = false;
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
            //}          //未实现保存
            /*if (captured.Count != 0 && is_saved == false)
            {
                while (true)
                {
                    DialogResult result = MessageBox.Show(
                    "是否保存已捕获的包",
                    "选择选项",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                    );
                    if (result == DialogResult.Yes)
                    {
                        saveFileDialog.ShowDialog();
                        packetlistBox.Items.Clear();
                        break;
                    }
                    else if (result == DialogResult.No)
                    {
                        packetlistBox.Items.Clear();
                        break;
                    }
                    else
                    {
                        return;
                    }
                    ///cancel donothing
                }

            }*/
            packetlistBox.Items.Clear();
            captured.Clear();
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
                PacketHandler(no, timearrival, e.Packet);

                no++;
            };
            try
            {
                selectedDevice.Open(DeviceMode.Promiscuous);
                selectedDevice.StartCapture();
            }
            catch (PcapException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void PacketHandler(int no, TimeSpan time, RawCapture rawCapture)
        {
            if (rawCapture != null)
            {
                if (rawCapture.LinkLayerType != LinkLayers.Null)
                {
                    EthernetPacket ethernetPacket = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as EthernetPacket;
                    IpPacket ipPacket = ethernetPacket.PayloadPacket as IpPacket;

                    if (ipPacket != null)
                    {
                        string TimeString = $"{time.TotalSeconds:F3}";

                        string sourceIp = ipPacket.SourceAddress.ToString();
                        string destinationIp = ipPacket.DestinationAddress.ToString();
                        string protocol = ipPacket.Protocol.ToString();
                        int length = ipPacket.TotalLength;

                        int srcport;
                        int dstport;
                        if (protocol == "TCP")
                        {
                            TcpPacket Tcp = ipPacket.PayloadPacket as TcpPacket;
                            srcport = Tcp.SourcePort;
                            dstport = Tcp.DestinationPort;

                        }
                        else if (protocol == "UDP")
                        {
                            UdpPacket udpPacket = ipPacket.PayloadPacket as UdpPacket;
                            srcport = udpPacket.SourcePort;
                            dstport = udpPacket.DestinationPort;
                        }
                        else
                        {
                            srcport = 0; dstport = 0;
                        }
                        string detail = Details(srcport, dstport);
                        captured.Add(new packet_details(no, TimeString, destinationIp, sourceIp, srcport, dstport, protocol, detail, length, ethernetPacket, null));
                        string details;
                        //string data = ipPacket.PayloadData.ToString();
                        ListViewItem item = new ListViewItem(new[] { no.ToString(), TimeString, sourceIp,
                                                                    destinationIp, protocol,length.ToString(),detail });

                        UpdateTextBox(item);
                    }
                }
                else
                {

                    int copylength = rawCapture.Data.Length - 4;
                    IPv4Packet ipPacket = new IPv4Packet(new ByteArraySegment(rawCapture.Data, 4, copylength));
                    string TimeString = $"{time.TotalSeconds:F3}";

                    string sourceIp = ipPacket.SourceAddress.ToString();
                    string destinationIp = ipPacket.DestinationAddress.ToString();
                    string protocol = ipPacket.Protocol.ToString();
                    int length = ipPacket.TotalLength;

                    int srcport;
                    int dstport;
                    if (protocol == "TCP")
                    {
                        TcpPacket Tcp = ipPacket.PayloadPacket as TcpPacket;
                        srcport = Tcp.SourcePort;
                        dstport = Tcp.DestinationPort;

                    }
                    else if (protocol == "UDP")
                    {
                        UdpPacket udpPacket = ipPacket.PayloadPacket as UdpPacket;
                        srcport = udpPacket.SourcePort;
                        dstport = udpPacket.DestinationPort;
                    }
                    else
                    {
                        srcport = 0; dstport = 0;
                    }
                    string detail = Details(srcport, dstport);
                    captured.Add(new packet_details(no, TimeString, destinationIp, sourceIp, srcport, dstport, protocol, detail, length, null, ipPacket));


                    //string data = ipPacket.PayloadData.ToString();
                    ListViewItem item = new ListViewItem(new[] { no.ToString(), TimeString, sourceIp,
                                                                    destinationIp, protocol,length.ToString(),detail });

                    UpdateTextBox(item);
                }
            }
        }

        private string Details(int sp, int dp)
        {
            if (dp == 21 || sp == 21)
            {
                return "FTP";
            }
            else if (dp == 22 || sp == 22)
            {
                return "SSH";
            }
            else if (dp == 23 || sp == 23)
            {
                return "Telnet";
            }
            else if (dp == 25 || sp == 25)
            {
                return "SMTP";
            }
            else if (dp == 80 || sp == 80)
            {
                return "HTTP";
            }
            else if (dp == 443 || sp == 443)
            {
                return "HTTPS";
            }
            return "";
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
                    packet_details selectedPacket = captured[selectedIndex];

                    // 16进制
                    DisplayHexData(selectedPacket.EthPacket, selectedPacket.Looppacket);
                    string src = selectedPacket.SourceAddress;
                    string dst = selectedPacket.DestinationAddress;
                    int sp = selectedPacket.Sp;

                    foreach (ListViewItem item in packetlistBox.Items)
                    {
                        if ((item.SubItems[2].Text == src && item.SubItems[3].Text == dst) || (item.SubItems[3].Text == src && item.SubItems[2].Text == dst))
                        {
                            // 高亮显示具有相同地址的项目
                            item.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            // 恢复其他项目的背景颜色
                            item.BackColor = packetlistBox.BackColor;
                        }
                    }
                }

            }
        }
        private void DisplayHexData(EthernetPacket packet, IPv4Packet looppacket)
        {
            // 将数据包字节数组以十六进制格式显示在 RichTextBox 中
            StringBuilder hexBuilder = new StringBuilder();
            if (packet != null)
            {
                foreach (byte b in packet.Bytes)
                {
                    hexBuilder.Append(b.ToString("X2") + " ");
                }
                infoBox2.Text = packet.ToString();
            }
            else
            {
                foreach (byte b in looppacket.Bytes)
                {

                    hexBuilder.Append(b.ToString("X2") + " ");
                }
                infoBox2.Text = looppacket.ToString();
            }

            infoBox.Text = hexBuilder.ToString();

        }

        private void saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*//string fileName = saveFileDialog.FileName;

            //todo */

        }

        private void filter_click(object sender, EventArgs e)
        {
            string pro = protocolBox.Text;
            string ip = ipBox.Text;

            // 执行数据包过滤
            FilterPackets(pro, ip);
        }
        private void FilterPackets(string pro, string ip)
        {
            List<packet_details> filteredPackets = new List<packet_details>();
            if (ip != "" && pro != "")
            {
                foreach (packet_details packet in captured)
                {
                    if ((packet.DestinationAddress == ip || packet.SourceAddress == ip) && (packet.Protocol == pro || packet.Detail == pro))
                    {
                        filteredPackets.Add(packet);
                    }
                }
            }
            else if (ip != "" && pro == "")
            {
                foreach (packet_details packet in captured)
                {

                    if (packet.DestinationAddress == ip || packet.SourceAddress == ip)
                    {
                        filteredPackets.Add(packet);
                    }
                }
            }
            else if (ip == "" && pro != "")
            {
                foreach (packet_details packet in captured)
                {

                    if (packet.Protocol == pro || packet.Detail == pro)
                    {
                        filteredPackets.Add(packet);
                    }
                }
            }
            else
            {
                return;
            }

            // 更新 ListView 中的数据包列表
            UpdateListViewWithFilteredPackets(filteredPackets);
        }

        private void UpdateListViewWithFilteredPackets(List<packet_details> packets)
        {
            // 清空 ListView
            packetlistBox.Items.Clear();

            // 将筛选后的数据包添加到 ListView

            foreach (packet_details packet in packets)
            {
                ListViewItem item = new ListViewItem(new[] { packet.Index.ToString(), packet.TimeString, packet.SourceAddress, packet.DestinationAddress,
                                                                    packet.Protocol,packet.Length.ToString(),packet.Detail});
                //ListViewItem item = new ListViewItem(new[] { no.ToString(), TimeString, sourceIp,
                //                                                    destinationIp, protocol,length.ToString() });
                packetlistBox.Items.Add(item);
            }
        }
        private class packet_details
        {

            public int Index { get; set; }
            public string TimeString { get; set; }
            public string DestinationAddress { get; set; }
            public string SourceAddress { get; set; }
            public int Sp { get; set; }
            public int Dp { get; set; }
            public string Protocol { get; set; }
            public string Detail { get; set; }
            public int Length { get; set; }
            public EthernetPacket EthPacket { get; set; }
            public IPv4Packet Looppacket { get; set; }

            public packet_details(int index, string timeString, string destinationAddress, string sourceAddress, int sp, int dp, string protocol, string detail, int length, EthernetPacket packet, IPv4Packet payload)
            {
                Index = index;
                TimeString = timeString;
                DestinationAddress = destinationAddress;
                SourceAddress = sourceAddress;
                Sp = sp; Dp = dp;
                Protocol = protocol;
                Detail = detail;
                Length = length;
                EthPacket = packet;
                Looppacket = payload;
            }
        }

        private void reload(object sender, EventArgs e)
        {
            packetlistBox.Items.Clear();

            // 将筛选后的数据包添加到 ListView

            foreach (packet_details packet in captured)
            {
                ListViewItem item = new ListViewItem(new[] { packet.Index.ToString(), packet.TimeString, packet.SourceAddress, packet.DestinationAddress,
                                                                    packet.Protocol,packet.Length.ToString()});
                //ListViewItem item = new ListViewItem(new[] { no.ToString(), TimeString, sourceIp,
                //                                                    destinationIp, protocol,length.ToString() });
                packetlistBox.Items.Add(item);
            }
        }
    }
}

