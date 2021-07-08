namespace NissanCAN
{
    partial class CANBUSReader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CANBUSReader));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbLog = new DarkUI.Controls.DarkTextBox();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.tbConnectCAN = new DarkUI.Controls.DarkButton();
            this.btnDownloadSpecial = new DarkUI.Controls.DarkButton();
            this.darkButton4 = new DarkUI.Controls.DarkButton();
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.darkLabel8 = new DarkUI.Controls.DarkLabel();
            this.cbProcessorSize = new DarkUI.Controls.DarkComboBox();
            this.darkButton2 = new DarkUI.Controls.DarkButton();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.darkLabel5 = new DarkUI.Controls.DarkLabel();
            this.dlSpeed = new DarkUI.Controls.DarkLabel();
            this.tbVinNumber = new DarkUI.Controls.DarkTextBox();
            this.tbCalibrationid = new DarkUI.Controls.DarkTextBox();
            this.tbAddress = new DarkUI.Controls.DarkTextBox();
            this.tbSize = new DarkUI.Controls.DarkTextBox();
            this.darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            this.tsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblTimeString = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnFlashRom = new DarkUI.Controls.DarkButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.darkGroupBox1.SuspendLayout();
            this.darkStatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(11, 337);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(204, 169);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 52;
            this.pictureBox1.TabStop = false;
            // 
            // tbLog
            // 
            this.tbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbLog.Location = new System.Drawing.Point(218, 90);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(461, 416);
            this.tbLog.TabIndex = 55;
            this.tbLog.Text = "Nii-saan Canbus Utility\r\nDevelopment build for Pytrex\r\n";
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(6, 19);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(192, 23);
            this.darkButton1.TabIndex = 47;
            this.darkButton1.Text = "Scan for J2534 Devices";
            this.darkButton1.Click += new System.EventHandler(this.CmdDetectDevices_Click);
            // 
            // tbConnectCAN
            // 
            this.tbConnectCAN.Location = new System.Drawing.Point(6, 48);
            this.tbConnectCAN.Name = "tbConnectCAN";
            this.tbConnectCAN.Padding = new System.Windows.Forms.Padding(5);
            this.tbConnectCAN.Size = new System.Drawing.Size(192, 23);
            this.tbConnectCAN.TabIndex = 48;
            this.tbConnectCAN.Text = "Initalize Adapter";
            this.tbConnectCAN.Click += new System.EventHandler(this.TbConnectCAN_Click);
            // 
            // btnDownloadSpecial
            // 
            this.btnDownloadSpecial.Enabled = false;
            this.btnDownloadSpecial.Location = new System.Drawing.Point(6, 126);
            this.btnDownloadSpecial.Name = "btnDownloadSpecial";
            this.btnDownloadSpecial.Padding = new System.Windows.Forms.Padding(5);
            this.btnDownloadSpecial.Size = new System.Drawing.Size(192, 23);
            this.btnDownloadSpecial.TabIndex = 49;
            this.btnDownloadSpecial.Text = "Download Nissan Rom";
            this.btnDownloadSpecial.Click += new System.EventHandler(this.btnDownloadSpecial_Click);
            // 
            // darkButton4
            // 
            this.darkButton4.Enabled = false;
            this.darkButton4.Location = new System.Drawing.Point(6, 184);
            this.darkButton4.Name = "darkButton4";
            this.darkButton4.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton4.Size = new System.Drawing.Size(192, 23);
            this.darkButton4.TabIndex = 50;
            this.darkButton4.Text = "Read Ram Address";
            this.darkButton4.Click += new System.EventHandler(this.btnCheckAlgo_Click);
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.darkGroupBox1.Controls.Add(this.btnFlashRom);
            this.darkGroupBox1.Controls.Add(this.darkLabel8);
            this.darkGroupBox1.Controls.Add(this.cbProcessorSize);
            this.darkGroupBox1.Controls.Add(this.darkButton2);
            this.darkGroupBox1.Controls.Add(this.darkButton4);
            this.darkGroupBox1.Controls.Add(this.darkButton1);
            this.darkGroupBox1.Controls.Add(this.btnDownloadSpecial);
            this.darkGroupBox1.Controls.Add(this.tbConnectCAN);
            this.darkGroupBox1.Location = new System.Drawing.Point(11, 90);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(204, 241);
            this.darkGroupBox1.TabIndex = 56;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "J2534 Controls";
            // 
            // darkLabel8
            // 
            this.darkLabel8.AutoSize = true;
            this.darkLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel8.Location = new System.Drawing.Point(28, 79);
            this.darkLabel8.Name = "darkLabel8";
            this.darkLabel8.Size = new System.Drawing.Size(142, 13);
            this.darkLabel8.TabIndex = 53;
            this.darkLabel8.Text = "Read Size(Select Processor)";
            // 
            // cbProcessorSize
            // 
            this.cbProcessorSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbProcessorSize.FormattingEnabled = true;
            this.cbProcessorSize.Items.AddRange(new object[] {
            "1.25MB (SH75231)",
            "1.5MB (SH7059)",
            "1MB (SH7058)",
            "2MB (SH72533)",
            "512KB (SH7055)"});
            this.cbProcessorSize.Location = new System.Drawing.Point(6, 99);
            this.cbProcessorSize.Name = "cbProcessorSize";
            this.cbProcessorSize.Size = new System.Drawing.Size(192, 21);
            this.cbProcessorSize.TabIndex = 52;
            // 
            // darkButton2
            // 
            this.darkButton2.Enabled = false;
            this.darkButton2.Location = new System.Drawing.Point(6, 213);
            this.darkButton2.Name = "darkButton2";
            this.darkButton2.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton2.Size = new System.Drawing.Size(192, 23);
            this.darkButton2.TabIndex = 51;
            this.darkButton2.Text = "Sniff All Traffic";
            this.darkButton2.Click += new System.EventHandler(this.darkButton2_Click);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(8, 13);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(62, 13);
            this.darkLabel1.TabIndex = 57;
            this.darkLabel1.Text = "Vin Number";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(8, 41);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(70, 13);
            this.darkLabel2.TabIndex = 58;
            this.darkLabel2.Text = "Calibration ID";
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(95, 67);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(202, 13);
            this.darkLabel3.TabIndex = 59;
            this.darkLabel3.Text = "Read address (in hex with preceeding 0x)";
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(406, 67);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(116, 13);
            this.darkLabel4.TabIndex = 60;
            this.darkLabel4.Text = "Read Size (Also in hex)";
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(6, 509);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(358, 25);
            this.darkLabel5.TabIndex = 61;
            this.darkLabel5.Text = "Copyright 2021 Nii-Saan@romraider";
            // 
            // dlSpeed
            // 
            this.dlSpeed.AutoSize = true;
            this.dlSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dlSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dlSpeed.Location = new System.Drawing.Point(363, 514);
            this.dlSpeed.Name = "dlSpeed";
            this.dlSpeed.Size = new System.Drawing.Size(0, 20);
            this.dlSpeed.TabIndex = 51;
            // 
            // tbVinNumber
            // 
            this.tbVinNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVinNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVinNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVinNumber.Location = new System.Drawing.Point(83, 11);
            this.tbVinNumber.Name = "tbVinNumber";
            this.tbVinNumber.ReadOnly = true;
            this.tbVinNumber.Size = new System.Drawing.Size(596, 20);
            this.tbVinNumber.TabIndex = 63;
            // 
            // tbCalibrationid
            // 
            this.tbCalibrationid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCalibrationid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCalibrationid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCalibrationid.Location = new System.Drawing.Point(83, 39);
            this.tbCalibrationid.Name = "tbCalibrationid";
            this.tbCalibrationid.ReadOnly = true;
            this.tbCalibrationid.Size = new System.Drawing.Size(596, 20);
            this.tbCalibrationid.TabIndex = 64;
            // 
            // tbAddress
            // 
            this.tbAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbAddress.Location = new System.Drawing.Point(300, 64);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(100, 20);
            this.tbAddress.TabIndex = 65;
            // 
            // tbSize
            // 
            this.tbSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbSize.Location = new System.Drawing.Point(528, 65);
            this.tbSize.Name = "tbSize";
            this.tbSize.Size = new System.Drawing.Size(100, 20);
            this.tbSize.TabIndex = 66;
            // 
            // darkStatusStrip1
            // 
            this.darkStatusStrip1.AutoSize = false;
            this.darkStatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkStatusStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkStatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabel,
            this.progressBar,
            this.lblTimeString});
            this.darkStatusStrip1.Location = new System.Drawing.Point(0, 541);
            this.darkStatusStrip1.Name = "darkStatusStrip1";
            this.darkStatusStrip1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.darkStatusStrip1.Size = new System.Drawing.Size(694, 30);
            this.darkStatusStrip1.SizingGrip = false;
            this.darkStatusStrip1.TabIndex = 67;
            this.darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // tsLabel
            // 
            this.tsLabel.Name = "tsLabel";
            this.tsLabel.Size = new System.Drawing.Size(39, 17);
            this.tsLabel.Text = "Status";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(500, 16);
            // 
            // lblTimeString
            // 
            this.lblTimeString.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblTimeString.ForeColor = System.Drawing.Color.Lime;
            this.lblTimeString.Name = "lblTimeString";
            this.lblTimeString.Size = new System.Drawing.Size(153, 17);
            this.lblTimeString.Spring = true;
            // 
            // btnFlashRom
            // 
            this.btnFlashRom.Enabled = false;
            this.btnFlashRom.Location = new System.Drawing.Point(6, 155);
            this.btnFlashRom.Name = "btnFlashRom";
            this.btnFlashRom.Padding = new System.Windows.Forms.Padding(5);
            this.btnFlashRom.Size = new System.Drawing.Size(192, 23);
            this.btnFlashRom.TabIndex = 54;
            this.btnFlashRom.Text = "Flash Rom";
            this.btnFlashRom.Click += new System.EventHandler(this.btnFlashRom_Click);
            // 
            // CANBUSReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 571);
            this.Controls.Add(this.darkStatusStrip1);
            this.Controls.Add(this.tbSize);
            this.Controls.Add(this.tbAddress);
            this.Controls.Add(this.tbCalibrationid);
            this.Controls.Add(this.tbVinNumber);
            this.Controls.Add(this.dlSpeed);
            this.Controls.Add(this.darkLabel5);
            this.Controls.Add(this.darkLabel4);
            this.Controls.Add(this.darkLabel3);
            this.Controls.Add(this.darkLabel2);
            this.Controls.Add(this.darkLabel1);
            this.Controls.Add(this.darkGroupBox1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(710, 573);
            this.Name = "CANBUSReader";
            this.Text = "Nii-saan CANBUS Utility";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.darkStatusStrip1.ResumeLayout(false);
            this.darkStatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private DarkUI.Controls.DarkButton darkButton4;
        private DarkUI.Controls.DarkButton btnDownloadSpecial;
        private DarkUI.Controls.DarkButton tbConnectCAN;
        private DarkUI.Controls.DarkButton darkButton1;
        private DarkUI.Controls.DarkTextBox tbLog;
        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private DarkUI.Controls.DarkLabel darkLabel5;
        private DarkUI.Controls.DarkLabel dlSpeed;
        private DarkUI.Controls.DarkTextBox tbVinNumber;
        private DarkUI.Controls.DarkTextBox tbCalibrationid;
        private DarkUI.Controls.DarkTextBox tbAddress;
        private DarkUI.Controls.DarkTextBox tbSize;
        private DarkUI.Controls.DarkButton darkButton2;
        private DarkUI.Controls.DarkComboBox cbProcessorSize;
        private DarkUI.Controls.DarkLabel darkLabel8;
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsLabel;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel lblTimeString;
        private DarkUI.Controls.DarkButton btnFlashRom;
    }
}

