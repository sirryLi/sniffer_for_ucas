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
            packetlistBox = new ListBox();
            SuspendLayout();
            // 
            // deviceselectBox
            // 
            deviceselectBox.FormattingEnabled = true;
            deviceselectBox.Location = new Point(52, 50);
            deviceselectBox.Name = "deviceselectBox";
            deviceselectBox.Size = new Size(232, 28);
            deviceselectBox.TabIndex = 0;
            deviceselectBox.SelectedIndexChanged += deviceselectBox_SelectedIndexChanged;
            // 
            // startButton
            // 
            startButton.Location = new Point(314, 50);
            startButton.Name = "startButton";
            startButton.Size = new Size(88, 28);
            startButton.TabIndex = 1;
            startButton.Text = "开始抓包";
            startButton.UseVisualStyleBackColor = false;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(430, 50);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(84, 27);
            stopButton.TabIndex = 2;
            stopButton.Text = "停止抓包";
            stopButton.UseVisualStyleBackColor = true;
            // 
            // packetlistBox
            // 
            packetlistBox.FormattingEnabled = true;
            packetlistBox.ItemHeight = 20;
            packetlistBox.Location = new Point(314, 101);
            packetlistBox.Name = "packetlistBox";
            packetlistBox.Size = new Size(665, 344);
            packetlistBox.TabIndex = 3;
            // 
            // NetworkSnifferForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 706);
            Controls.Add(packetlistBox);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(deviceselectBox);
            ImeMode = ImeMode.NoControl;
            Name = "NetworkSnifferForm";
            Text = "sniffer-lsy";
            Load += NetworkSnifferForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private ComboBox deviceselectBox;
        private Button startButton;
        private Button stopButton;
        private ListBox packetlistBox;
    }
}