using SharpPcap;
using PacketDotNet;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpPcap.WinPcap;
using System.Text.RegularExpressions;

namespace sniffer
{
    public partial class NetworkSnifferForm : Form
    {
        private WinPcapDeviceList devices;

        private ICaptureDevice selectedDevice;
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
                MessageBox.Show("Î´¼ì²âµ½ÍøÂçÊÊÅäÆ÷¡£");
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
                        name = "±¾µØ»·»Ø";
                    }
                    else {
                        name = "ÎÞÃû³Æ";
                    }
                    
                }
                
                var str = string.Format("¡¾{0}¡¿{1}", name,regex.Match(dev.Description).Groups[1].Value);
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
            //    MessageBox.Show("ÇëÑ¡ÔñÒ»¸öÍøÂçÊÊÅäÆ÷¡£");
            //    return;
            //}          
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
            selectedDevice.OnPacketArrival += (Device, e) =>
            {
                PacketHandler(Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data));
            };
            selectedDevice.Open(DeviceMode.Promiscuous);
            selectedDevice.StartCapture();
        }
        private void PacketHandler(Packet packet)
        {
            /// EthernetPacket ethernetPacket;
            //ethernetPacket = packet.Extract(ethernetPacket);

            if (packet != null)
            {
                IpPacket ipPacket = packet.PayloadPacket as IpPacket;

                if (ipPacket != null)
                {
                    string sourceIp = ipPacket.SourceAddress.ToString();
                    string destinationIp = ipPacket.DestinationAddress.ToString();
                    string protocol = ipPacket.Protocol.ToString();

                    ListViewItem item = new ListViewItem(new[] { sourceIp, destinationIp, protocol });

                    UpdateTextBox(item);
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

    }
}
