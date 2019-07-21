using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Threading.Tasks;
using Main.Support;
using System.IO;
using Main.JSON_Classes.LootFarm;

namespace Main
{
    public partial class AccountControl : UserControl
    {
        private string HOME = AppDomain.CurrentDomain.BaseDirectory + "avatar.jpg";

        WebClient client = new WebClient();

        public AccountControl()
        {
            InitializeComponent();
        }
        public void GetData()
        {
            var res = Task.Run(() => GetJSONData.GetXHR(Links.LOOT_ACCOUNT_URL));
            var account = LootAccount.FromJson(res.Result);
            if (!File.Exists(HOME))
            {
                client.DownloadFile(account.Avatar ?? new Uri("https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/58/58de4dcd213d2b774a5d41b38158b724ff1232e4_medium.jpg"), HOME);
            }
            lBalance.Text = String.Format($"{account.Balance * 0.01}$");
            lNick.Text = account.Nickname;
            pictureBox1.Image = Image.FromFile(HOME);
            //GetUserInventory();
        }
        public void GetUserInventory()
        {
            var res = Task.Run(() => GetJSONData.GetXHR(Links.LOOT_USER_DOTA));
            var inv = LootUserInventory.FromJson(res.Result);
            var s = inv.Result.Where(x => chItems.Checked ? x.Value.Tk > 0 : x.Value.P > 0).Select(x => new { Name = x.Value.N, Price = x.Value.P * 0.01, Free = x.Value.Tk }).ToList();
            dataGridView1.DataSource = s;
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetData();
        }
    }
}
