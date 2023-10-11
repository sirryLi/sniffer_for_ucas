using SharpPcap;
using PacketDotNet;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace sniffer
{
    public partial class Form1 : Form
    {
        private CaptureDeviceList devices;
        private ICaptureDevice selectedDevice;
        public Form1()
        {
            InitializeComponent();
        }
        private void NetworkSnifferForm_Load(object sender, EventArgs e)
        {
            devices = CaptureDeviceList.Instance;
            if (devices.Count == 0)
            {
                MessageBox.Show("Î´¼ì²âµ½ÍøÂçÊÊÅäÆ÷¡£");
                return;
            }

            foreach (var dev in devices)
            {
                deviceselectBox.Items.Add(dev.Description);
            }

            if (deviceselectBox.Items.Count > 0)
            {
                deviceselectBox.SelectedIndex = 0;
            }

        }

        private void deviceselectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDevice = devices[deviceselectBox.SelectedIndex];
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (selectedDevice == null)
            {
                MessageBox.Show("ÇëÑ¡ÔñÒ»¸öÍøÂçÊÊÅäÆ÷¡£");
                return;
            }

            selectedDevice.OnPacketArrival += (senderDevice, eArgs) =>
            {
                PacketHandler(eArgs.Packet);
            };

            selectedDevice.Open(DeviceMode.Promiscuous);
            selectedDevice.StartCapture();

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

        private void PacketHandler(Packet packet)
        {
            EthernetPacket ethernetPacket = packet.Extract<EthernetPacket>();

            if (ethernetPacket != null)
            {
                IpPacket ipPacket = ethernetPacket.PayloadPacket as IpPacket;

                if (ipPacket != null)
                {
                    string sourceIp = ipPacket.SourceAddress.ToString();
                    string destinationIp = ipPacket.DestinationAddress.ToString();

                    UpdateTextBox(sourceIp + " -> " + destinationIp + "\n");
                }
            }
        }

        private void UpdateTextBox(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => UpdateTextBox(text)));
            }
            else
            {
                packetTextBox.AppendText(text);
            }
        }


    }
}
