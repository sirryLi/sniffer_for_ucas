## 《网络攻防基础》Exp1A——嗅探器设计与实现

建议使用branch1分支下的代码
------

### NPS功能实现

针对NPS功能，本嗅探器主要进行了以下两点实现：

- 网卡的选择功能
- 在选取的网卡上进行混杂模式嗅探的功能

#### 1、网卡选择功能

`winpcap`库是`libpcap`库的Windows版本，利用`winpcap`库内置的`WinPcapDeviceList.Instance`接口，可以很轻易实现获取本电脑所有网络设备的功能。

~~~c#
 devices = CaptureDeviceList.Instance;
 if (devices.Count == 0)
 {
     MessageBox.Show("未检测到网络适配器。");
     return;
 }

 foreach (var dev in devices)
 {
     var str = dev.Description;
     deviceselectBox.Items.Add(str);
 }
~~~

当选择网卡时，将调用`deviceselectBox_SelectedIndexChanged`修改变量`selectedDevice`记录当前进行抓包的网卡：

~~~c#
private void deviceselectBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        selectedDevice = devices[deviceselectBox.SelectedIndex];
        startButton.Enabled = true;

    }
~~~

#### 2、嗅探功能

嗅探功能进行了以下基础功能实现：

- 创建数据结构保存抓到的所有数据包
- 将数据包按照到达顺序排序
- 为每个数据包创建其到达时间（距离开始抓包的时间）

嗅探功能的实现：

- 为`selectedDevice.OnPacketArrival`事件创建事件处理程序，当有新的数据包到达时，该程序被触发并进行程序内的响应行为：计算数据包到达事件，并将数据包到达顺序、到达时间以及数据包递交给`PacketHandler`函数。

`PacketHandler`函数将在下一部分中进行介绍，其主要作用是在图形化界面上显示数据包的部分信息，以及将数据包保存在对应数据结构中以便进行后续分析。

- 在开始抓包前，通过`selectedDevice.Open(DeviceModes.Promiscuous)`将网络设备设置为混杂模式是非常有必要的；

代码如下：

~~~c#
        private void startCapture()
        {
            int no = 1;
            var captureStartTime = DateTime.Now;
            selectedDevice.OnPacketArrival += (Device, e) =>
            {
                TimeSpan timearrival = DateTime.Now - captureStartTime;
                if(PacketHandler(no, timearrival, e.GetPacket()))
                {
                    no++;
                }        
            };
            try
            {
                selectedDevice.Open(DeviceModes.Promiscuous);
                selectedDevice.StartCapture();
            }
            catch (PcapException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
~~~

-----

### NPA功能实现

NPA功能进行了以下实现：

- 显示报文地址、协议等重要字段信息
- 点击报文显示其二进制以及详细信息
- 过滤器，可以分别根据ip地址和协议进行筛选
- 基于ip和端口的流分析能力

`PacketHandler`接收`RawCapture`类型数据，并进行相应分析。在这里，我根据是否是loopback数据包进行了分类处理：

- 对于一般数据包，可以直接调用` Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data)`来构造`EthernetPacket`类型数据包，使用内置的接口获取ip数据包进行分析；
- 对于loopback数据包，进行特殊处理：

```c#
 int copylength = rawCapture.Data.Length - 4;
 IPv4Packet ipPacket = new IPv4Packet(new ByteArraySegment(rawCapture.Data, 4, copylength));
```

- 对于 `int copylength= e.Packet.Data.Length - 4`的解释：

<img src="李思悦_202318018670045_ex1A_report.assets/image-20231018181638790.png" alt="image-20231018181638790" style="zoom:50%;"  align="center"/>

<img src="李思悦_202318018670045_ex1A_report.assets/image-20231018181905644.png" alt="image-20231018181905644" style="zoom:50%;" align="center" />

如图所示，上图为程序运行过程中的嗅探到的包，下图为wireshark对于loopback数据包的解析。可以看出loopback数据包与一般数据包具有差别：没有以太网报头，ip数据包从下标4开始；因此在构造ip数据包时将前4个字节丢弃，以实现对于loopback数据包的分析功能。

- 在获取ip数据包后，可以很容易通过接口获取其源地址、目的地址、传输层协议、源端口、目的端口、报文长度等信息，之后再通过端口号判断其应用层协议，并将上述信息记录至数据结构中。记录所使用的数据结构代码如下：

~~~c#
private class packet_details
{

    public int Index { get; set; }
    public string TimeString { get; set; }
    public string DestinationAddress { get; set; }
    public string SourceAddress { get; set; }
    public int Sp { get; set; }
    public int Dp { get; set; }
    public string Protocol { get; set; }
    public string Detail {  get; set; }
    public int Length { get; set; }
    public EthernetPacket EthPacket { get; set; }
    public IPv4Packet Looppacket { get; set; }
    public packet_details(int index, string timeString, string destinationAddress, string sourceAddress, int sp, int dp, string protocol,string detail, int length, EthernetPacket packet, IPv4Packet payload)
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
~~~

使用该数据结构，可以更简单地实现filter功能和流分析功能，这两个功能都是通过字符串比对实现的，因此不在此赘述，可以参见代码中的`FilterPackets`和`packetlistBox_SelectedIndexChanged`函数。

-----

## 功能截图展示

网卡选择

![image-20231025144019390](李思悦_202318018670045_ex1A_report.assets/image-20231025144019390.png)

抓包

![image-20231025144042722](李思悦_202318018670045_ex1A_report.assets/image-20231025144042722.png)

过滤器

![image-20231025144110643](李思悦_202318018670045_ex1A_report.assets/image-20231025144110643.png)

![image-20231025144121852](李思悦_202318018670045_ex1A_report.assets/image-20231025144121852.png)

流分析&数据包内容显示：

![image-20231025144242813](李思悦_202318018670045_ex1A_report.assets/image-20231025144242813.png)

