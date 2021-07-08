#region Copyright (c) 2021, Nii-Saan @ Romraider
/* 
 * Copyright (c) 2021, Nii-Saan @ Romraider
 * 
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the organization nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using SAE.J2534;
using System.Globalization;
using DarkUI.Collections;
using DarkUI.Config;
using DarkUI.Controls;
using DarkUI.Docking;
using DarkUI.Forms;
using DarkUI.Renderers;


namespace NissanCAN
{
    public partial class CANBUSReader : DarkForm
    {
        public static string DllFileName = string.Empty;
        public static string textLog = "";
        delegate void SetTextCallback(string text);
        public CANBUSReader()
        {
            InitializeComponent();
            cbProcessorSize.SelectedIndex = 2;
        }

        private void SetText(string text)
        {
            if (tbLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                tbLog.Text = text;
            }
        }

        public static byte[] CalibrationIdResponse = { 0x00, 0x00, 0x07, 0xE8, 0x61, 0x10 };
        public static byte[] WakeupResponse = { 0x07, 0xE8, 0x50 };
        public static byte[] PrepareReadResponse = { 0x07, 0xE8, 0x61 };
        public static byte[] VinNumberResponse = { 0x07, 0xE8, 0x61, 0x81 };
        public static byte[] HardwareIDResponse = { 0x07, 0xE8, 0x61, 0xFE };
        public static byte[] FlashEraseResponse = { 0x00, 0x00, 0x07, 0xE8, 0x71, 0x01, 0xFF, 0x00 };
        public static byte[] positiveReadResponse = { 0x00, 0x00, 0x07, 0xE8, 0x63 };
        public static byte[] SeedResponse = { 0x07, 0xE8, 0x67, 0x81 };
        public static byte[] KeyResponse = { 0x07, 0xE8, 0x67, 0x82 };


        public byte[] flashMemory = new byte[1];
        public uint blockSize = 0x37;
        public uint readsize = 0xFFFFF;
        public static byte[] writeFile = new byte[0xF8000];
        string filePath = string.Empty;



        private void CmdDetectDevices_Click(object sender, EventArgs e)
        {
            foreach (APIInfo device in APIFactory.GetAPIList())
            {
                tbLog.Text += device.Name + ", " + device.Name + "\r\n";
                tbLog.Text += "Filename:" + device.Filename + "\r\n";
                tbLog.Text += device.Details + "\r\n\r\n";
            }
        }

        private void TbConnectCAN_Click(object sender, EventArgs e)
        {
            frmSelect2534 sd = new frmSelect2534();
            cbProcessorSize.SelectedIndex = 2;

            DialogResult dialogresult = sd.ShowDialog();

            if (dialogresult == DialogResult.OK)
            {
                DllFileName = sd.apiInfo.Filename;
            }
            else
            {
                tbLog.Text = "Couldn't open device selection form";
                return;
            }
            sd.Dispose();
            try
            {


                using (API API = APIFactory.GetAPI(DllFileName))
                using (Device Device = API.GetDevice())
                using (Channel Channel = Device.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.NONE))
                {
                    SConfig[] configList = new SConfig[1] { new SConfig(Parameter.LOOP_BACK, 0) };//, new SConfig(Parameter.DATA_RATE, 500000) };
                    Channel.SetConfig(configList);

                    Channel.StartMsgFilter(new MessageFilter(UserFilterType.STANDARDISO15765, new byte[] { 0x00, 0x00, 0x07, 0xE0 }));


                    SAE.J2534.Message wakeup = new SAE.J2534.Message(
                    new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x10, 0xC0 },
                    TxFlag.ISO15765_FRAME_PAD
                    );
                    SAE.J2534.Message calIdMsg = new SAE.J2534.Message(
                    new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x21, 0x10 },
                    TxFlag.ISO15765_FRAME_PAD
                    );
                    SAE.J2534.Message vinMsg = new SAE.J2534.Message(
                    new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x21, 0x81 },
                    TxFlag.ISO15765_FRAME_PAD
                    );



                    //Setup some loop stuff. 
                    int i = 0;
                    int details = 0;

                    //Send out our first message to find the vin number
                    Channel.SendMessage(wakeup);

                    while (i < 5)
                    {
                        GetMessageResults Response = Channel.GetMessage();
                        if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                        {
                            foreach (SAE.J2534.Message msg in Response.Messages)
                            {

                                tbLog.AppendText("Wakeup Message: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine);
                            }
                        }
                        i++;
                    }

                    i = 0;

                    Channel.SendMessage(vinMsg);
                    ////Check for Vin response message
                    while (i < 8)
                    {
                        GetMessageResults Response = Channel.GetMessage();
                        if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                        {
                            foreach (SAE.J2534.Message msg in Response.Messages)
                            {
                                if ((msg.RxStatus & RxFlag.START_OF_MESSAGE) != 0)
                                {
                                    GetMessageResults Response2 = Channel.GetMessage();
                                    tbLog.AppendText("Got Start Of Message: " + Environment.NewLine + ByteArrayToStringP(Response2.Messages[0].Data) + Environment.NewLine);
                                    int VinPattern = FindBytePattern(Response2.Messages[0].Data, VinNumberResponse);
                                    //tbLog.Text += ByteArrayToStringP(msg.Data) + Environment.NewLine;
                                    if (VinPattern != -1)
                                    {
                                        //Gimme that VIN
                                        i = 8;

                                        byte[] results = new byte[17];
                                        Array.Copy(Response2.Messages[0].Data, 6, results, 0, 17);
                                        tbVinNumber.Text = System.Text.Encoding.ASCII.GetString(results);
                                        details++;
                                        i = 8;
                                    }
                                }
                            }
                        }
                        i++;
                    }
                    //Reset our counter
                    i = 0;
                    //Send calibration ID message and check for response
                    Channel.SendMessage(calIdMsg);
                    while (i < 5)
                    {
                        GetMessageResults Response = Channel.GetMessage();
                        if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                        {
                            foreach (SAE.J2534.Message msg in Response.Messages)
                            {
                                if ((msg.RxStatus & RxFlag.START_OF_MESSAGE) != 0)
                                {
                                    GetMessageResults Response2 = Channel.GetMessage();
                                    tbLog.AppendText("Got Start Of CALID Message: " + Environment.NewLine + ByteArrayToStringP(Response2.Messages[0].Data) + Environment.NewLine);

                                    int CalibrationPattern = FindBytePattern(Response2.Messages[0].Data, CalibrationIdResponse);
                                    if (CalibrationPattern != -1)
                                    {
                                        i = 5;
                                        byte[] results = new byte[16];
                                        Array.Copy(Response2.Messages[0].Data, 6, results, 0, 11);
                                        tbCalibrationid.Text = System.Text.Encoding.ASCII.GetString(results);
                                        details++;
                                    }
                                }

                            }
                        }
                        i++;
                    }
                    btnDownloadSpecial.Enabled = true;
                    btnFlashRom.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DarkMessageBox.ShowError(ex.Message, "Error Connecting to vehicle");
            }
        }
        BackgroundWorker bgWorker;

        public void UpdateReadPercentage(int percentage)
        {
            progressBar.Value = percentage;
            tsLabel.Text = "Reading: " + percentage.ToString() + "%";
        }
        public void UpdateWritePercentage(int percentage)
        {
            progressBar.Value = percentage;
            tsLabel.Text = "Writing: " + percentage.ToString() + "%";
        }
        private void UpdateFlashReadProgress(object sender, ProgressChangedEventArgs e)
        {
            string logData = e.UserState as String;
            lblTimeString.Text = logData;
            UpdateReadPercentage(e.ProgressPercentage);
        }

        private void FlashReadFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (flashMemory != null) SaveFile(flashMemory);
        }


        void SaveFile(byte[] rawBinaryFile)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.RestoreDirectory = true;
            savefile.Filter = "Nissan Rom Dump|*.Bin";
            savefile.FileName = tbCalibrationid.Text;
            if (savefile.ShowDialog() != DialogResult.OK) return;

            File.WriteAllBytes(savefile.FileName, rawBinaryFile);

            DarkMessageBox.ShowInformation("Successfully Saved File!", "Success");
            cbProcessorSize.Enabled = true;
            btnDownloadSpecial.Enabled = true;
        }

        public void ReadFlashROM(Object sender, DoWorkEventArgs e)
        {
            using (API API = APIFactory.GetAPI(DllFileName))
            using (Device Device = API.GetDevice())
            using (Channel Channel = Device.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.NONE))
            {
                SConfig[] configList = new SConfig[1] { new SConfig(Parameter.LOOP_BACK, 0) };//, new SConfig(Parameter.DATA_RATE, 500000) };
                Channel.SetConfig(configList);
                Channel.StartMsgFilter(new MessageFilter(UserFilterType.STANDARDISO15765, new byte[] { 0x00, 0x00, 0x07, 0xE0 }));


                SAE.J2534.Message wakeup = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x10, 0xFB },
                TxFlag.ISO15765_FRAME_PAD
                );
                SAE.J2534.Message knockknock = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x21, 0xFF },
                TxFlag.ISO15765_FRAME_PAD
                );


                //Send out our first message to find the vin number
                Channel.SendMessage(wakeup);


                int i = 0;
                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Wakeup Neo: " + Environment.NewLine + ByteArrayToStringP(wakeup.Data) + Environment.NewLine)));

                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {
                            //Find wakeup for Reading Diag Mode
                            int whiteRabbit = FindBytePattern(msg.Data, WakeupResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Woah, Morpheus?: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }

                i = 0;
                Channel.SendMessage(knockknock);
                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {

                            int whiteRabbit = FindBytePattern(msg.Data, PrepareReadResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Knock Knock Neo: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }
                var stopwatch = new Stopwatch();

                stopwatch.Start();
                flashMemory = ReadFlashMemory(Channel, bgWorker);

                stopwatch.Stop();
                var timeTaken = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText($"Successfully read {flashMemory.Length} bytes of flash memory in {timeTaken.Minutes}:{timeTaken.Seconds}")));
                Device.SetProgrammingVoltage(Pin.PIN_12, -1);
            }
        }
        public void ReadFlashRAM(Object sender, DoWorkEventArgs e)
        {
            using (API API = APIFactory.GetAPI(DllFileName))
            using (Device Device = API.GetDevice())
            using (Channel Channel = Device.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.CAN_ID_BOTH))
            {

                MessageFilter FlowControlFilter = new MessageFilter()
                {
                    FilterType = Filter.FLOW_CONTROL_FILTER,
                    Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF },
                    Pattern = new byte[] { 0x00, 0x00, 0x7e, 0x08 },
                    FlowControl = new byte[] { 0x00, 0x00, 0x7e, 0x00 }
                };

                Channel.StartMsgFilter(FlowControlFilter);

                SAE.J2534.Message SetDiagMode = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x7e, 0x00, 0x10, 0xC0 },
                TxFlag.ISO15765_FRAME_PAD
                );

                SConfig[] configList = new SConfig[2] { new SConfig(Parameter.LOOP_BACK, 0), new SConfig(Parameter.DATA_RATE, 500000) };

                Channel.SetConfig(configList);

                Channel.SendMessage(SetDiagMode);

                int i = 0;

                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {

                            tbLog.AppendText("Wakeup Neo: " + ByteArrayToStringP(msg.Data) + Environment.NewLine);
                            if ((msg.RxStatus & RxFlag.START_OF_MESSAGE) != 0)
                            {
                                GetMessageResults Response2 = Channel.GetMessage();
                                tbLog.AppendText("Woah, Morpheus?: " + msg.Data.Length.ToString() + Environment.NewLine + Environment.NewLine);
                                i = 5;
                            }
                        }
                    }
                    i++;
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                flashMemory = ReadFlashRam(Channel, bgWorker);
                stopwatch.Stop();
                var timeTaken = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

                bgWorker.ReportProgress(0, $"Successfully read {flashMemory.Length} bytes of flash memory in {timeTaken.Minutes}:{timeTaken.Seconds}");
                Device.SetProgrammingVoltage(Pin.PIN_12, -1);
            }
        }


        public static int FindBytePattern(byte[] arrayToSearchThrough, byte[] patternToFind)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = 0; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
        public static string ByteArrayToStringP(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.Append("0x");
                hex.AppendFormat("{0:X2} ", b);
            }
            return hex.ToString();
        }

        private static int findSequence(byte[] array, byte[] sequence, int start = 0)
        {
            int end = array.Length - sequence.Length; // past here no match is possible
            byte firstByte = sequence[0]; // cached to tell compiler there's no aliasing

            while (start <= end)
            {
                // scan for first byte only. compiler-friendly.
                if (array[start] == firstByte)
                {
                    // scan for rest of sequence
                    for (int offset = 1; ; ++offset)
                    {
                        if (offset == sequence.Length)
                        { // full sequence matched?
                            return start;
                        }
                        else if (array[start + offset] != sequence[offset])
                        {
                            break;
                        }
                    }
                }
                ++start;
            }

            // end of array reached without match
            return -1;
        }

        public byte[] ReadFlashMemory(Channel channel, BackgroundWorker progressReporter = null)
        {
            byte[] buffer = { 0 };
            DateTime startTime = DateTime.Now;
            TimeSpan timeRemaining;
            string timeString;
            //This is the largest size we can request that is an even divisible number, 0x900 is supported but then we need an odd request at the end
            tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Read Memory Starts" + Environment.NewLine)));
            for (uint i = 0x0; i <= readsize; i += blockSize)
            {
                Application.DoEvents();

                timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (readsize - (i + 1)) / (i + 1));

                ReadMemoryByAddress(i, blockSize, out buffer, channel);

                timeString = "Time Remaining:" + string.Format("{0:mm\\:ss}", timeRemaining);
                progressChanged(i);
                //We recieved an incorrect amount of data, there is no way to handle this error so bubble it back to the user
                if (buffer.Length != blockSize)
                {
                    //Last bytes will throw this error as well.
                    tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Invalid Block size detected" + Environment.NewLine + $"Buffer :{buffer.Length} != blockSize {blockSize} " + Environment.NewLine)));
                }
                try
                {
                    Buffer.BlockCopy(buffer, 0, flashMemory, (int)i, buffer.Length);
                }
                catch
                {

                    tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText($"Failed block read at: {i}" + Environment.NewLine)));
                }
                if (i % 0x100 == 0)
                    tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText($"block read at: {i}" + Environment.NewLine)));

                //Report progress back to the GUI if there is one
                if (progressReporter != null) progressReporter.ReportProgress((int)((float)i / (float)readsize * 100.0f), timeString);
            }
            return flashMemory;
        }

        public byte[] ReadFlashRam(Channel channel, BackgroundWorker progressReporter = null)
        {
            byte[] flashMemory = new byte[0x100000];
            byte[] buffer = { 0 };
            uint blockSize = 0x04; //This is the largest size we can request that is an even divisible number, 0x900 is supported but then we need an odd request at the end
            bgWorker.ReportProgress(0, "Read Memory Starts");
            try
            {
                for (uint i = 0xFFFF0000; i <= 0xFFFFFFFF; i += blockSize)
                {
                    ReadMemoryByAddress(i, blockSize, out buffer, channel);

                    //We recieved an incorrect amount of data, there is no way to handle this error so bubble it back to the user
                    if (buffer.Length != blockSize)
                    {
                        bgWorker.ReportProgress(0, "Invalid Block size detected at Address" + i.ToString() + Environment.NewLine);
                        break;
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, 0, flashMemory, (int)(i - 0xFFFF0000), buffer.Length);
                    }
                    //Report progress back to the GUI if there is one
                    if (progressReporter != null) progressReporter.ReportProgress((int)((float)(i - 0xFFFF0000) / (float)0xFFFF * 100.0f));
                }
            }
            catch
            {
                return flashMemory;
            }
            return flashMemory;
        }

        DateTime lastUpdate;
        long lastBytes = 0;
        //Simple Rom download/upload Speed 
        private void progressChanged(long bytes)
        {
            if (lastBytes == 0)
            {
                lastUpdate = DateTime.Now;
                lastBytes = bytes;
                return;
            }
            var now = DateTime.Now;
            var timeSpan = now - lastUpdate;
            var bytesChange = bytes - lastBytes;
            var bytesPerSecond = bytesChange / timeSpan.TotalSeconds;


            lastBytes = bytes;
            lastUpdate = now;
            dlSpeed.Invoke((MethodInvoker)(() => dlSpeed.Text = "Download Rate: " + bytesPerSecond.ToString()));
        }



        public void ReadMemoryByAddress(uint address, uint blockSize, out byte[] memory, Channel channel)
        {
            //Send the read memory request
            byte blockSizeUpper = (byte)((blockSize >> 8) & 0xFF);
            byte blockSizeLower = (byte)(blockSize & 0xFF);
            //ISO14229 ReadMemoryByAddress
            //byte1 ServiceID 0x23
            //byte2 AddressAndLengthFormatIdentifier
            //byte3 address byte 1
            //byte4 address byte 2
            //byte5 address byte 3
            //byte6 address byte 4
            //byte7 block size byte1
            //byte8 block size byte2
            memory = new byte[] { 0x00 };

            byte[] txMsgBytes = { 0x00, 0x00, 0x07, 0xE0, 0x23, (byte)((address >> 24) & 0xFF), (byte)((address >> 16) & 0xFF), (byte)((address >> 8) & 0xFF), (byte)((address) & 0xFF), 0x00, (byte)blockSize };
            SAE.J2534.Message ReadMe = new SAE.J2534.Message(txMsgBytes, (TxFlag.ISO15765_FRAME_PAD));

            channel.SendMessage(ReadMe);

            GetMessageResults Response;

            int byteSearchIndex = -1;
            Response = channel.GetMessages(3, 100);
            byteSearchIndex = -1;
            if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
            {
                foreach (SAE.J2534.Message msg in Response.Messages)
                {
                    if (msg.Data.Length < 5)
                        continue;

                    byteSearchIndex = findSequence(msg.Data, positiveReadResponse, 0);

                    if (byteSearchIndex > -1)
                    {
                        byteSearchIndex += 5;
                        Array.Resize(ref memory, msg.Data.Length - byteSearchIndex);
                        Array.Copy(msg.Data, byteSearchIndex, memory, 0, msg.Data.Length - byteSearchIndex);
                        return;
                    }
                }
            }
        }

        private void btnDownloadSpecial_Click(object sender, EventArgs e)
        {
            tbConnectCAN.Enabled = false;
            if (DllFileName.Length == 0)
            {
                frmSelect2534 sd = new frmSelect2534();
                btnDownloadSpecial.Enabled = true;

                DialogResult dialogresult = sd.ShowDialog();

                if (dialogresult == DialogResult.OK)
                {
                    DllFileName = sd.apiInfo.Filename;
                }
                else
                {
                    tbLog.Text = "Couldn't open device selection form";
                    return;
                }
                sd.Dispose();
            }

            switch (cbProcessorSize.SelectedIndex)
            {
                case 0:
                    {
                        blockSize = 0x4;
                        readsize = 0x13FFFF;
                        Array.Resize(ref flashMemory, (int)readsize + 1);
                        flashMemory = Enumerable.Repeat((byte)0xFF, (int)readsize + 1).ToArray();
                    }
                    break;

                case 1:
                    {
                        blockSize = 0x4;
                        readsize = 0x17FFFF;
                        Array.Resize(ref flashMemory, (int)readsize + 1);
                        flashMemory = Enumerable.Repeat((byte)0xFF, (int)readsize + 1).ToArray();
                    }
                    break;
                case 2:
                    {
                        blockSize = 0x37;
                        //blockSize = 55;
                        readsize = 0xFFFFF;
                        Array.Resize(ref flashMemory, (int)readsize + 1);
                        flashMemory = Enumerable.Repeat((byte)0xFF, (int)readsize + 1).ToArray();
                    }
                    break;
                case 3:
                    {
                        blockSize = 49;
                        readsize = 0x1FFFFF;
                        Array.Resize(ref flashMemory, (int)readsize + 1);
                        flashMemory = Enumerable.Repeat((byte)0xFF, (int)readsize + 1).ToArray();
                    }
                    break;
                case 4:
                    {
                        blockSize = 0x32;
                        readsize = 0x80000;
                        Array.Resize(ref flashMemory, (int)readsize + 1);
                        flashMemory = Enumerable.Repeat((byte)0xFF, (int)readsize + 1).ToArray();
                    }
                    break;
                default:
                    { }
                    break;
            }

            DialogResult dialogResultcs = DarkMessageBox.ShowInformation("Would you like to read ROM(Yes) or RAM(No)?", "Nii-Saan Rom Read", DarkDialogButton.YesNo);
            if (dialogResultcs == DialogResult.Yes)
            {

                cbProcessorSize.Enabled = false;
                btnDownloadSpecial.Enabled = false;
                bgWorker = new BackgroundWorker();

                bgWorker.WorkerReportsProgress = true;

                bgWorker.DoWork += new DoWorkEventHandler(ReadFlashROM);
                bgWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateFlashReadProgress);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FlashReadFinished);

                bgWorker.RunWorkerAsync();
            }
            else
            {
                cbProcessorSize.Enabled = false;
                btnDownloadSpecial.Enabled = false;
                bgWorker = new BackgroundWorker();

                bgWorker.WorkerReportsProgress = true;

                bgWorker.DoWork += new DoWorkEventHandler(ReadFlashRAM);
                bgWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateFlashReadProgress);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FlashReadFinished);

                bgWorker.RunWorkerAsync();
            }




        }

        private void btnCheckAlgo_Click(object sender, EventArgs e)
        {
            using (API API = APIFactory.GetAPI(DllFileName))
            using (Device Device = API.GetDevice())
            using (Channel Channel = Device.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.CAN_29BIT_ID))
            {

                MessageFilter FlowControlFilter = new MessageFilter()
                {
                    FilterType = Filter.FLOW_CONTROL_FILTER,
                    Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF },
                    Pattern = new byte[] { 0x00, 0x00, 0x7e, 0x08 },
                    FlowControl = new byte[] { 0x00, 0x00, 0x7e, 0x00 }
                };

                Channel.StartMsgFilter(FlowControlFilter);

                SConfig[] configList = new SConfig[2] { new SConfig(Parameter.LOOP_BACK, 1), new SConfig(Parameter.DATA_RATE, 500000) };
                Channel.SetConfig(configList);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                flashMemory = ReadRamAddress(Channel, bgWorker);
                stopwatch.Stop();
                var timeTaken = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
                tbLog.Text += ByteArrayToStringP(flashMemory) + Environment.NewLine;
            }
        }
        public byte[] ReadRamAddress(Channel channel, BackgroundWorker progressReporter = null)
        {
            byte[] flashMemory = new byte[0x04];
            byte[] buffer = { 0 };
            uint blockSize = uint.Parse(tbSize.Text);
            uint address = uint.Parse(tbAddress.Text, NumberStyles.HexNumber);
            ReadMemoryByAddress(address, blockSize, out buffer, channel);

            //We recieved an incorrect amount of data, there is no way to handle this error so bubble it back to the user
            if (buffer.Length != blockSize)
            {
                //bgWorker.ReportProgress(0, "Invalid Block size detected at Address" + address.ToString() + Environment.NewLine);
            }
            else
            {
                Buffer.BlockCopy(buffer, 0, flashMemory, 0, buffer.Length);
            }
            return flashMemory;
        }


        private void darkButton2_Click(object sender, EventArgs e)
        {
            if (DllFileName == String.Empty)
            {
                frmSelect2534 sd = new frmSelect2534();
                btnDownloadSpecial.Enabled = true;

                DialogResult dialogresult = sd.ShowDialog();

                if (dialogresult == DialogResult.OK)
                {
                    DllFileName = sd.apiInfo.Filename;
                }
                else
                {
                    tbLog.Text = "Couldn't open device selection form";
                    return;
                }
                sd.Dispose();
            }

            try
            {


                using (API API = APIFactory.GetAPI(DllFileName))
                using (Device Device = API.GetDevice())
                using (Channel Channel = Device.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.SNIFF_MODE))
                {
                    List<byte[]> msgs = new List<byte[]>();


                    GetMessageResults Response = Channel.GetMessages(10000);

                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {

                        foreach (var msg in Response.Messages)
                        {
                            msgs.Add(msg.Data);
                        }

                    }


                    if (msgs.Count() > 0)
                    {
                        for (int i = 0; i < msgs.Count(); i++)
                        {
                            tbLog.AppendText($"message[{i}]: " + ByteArrayToStringP(msgs[i]) + Environment.NewLine);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void btnFlashRom_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "calibration files (*.bin)|*.bin|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    writeFile = File.ReadAllBytes(filePath);


                    bgWorker = new BackgroundWorker();

                    bgWorker.WorkerReportsProgress = true;

                    bgWorker.DoWork += new DoWorkEventHandler(WriteFlashStart);
                    bgWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateFlashWriteProgress);
                    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FlashWriteFinished);

                    bgWorker.RunWorkerAsync();

                }

            }
        }
        private void WriteFlashStart(Object sender, DoWorkEventArgs e)
        {
            using (API API = APIFactory.GetAPI(DllFileName))
            using (Device Device = API.GetDevice())
            using (Channel Channel = Device.GetChannel(Protocol.ISO15765, Baud.ISO15765, ConnectFlag.NONE))
            {
                SConfig[] configList = new SConfig[1] { new SConfig(Parameter.LOOP_BACK, 0) };//, new SConfig(Parameter.DATA_RATE, 500000) };
                Channel.SetConfig(configList);
                Channel.StartMsgFilter(new MessageFilter(UserFilterType.STANDARDISO15765, new byte[] { 0x00, 0x00, 0x07, 0xE0 }));


                SAE.J2534.Message wakeup = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x10, 0xC0 },
                TxFlag.ISO15765_FRAME_PAD
                );
                SAE.J2534.Message knockknock = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x21, 0xFF },
                TxFlag.ISO15765_FRAME_PAD
                );

                SAE.J2534.Message enableWriteDiag = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x10, 0x85 },
                TxFlag.ISO15765_FRAME_PAD
                );
                SAE.J2534.Message RequestSeed = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x27, 0x81 },
                TxFlag.ISO15765_FRAME_PAD
                );
                SAE.J2534.Message SendKey = new SAE.J2534.Message(
                new byte[] { 0x00, 0x00, 0x07, 0xE0, 0x27, 0x82, 0x2C, 0x46, 0x63, 0x9C },
                TxFlag.ISO15765_FRAME_PAD
                );
                //Send out our first message to find the vin number
                Channel.SendMessage(wakeup);


                int i = 0;
                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Wakeup Neo: " + Environment.NewLine + ByteArrayToStringP(wakeup.Data) + Environment.NewLine)));

                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {
                            //Find wakeup for Reading Diag Mode
                            int whiteRabbit = FindBytePattern(msg.Data, WakeupResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Woah, Morpheus?: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }


                i = 0;
                Channel.SendMessage(knockknock);
                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {

                            int whiteRabbit = FindBytePattern(msg.Data, PrepareReadResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Knock Knock Neo: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }
                i = 0;
                Channel.SendMessage(enableWriteDiag);


                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("It's time Neo: " + Environment.NewLine + ByteArrayToStringP(wakeup.Data) + Environment.NewLine)));

                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {
                            //Find wakeup for Reading Diag Mode
                            int whiteRabbit = FindBytePattern(msg.Data, WakeupResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("The Matrix has you: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }

                i = 0;
                Channel.SendMessage(RequestSeed);


                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Blue Pill?: " + Environment.NewLine + ByteArrayToStringP(RequestSeed.Data) + Environment.NewLine)));

                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {
                            //Find wakeup for Reading Diag Mode
                            int whiteRabbit = FindBytePattern(msg.Data, SeedResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Red Pilled: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }
                i = 0;
                Channel.SendMessage(SendKey);

                while (i < 5)
                {
                    GetMessageResults Response = Channel.GetMessage();
                    if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                    {
                        foreach (SAE.J2534.Message msg in Response.Messages)
                        {
                            //Find wakeup for Reading Diag Mode
                            int whiteRabbit = FindBytePattern(msg.Data, KeyResponse);
                            if (whiteRabbit != -1)
                            {
                                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("The Machines: " + Environment.NewLine + ByteArrayToStringP(msg.Data) + Environment.NewLine + Environment.NewLine)));
                                i = 5;
                            }
                        }
                    }
                    i++;
                }


                var stopwatch = new Stopwatch();

                stopwatch.Start();
                WriteFlash(Channel, writeFile, bgWorker);

                stopwatch.Stop();
                var timeTaken = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

                tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText($"Successfully read {flashMemory.Length} bytes of flash memory in {timeTaken.Minutes}:{timeTaken.Seconds}")));
                Device.SetProgrammingVoltage(Pin.PIN_12, -1);
            }
        }
        public void WriteFlash(Channel channel, byte[] data, BackgroundWorker progressReporter = null)
        {

            EraseFlash(channel, progressReporter);
            //RequestDownload(channel);
            //F80000 is the size of calibrations only;
            DateTime startTime = DateTime.Now;
            TimeSpan timeRemaining;
            string timeString;

            if (data.Length < 0xF8000) return;

            int i;
            byte j = 0x01;
            var blocksize = 128;
            for (i = 0x00; i < 0xF8000; i += blocksize)
            {
                timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (0xF8000 - (i + 1)) / (i + 1));


                TransferData(channel, data, i, j, blocksize);
                timeString = "Time Remaining:" + string.Format("{0:mm\\:ss}", timeRemaining);
                progressChanged(i);

                if (j == byte.MaxValue)
                    j = 0x00;
                else
                    j++;

                if (progressReporter != null)
                    progressReporter.ReportProgress((int)((float)i / (float)0xF8000 * 100.0f), timeString);
            }
            RequestTransferExit(channel);
        }

        private void UpdateFlashWriteProgress(object sender, ProgressChangedEventArgs e)
        {
            string logData = e.UserState as String;
            lblTimeString.Text = logData;
            UpdateWritePercentage(e.ProgressPercentage);
        }

        private void FlashWriteFinished(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        public void EraseFlash(Channel channel, BackgroundWorker progressReporter = null)
        {

            byte[] txCipherBytes = { 0x00, 0x00, 0x07, 0xE0, 0x31, 0x81, 0x82, 0xF0, 0x5A };
            SAE.J2534.Message SetBlankCipher = new SAE.J2534.Message(txCipherBytes, TxFlag.ISO15765_FRAME_PAD);
            channel.SendMessage(SetBlankCipher);

            bool cipherResponse = false;
            byte[] positiveCipher = { 0xE8, 0x71, 0x81, 0x01 };

            while (!cipherResponse)
            {
                GetMessageResults Response = channel.GetMessages(5, 1000);
                int BSI = -1;
                if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                {
                    foreach (SAE.J2534.Message msg in Response.Messages)
                    {
                        BSI = findSequence(msg.Data, positiveCipher, 0);

                        if (BSI > 0)
                        {
                            tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Erase Started" + Environment.NewLine)));
                            cipherResponse = true;
                            break;
                        }
                    }
                }
            }


            bgWorker.ReportProgress(0, ("Extended Function Set") + Environment.NewLine);

            byte[] txMsgBytes = { 0x00, 0x00, 0x07, 0xE0, 0x31, 0x81, 0x01 };
            SAE.J2534.Message eraseMe = new SAE.J2534.Message(txMsgBytes, TxFlag.ISO15765_FRAME_PAD);



            bool eraseCompleted = false;
            int erased = -1;

            byte[] eraseConf = { 0xE8, 0x71, 0x81, 0x02, };

            while (!eraseCompleted)
            {
                channel.SendMessage(eraseMe);

                GetMessageResults Response = channel.GetMessages(2, 1000);
                if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                {
                    foreach (SAE.J2534.Message msg in Response.Messages)
                    {
                        erased = findSequence(msg.Data, eraseConf, 0);

                        if (erased > 0)
                        {
                            eraseCompleted = true;
                            tbLog.Invoke((MethodInvoker)(() => tbLog.AppendText("Flash Erased, Ready for Write" + Environment.NewLine)));
                            break;
                        }
                    }
                }
            }
        }

        //Nissans don't use 36 for some weird reason. 
        public void TransferData(Channel channel, byte[] data, int offset, byte blockNum, int length, int blockSize = -1)
        {
            //ISO14229 RequestDownload
            //byte1 ServiceID 0x34
            //byte2 DataFormatIdentifier 
            //      High nibble = memorySize)
            //      Low nibble  = memoryAddress
            //byte3 AddressAndLengthFormatIdentifier
            //byte4 memoryAddressByte1
            //byte5 memoryAddressByte2
            //byte6 memoryAddressByte3
            //byte7 uncompressedMemorySizeByte1
            //byte8 uncompressedMemorySizeByte1
            //byte9 uncompressedMemorySizeByte1


            ////We need to do something more clever here based upon the flash size

            if (blockSize == -1) blockSize = length;

            byte[] txMsgBytes = new byte[(length + 10)];
            txMsgBytes[0] = 0x00;
            txMsgBytes[1] = 0x00;
            txMsgBytes[2] = 0x07;
            txMsgBytes[3] = 0xE0;
            txMsgBytes[4] = 0x34;
            txMsgBytes[5] = 0x82;
            txMsgBytes[6] = (byte)((offset >> 16) & 0xFF);
            txMsgBytes[7] = (byte)((offset >> 8) & 0xFF);
            txMsgBytes[8] = (byte)((offset) & 0xFF);
            txMsgBytes[9] = (byte)blockSize;

            Buffer.BlockCopy(data, offset, txMsgBytes, 10, blockSize);

            SAE.J2534.Message TransferData = new SAE.J2534.Message(txMsgBytes, (TxFlag.ISO15765_FRAME_PAD));

            ushort test = bruteforce(TransferData.Data);
            channel.SendMessage(TransferData);

            bool gotResponse = false;

            byte[] positiveResponse = { 0xE8, 0x74, 0x02, };

            while (!gotResponse)
            {
                GetMessageResults Response = channel.GetMessages(4, 250);
                int byteSearchIndex = -1;
                if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                {
                    foreach (SAE.J2534.Message msg in Response.Messages)
                    {
                        byteSearchIndex = findSequence(msg.Data, positiveResponse, 0);

                        if (byteSearchIndex > 0)
                        {
                            gotResponse = true;
                            return;
                        }
                    }
                }
            }
        }

        public void RequestTransferExit(Channel channel)
        {
            byte[] txCipherBytes = { 0x00, 0x00, 0x07, 0xE0, 0x31, 0x82, 0x00 };
            SAE.J2534.Message SetBlankCipher = new SAE.J2534.Message(txCipherBytes, TxFlag.ISO15765_FRAME_PAD);
            channel.SendMessage(SetBlankCipher);

            bool cipherResponse = false;
            byte[] positiveCipher = { 0xE8, 0x71, 0x82, 0x01 };
            int i = 0;
            while (!cipherResponse)
            {
                GetMessageResults Response = channel.GetMessages(3, 500);
                int BSI = -1;
                if (Response.Result != ResultCode.DEVICE_NOT_CONNECTED)
                {
                    foreach (SAE.J2534.Message msg in Response.Messages)
                    {
                        BSI = findSequence(msg.Data, positiveCipher, 0);

                        if (BSI > 0)
                        {
                            cipherResponse = true;
                            bgWorker.ReportProgress(0, ("Transfer Exited") + Environment.NewLine);
                            break;
                        }
                    }
                }
            }

            byte[] txBytes = { 0x00, 0x00, 0x07, 0xE0, 0x31, 0x82, 0x01 };
            SAE.J2534.Message routine = new SAE.J2534.Message(txBytes, TxFlag.ISO15765_FRAME_PAD);
            channel.SendMessage(routine);

            int a = 0;
            GetMessageResults Responses = channel.GetMessages(4, 1000);
            if (Responses.Result != ResultCode.DEVICE_NOT_CONNECTED)
            {
                foreach (SAE.J2534.Message msg in Responses.Messages)
                {
                    a++;
                }
            }


            byte[] exitBytes = { 0, 0x00, 0x07, 0xE0, 0x10, 0x81, };
            SAE.J2534.Message ExitRoutine = new SAE.J2534.Message(exitBytes, TxFlag.ISO15765_FRAME_PAD);
            channel.SendMessage(ExitRoutine);

            int b = 0;
            GetMessageResults Response2 = channel.GetMessages(4, 1000);
            if (Response2.Result != ResultCode.DEVICE_NOT_CONNECTED)
            {
                foreach (SAE.J2534.Message msg in Response2.Messages)
                {
                    b++;
                }
            }






        }


        public ushort CRC_calc(ref byte[] msg_pntr, ushort msg_length, bool start, ushort prtl_CRC)
        {
            uint uVar1;
            byte uVar2;
            ushort CRC;
            byte bVar4;
            uint uVar5;

            if (start)
            {
                CRC = 0xffff; //initial value for CRC
            }
            else
            {
                CRC = prtl_CRC; //the result of the payload CRC without the last two bytes
            }
            uVar5 = 0;
            if (msg_length != 0)
            {
                do
                {
                    uVar2 = msg_pntr[uVar5];
                    bVar4 = 0;
                    do
                    {
                        uVar1 = (uint)(CRC & 1);
                        CRC >>= 1;
                        if (uVar1 != (uVar2 & 1))
                        { //check the CRC and the rightmost bit of the data
                            CRC = (ushort)(CRC ^ 0x8408);
                        }
                        bVar4 += 1;
                        uVar2 >>= 1;
                    } while (bVar4 < 8); //for each bit
                    uVar5 += 1;
                } while (uVar5 < msg_length); //for each byte
            }
            return CRC; //bootloader expects 0xF0B8 as result
        }

        private ushort bruteforce(byte[] msg)
        {
            ushort reqd_CRC = 0xF0B8;
            ushort prtl_CRC;
            prtl_CRC = CRC_calc(ref msg, (ushort)(msg.Length - 2), true, 0xFFFF); //send UDS payload for CRC without last 2 bytes
            byte[] trial_bytes = new byte[] { 0, 0 };
            int start;
            int elapsed;
            ushort result;
            ushort guess;
            guess = (ushort)((trial_bytes[0] << 8) | trial_bytes[1]);
            //start = millis();
            while (guess < 0xFFFF) //need a better condition here
            {
                result = CRC_calc(ref trial_bytes, 2, false, prtl_CRC);
                //SerialUSB.print(guess, HEX);
                //SerialUSB.print(" ");
                //SerialUSB.println(result, HEX);
                if (result == reqd_CRC)
                {
                    //elapsed = millis() - start;
                    //SerialUSB.print("Bruteforce time (ms): ");
                    //SerialUSB.println(elapsed, DEC);
                    return (ushort)((trial_bytes[0] << 8) | trial_bytes[1]);
                }
                // check for a close result, we have last 3 bits correct when result is 0xF0Bx
                if ((result & 0xFFF0) == (reqd_CRC & 0xFFF0))
                {
                    switch (result & 0xf)
                    {
                        case 0: //F0B0
                            trial_bytes[0] ^= 0x88;
                            trial_bytes[1] ^= 0x40;
                            break;
                        case 1: //F0B1
                            trial_bytes[0] ^= 0x99;
                            trial_bytes[1] ^= 0x48;
                            break;
                        case 2: //F0B2
                            trial_bytes[0] ^= 0xAA;
                            trial_bytes[1] ^= 0x50;
                            break;
                        case 3: //F0B3
                            trial_bytes[0] ^= 0xBB;
                            trial_bytes[1] ^= 0x58;
                            break;
                        case 4: //F0B4
                            trial_bytes[0] ^= 0xCC;
                            trial_bytes[1] ^= 0x60;
                            break;
                        case 5: //F0B5
                            trial_bytes[0] ^= 0xDD;
                            trial_bytes[1] ^= 0x68;
                            break;
                        case 6: //F0B6
                            trial_bytes[0] ^= 0xEE;
                            trial_bytes[1] ^= 0x70;
                            break;
                        case 7: //F0B7
                            trial_bytes[0] ^= 0xFF;
                            trial_bytes[1] ^= 0x78;
                            break;
                        case 9: //F0B9
                            trial_bytes[0] ^= 0x11;
                            trial_bytes[1] ^= 0x08;
                            break;
                        case 10: //F0BA
                            trial_bytes[0] ^= 0x22;
                            trial_bytes[1] ^= 0x10;
                            break;
                        case 11: //F0BB
                            trial_bytes[0] ^= 0x33;
                            trial_bytes[1] ^= 0x18;
                            break;
                        case 12: //F0BC
                            trial_bytes[0] ^= 0x44;
                            trial_bytes[1] ^= 0x20;
                            break;
                        case 13: //F0BD
                            trial_bytes[0] ^= 0x55;
                            trial_bytes[1] ^= 0x28;
                            break;
                        case 14: //F0BE
                            trial_bytes[0] ^= 0x66;
                            trial_bytes[1] ^= 0x30;
                            break;
                        case 15: //F0BF
                            trial_bytes[0] ^= 0x77;
                            trial_bytes[1] ^= 0x38;
                            break;
                    }
                    //elapsed = millis() - start;
                    //SerialUSB.print("Bruteforce time (ms): ");
                    //SerialUSB.println(elapsed, DEC);
                    return (ushort)((trial_bytes[0] << 8) | trial_bytes[1]);
                }
                else
                {
                    guess += 1;
                }
                trial_bytes[0] = (byte)(guess >> 8);
                trial_bytes[1] = (byte)(guess & 0xff);
            }
            //elapsed = millis() - start;
            //SerialUSB.print("Fail to find bytes. Time (ms): ");
            //SerialUSB.println(elapsed, DEC);
            return 0; //should probably return a bool and pass trial bytes as a pointer
        }

    }
}
