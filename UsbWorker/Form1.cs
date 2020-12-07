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

namespace UsbWorker
{
    public partial class Form1 : Form
    {
        private string[] ports;
        public Form1()
        {
            InitializeComponent();

            ports = SerialPort.GetPortNames();
        }

    }
}
