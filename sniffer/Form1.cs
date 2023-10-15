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
                        string protocol = ipPacket.Protocol.ToString();
                        int length = ipPacket.TotalLength;

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
                    EthernetPacket selectedPacket = captured[selectedIndex];

                    // 16����
                    DisplayHexData(selectedPacket);
                    // todo ��ϸ�ֶ�////////////////////

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
    }
}
