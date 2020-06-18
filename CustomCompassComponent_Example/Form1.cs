using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomCompassComponent_Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            customCompass1.UpdateCompass(trackBar1.Value);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            customCompass1.UpdateCompass(trackBar1.Value);
        }
    }
}
