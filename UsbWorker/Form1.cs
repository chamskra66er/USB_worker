﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace UsbWorker
{
    public partial class Form1 : Form
    {
        private string OutputData;
        private List<string> ports = new List<string>();

        private string DataIn;
        public Form1()
        {
            InitializeComponent();

            ports.AddRange(SerialPort.GetPortNames());
            lbStatus.Text = "Disconnect...";
            lbStatus.BackColor = Color.Red;
            btnClose.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbPort.Items.AddRange(ports.ToArray());
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
                OutputData = tbDataOut.Text;
                serialPort1.WriteLine(OutputData);
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

        }
    }
}
