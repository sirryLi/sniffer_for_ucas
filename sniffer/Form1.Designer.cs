namespace sniffer
{
    partial class Form1
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
            SuspendLayout();
            // 
            // deviceselectBox
            // 
            deviceselectBox.FormattingEnabled = true;
            deviceselectBox.Location = new Point(84, 90);
            deviceselectBox.Name = "deviceselectBox";
            deviceselectBox.Size = new Size(151, 28);
            deviceselectBox.TabIndex = 0;
            deviceselectBox.SelectedIndexChanged += deviceselectBox_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 706);
            Controls.Add(deviceselectBox);
            Name = "sniffer-lsy";
            Text = "sniffer-lsy";
            ResumeLayout(false);
        }

        #endregion

        private ComboBox deviceselectBox;
    }
}