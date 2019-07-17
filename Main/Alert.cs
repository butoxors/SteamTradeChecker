using Main.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class Alert : Form
    {
        private string text;

        public Alert(string t)
        {
            InitializeComponent();
            text = t;
            label1.Text = t;
        }

        private void Alert_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(text);
            Process.Start(@"https://loot.farm/");
            this.Close();
        }

        private void Alert_Load(object sender, EventArgs e)
        {
            this.Top = 60;
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 50;
        }
    }
}
