using System.Windows.Forms;

namespace sniffer
{
    partial class NetworkSnifferForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            deviceselectBox = new ComboBox();
            startButton = new Button();
            stopButton = new Button();
            packetlistBox = new ListView();
            saveFileDialog = new SaveFileDialog();
            infoBox = new RichTextBox();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // deviceselectBox
            // 
            deviceselectBox.FormattingEnabled = true;
            deviceselectBox.Location = new Point(314, 49);
            deviceselectBox.Name = "deviceselectBox";
            deviceselectBox.Size = new Size(452, 28);
            deviceselectBox.TabIndex = 0;
            deviceselectBox.Text = "请选择抓包设备";
            deviceselectBox.SelectedIndexChanged += deviceselectBox_SelectedIndexChanged;
            // 
            // startButton
            // 
            startButton.Location = new Point(790, 48);
            startButton.Name = "startButton";
            startButton.Size = new Size(88, 28);
            startButton.TabIndex = 1;
            startButton.Text = "开始抓包";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(895, 50);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(84, 27);
            stopButton.TabIndex = 2;
            stopButton.Text = "停止抓包";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += stopButton_Click;
            // 
            // packetlistBox
            // 
            packetlistBox.FullRowSelect = true;
            packetlistBox.Location = new Point(314, 98);
            packetlistBox.Name = "packetlistBox";
            packetlistBox.Size = new Size(784, 321);
            packetlistBox.TabIndex = 3;
            packetlistBox.UseCompatibleStateImageBehavior = false;
            packetlistBox.View = View.Details;
            packetlistBox.SelectedIndexChanged += packetlistBox_SelectedIndexChanged;
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "PCAP Files (*.pcap)|*.pcap";
            saveFileDialog.Title = "Save Captured Packets as PCAP";
            saveFileDialog.FileOk += saveFileDialog_FileOk;
            // 
            // infoBox
            // 
            infoBox.Location = new Point(314, 439);
            infoBox.Name = "infoBox";
            infoBox.Size = new Size(784, 161);
            infoBox.TabIndex = 4;
            infoBox.Text = "";
            // 
            // protocolBox
            // 
            protocolBox.Location = new Point(94, 98);
            protocolBox.Name = "protocolBox";
            protocolBox.Size = new Size(125, 27);
            protocolBox.TabIndex = 5;
            // 
            // ipBox
            // 
            ipBox.Location = new Point(94, 48);
            ipBox.Name = "ipBox";
            ipBox.Size = new Size(125, 27);
            ipBox.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 52);
            label1.Name = "label1";
            label1.Size = new Size(39, 20);
            label1.TabIndex = 7;
            label1.Text = "协议";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 101);
            label2.Name = "label2";
            label2.Size = new Size(22, 20);
            label2.TabIndex = 8;
            label2.Text = "IP";
            // 
            // button1
            // 
            button1.Location = new Point(94, 150);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 9;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += filter_click;
            // 
            // NetworkSnifferForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 706);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ipBox);
            Controls.Add(protocolBox);
            Controls.Add(infoBox);
            Controls.Add(packetlistBox);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(deviceselectBox);
            Name = "NetworkSnifferForm";
            Text = "sniffer-lsy";
            Load += NetworkSnifferForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private ComboBox deviceselectBox;
        private Button startButton;
        private Button stopButton;
        private ListView packetlistBox;
        private SaveFileDialog saveFileDialog;
        private RichTextBox infoBox;
        private TextBox protocolBox;
        private TextBox ipBox;
        private Label label1;
        private Label label2;
        private Button button1;
    }
}