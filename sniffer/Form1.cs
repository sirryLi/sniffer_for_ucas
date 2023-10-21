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

namespace sniffer
{
    public partial class NetworkSnifferForm : Form
    {
        private WinPcapDeviceList devices;
        private ICaptureDevice selectedDevice;
        private List<packet_details> captured = new List<packet_details>();
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
                MessageBox.Show("δ��⵽������������");
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
                        name = "���ػ���";
                    }
                    else
                    {
                        name = "������";
                    }

                }

                var str = string.Format("��{0}��{1}", name, regex.Match(dev.Description).Groups[1].Value);
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
            //    MessageBox.Show("��ѡ��һ��������������");
            //    return;
            //}          
            if (captured.Count != 0)
            {
                while (true) { 
                    DialogResult result = MessageBox.Show(
                    "�Ƿ񱣴��Ѳ���İ�",
                    "ѡ��ѡ��",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                    );
                    if (result == DialogResult.Yes) { 
                        saveFileDialog.ShowDialog();
                        packetlistBox.Items.Clear();
                        break;
                    }else if (result == DialogResult.No)
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
                if (e.Packet.LinkLayerType != LinkLayers.Null)
                {
                    PacketHandler(no, timearrival, Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data));
                }
                else
                {
                    int length = e.Packet.Data.Length - 4;

                    PhysicalAddress sourceMac = PhysicalAddress.Parse("00-00-00-00-00-00");
                    PhysicalAddress destinationMac = PhysicalAddress.Parse("00-00-00-00-00-00");
                    EthernetPacket ethernetPacket = new EthernetPacket(destinationMac, sourceMac, EthernetPacketType.IpV4);
                    byte[] slice = new byte[length + ethernetPacket.Bytes.Length];
                    Array.Copy(ethernetPacket.Bytes, 0, slice, 0, ethernetPacket.Bytes.Length);
                    Array.Copy(e.Packet.Data, 4, slice, ethernetPacket.Bytes.Length, length);


                    PacketHandler(no, timearrival, Packet.ParsePacket(LinkLayers.Ethernet, slice));
                }
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
                    IpPacket ipPacket = ethernetPacket.PayloadPacket as IpPacket;

                    if (ipPacket != null)
                    {
                        string TimeString = $"{time.TotalSeconds:F3}";

                        string sourceIp = ipPacket.SourceAddress.ToString();
                        string destinationIp = ipPacket.DestinationAddress.ToString();
                        string protocol = ipPacket.Protocol.ToString();
                        int length = ipPacket.TotalLength;

                        captured.Add(new packet_details(no, TimeString, destinationIp, sourceIp, protocol, length, ethernetPacket));

                        //string data = ipPacket.PayloadData.ToString();
                        ListViewItem item = new ListViewItem(new[] { no.ToString(), TimeString, sourceIp,
                                                                    destinationIp, protocol,length.ToString() });

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
                    packet_details selectedPacket = captured[selectedIndex];

                    // 16����
                    DisplayHexData(selectedPacket.EthPacket, selectedPacket.Looppacket);
                    string src = selectedPacket.SourceAddress;
                    string dst = selectedPacket.DestinationAddress;
                    int sp = selectedPacket.Sp;

                }
            }
        }
        private void DisplayHexData(EthernetPacket packet)
        {
            // �����ݰ��ֽ�������ʮ�����Ƹ�ʽ��ʾ�� RichTextBox ��
            StringBuilder hexBuilder = new StringBuilder();

            foreach (byte b in packet.Bytes)
            {
                hexBuilder.Append(b.ToString("X2") + " ");
            }

            infoBox.Text = hexBuilder.ToString();
        }

        private void saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //string fileName = saveFileDialog.FileName;

            //todo 

        }

        private void filter_click(object sender, EventArgs e)
        {
            string pro = protocolBox.Text;
            string ip = ipBox.Text;

            // ִ�����ݰ�����
            FilterPackets(pro, ip);
        }
        private void FilterPackets(string pro, string ip)
        {
            List<packet_details> filteredPackets = new List<packet_details>();
            if (ip != "" && pro != "")
            {
                foreach (packet_details packet in captured)
                {
                    if ((packet.DestinationAddress == ip || packet.SourceAddress == ip) && packet.Protocol == pro)
                    {
                        filteredPackets.Add(packet);
                    }
                }
            }
            else if (ip != "" && pro == "")
            {
                foreach (packet_details packet in captured)
                {
                    // �ڴ˴�ʵ����Ĺ����߼����������Э�����ͻ�IP��ַ
                    // ����һ��ʾ�������߼�������Ը���ʵ�������޸�
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
                    // �ڴ˴�ʵ����Ĺ����߼����������Э�����ͻ�IP��ַ
                    // ����һ��ʾ�������߼�������Ը���ʵ�������޸�
                    if (packet.Protocol == pro)
                    {
                        filteredPackets.Add(packet);
                    }
                }
            }
            else
            {
                return;
            }

            // ���� ListView �е����ݰ��б�
            UpdateListViewWithFilteredPackets(filteredPackets);
        }

        private void UpdateListViewWithFilteredPackets(List<packet_details> packets)
        {
            // ��� ListView
            packetlistBox.Items.Clear();

            // ��ɸѡ������ݰ���ӵ� ListView

            foreach (packet_details packet in packets)
            {
                ListViewItem item = new ListViewItem(new[] { packet.Index.ToString(), packet.TimeString, packet.SourceAddress, packet.DestinationAddress,
                                                                    packet.Protocol,packet.Length.ToString()});
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
            public string Protocol { get; set; }
            public int Length { get; set; }
            public EthernetPacket Packet { get; set; }

            public packet_details(int index, string timeString, string destinationAddress, string sourceAddress, string protocol, int length, EthernetPacket packet)
            {
                Index = index;
                TimeString = timeString;
                DestinationAddress = destinationAddress;
                SourceAddress = sourceAddress;
                Protocol = protocol;
                Length = length;
                Packet = packet;

            }
        }

        private void reload(object sender, EventArgs e)
        {
            packetlistBox.Items.Clear();

            // ��ɸѡ������ݰ���ӵ� ListView

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

