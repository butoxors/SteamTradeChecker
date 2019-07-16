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
        public Alert(string t, Point l)
        {
            this.Top = l.Y;
            this.Left = l.X;

            InitializeComponent();
            label1.Text = t;
        }

        private void Alert_Click(object sender, EventArgs e)
        {
            using(Process proc = new Process())
            {
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
            }
            this.Close();
        }
    }
}
