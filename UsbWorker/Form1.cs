using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace UsbWorker
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort1;
        private List<string> dataFormat = new List<string>{"Hex","Decimal","Binary","Char"};
        private int selectedDataReceive = 3;
        private int selectedDataTransmitte = 3;
        private int[] dataInDec;

        private string OutputData;
        private List<string> ports = new List<string>();

        private string DataIn;
        public Form1()
        {
            InitializeComponent();

            serialPort1 = new SerialPort();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(this.serialPort1_DataReceived);

            ports.AddRange(SerialPort.GetPortNames());

            lbStatus.Text = "Disconnect...";
            lbStatus.BackColor = Color.Red;
            btnClose.Enabled = false;

            cmbDataFormat.Items.AddRange(dataFormat.ToArray());
            cmbTransmitteDataFormat.Items.AddRange(dataFormat.ToArray());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbPort.Items.AddRange(ports.ToArray());

            this.cmbTransmitteDataFormat.SelectionChangeCommitted += 
                new EventHandler(this.cmbTransmitteDataFormat_SelectionChangeCommitted);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cmbPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(cmbBaud.Text);
                serialPort1.DataBits = Convert.ToInt32(cmbData.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStop.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);

                serialPort1.Open();

                lbStatus.Text = "Connect...";
                lbStatus.BackColor = Color.Green;

                btnOpen.Enabled = false;
                btnClose.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            lbStatus.Text = "Disconnect...";
            lbStatus.BackColor = Color.Red;

            btnOpen.Enabled = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                //OutputData = tbDataOut.Text;
                //serialPort1.WriteLine(OutputData);
                RX_Data_Send();
            }
        }

        private void tbDataOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (serialPort1.IsOpen)
                {
                    OutputData = tbDataOut.Text;
                    serialPort1.WriteLine(OutputData);
                }
                else return;
            }
            else return;
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //DataIn = serialPort1.ReadExisting();
            List<int> dataBuffer = new List<int>();
            while (serialPort1.BytesToRead>0)
            {
                try
                {
                    dataBuffer.Add(serialPort1.ReadByte());
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            dataInDec = new int[dataBuffer.Count()];
            dataInDec = dataBuffer.ToArray();

            this.Invoke(new EventHandler(ShowData));
        }

        private void ShowData(object sender, EventArgs e)
        {
            DataIn = RX_Data(dataInDec);
            
            if (checkBox1.Checked)
            {
                tbDataIn.Text = DataIn;
            }
            else if (checkBox2.Checked)
            {
                tbDataIn.Text += DataIn;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbDataIn.Text = string.Empty;
        }

        private void cmbSaveAs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbDataIn.Text==string.Empty)
            {
                return;
            }

            var send = sender as ComboBox;
            switch (send.SelectedIndex)
            {
                case 0:
                    this.Invoke(new EventHandler(SaveAsTxt));
                    break;
                default:
                    break;
            }
        }

        private void SaveAsTxt(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("Data.txt", true))
                {
                    writer.Write(tbDataIn.Text);
                }
                MessageBox.Show("Data was saved", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {

                MessageBox.Show("File was not saved", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private string RX_Data(int[] dataIn)
        {
            string outString = "";
            //"Hex","Decimal","Binary","Char"
            
            switch (selectedDataReceive)
            {
                case 0:
                    foreach (var item in dataIn)
                    {
                        outString += Convert.ToString(item, 16) + "\t";
                    }
                    break;
                case 1:
                    foreach (var item in dataIn)
                    {
                        outString += Convert.ToString(item) + "\t";
                    }
                    break;
                case 2:
                    foreach (var item in dataIn)
                    {
                        outString += Convert.ToString(item, 2) + "\t";
                    }
                    break;
                case 3:
                    foreach (var item in dataIn)
                    {
                        outString += Convert.ToChar(item) + "\t";
                    }
                    break;
                default:
                    break;
            }
            return outString;
        }

        private void cmbDataFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDataReceive = cmbDataFormat.SelectedIndex;
        }

        private void cmbTransmitteDataFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDataTransmitte = cmbTransmitteDataFormat.SelectedIndex;
        }

        private void tbDataOut_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            switch (selectedDataTransmitte)
            {
                case 0:
                    char upper = char.ToUpper(c);
                    if (!char.IsDigit(upper) && upper!=8 && upper!=46 && upper!=','
                        && !(upper>=65 && upper<=70))
                    {
                        e.Handled = true;
                    }
                    break;
                case 1:
                    if (!char.IsDigit(c) && c != 8 && c != 46 && c != ',')
                    {
                        e.Handled = true;
                    }
                    break;
                case 2:
                    if (c != 49 && c != 48 && c != 8 && c != 46 && c != ',')
                    {
                        e.Handled = true;
                    }
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }
        private void cmbTransmitteDataFormat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            tbDataOut.Clear();
        }
        private void RX_Data_Send()
        {
            if(selectedDataTransmitte ==3)
            {
                serialPort1.Write(tbDataOut.Text);
            }
            else
            {
                try
                {
                    string dataOutBuffer;
                    int countComma = 0;
                    string[] dataPrepareToSend;
                    byte[] dataToSend;

                    dataOutBuffer = tbDataOut.Text;
                    foreach (var c in dataOutBuffer)
                    {
                        if (c == ',')
                        {
                            countComma++;
                        }
                    }
                    dataPrepareToSend = new string[countComma];
                    countComma = 0;
                    foreach (var c in dataOutBuffer)
                    {
                        if (c != ',')
                        {
                            dataPrepareToSend[countComma] += c;
                        }
                        else
                        {
                            countComma++;

                            if (countComma == dataPrepareToSend.GetLength(0))
                            {
                                break;
                            }
                        }
                    }

                    dataToSend = new byte[dataPrepareToSend.Length];

                    //"Hex","Decimal","Binary","Char"
                    switch (selectedDataTransmitte)
                    {
                        case 0://hex
                            for (int i = 0; i < dataToSend.Length; i++)
                            {
                                dataToSend[i] = Convert.ToByte(dataPrepareToSend[i], 16);
                            }
                            serialPort1.Write(dataToSend, 0, dataToSend.Length);
                            break;
                        case 1://decimal
                            for (int i = 0; i < dataToSend.Length; i++)
                            {
                                dataToSend[i] = Convert.ToByte(dataPrepareToSend[i], 10);
                            }
                            serialPort1.Write(dataToSend, 0, dataToSend.Length);
                            break;
                        case 2://binary
                            for (int i = 0; i < dataToSend.Length; i++)
                            {
                                dataToSend[i] = Convert.ToByte(dataPrepareToSend[i], 2);
                            }
                            serialPort1.Write(dataToSend, 0, dataToSend.Length);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}
