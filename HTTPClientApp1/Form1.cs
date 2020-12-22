using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTTPClientApp1
{
    public partial class Form1 : Form
    {
        socketctl client;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new socketctl();
            client.Connect("10.63.81.185", 11000);

            client



        }
    }
}
