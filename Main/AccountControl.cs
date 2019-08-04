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
using System.Net.Http;

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

        public void GetUserInventory(string json)
        {
            var inv = LootUserInventory.FromJson(json);
            var s = inv.Result.Where(x => x.Value.U.FindAll(a => a.Tr > 0).Count > 0).Select(x => new { Name = x.Value.N, Price = x.Value.P * 0.01, Free = x.Value.Tk }).Where(x => x.Free > 0).ToList();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = s;
        }

        public void GetAccountData(string res)
        {
            var account = LootAccount.FromJson(res);
            if (!File.Exists(HOME))
            {
                client.DownloadFile(account.Avatar ?? new Uri("https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/58/58de4dcd213d2b774a5d41b38158b724ff1232e4_medium.jpg"), HOME);
            }
            lBalance.Text = String.Format($"{account.Balance * 0.01}$");
            lNick.Text = account.Nickname;
            pictureBox1.Image = Image.FromFile(HOME);
        }
        private void btnRefresh_ClickAsync(object sender, EventArgs e)
        {
            var res = Task.Run(() => MakeAccountRequest(Links.LOOT_ACCOUNT_URL));
            GetAccountData(res.Result);

            var inv = Task.Run(() => MakeAccountRequest(Links.LOOT_USER_DOTA));
            GetUserInventory(inv.Result);
        }
        private CookieContainer GetCookies()
        {
            CookieContainer c = new CookieContainer();

            c.Add(new Cookie("__tawkuuid", "e::loot.farm::yyDXu/0fgsfBerohz6J87qNFs/DxaseqxdkdT5bpu4PQxl5U4q5DTwuz9roh4jZR::2", string.Empty, "loot.farm"));
            c.Add(new Cookie("_fbp", "fb.1.1558441183960.1908168188", string.Empty, "loot.farm"));
            c.Add(new Cookie("_ga", "GA1.2.363365182.1552735823", string.Empty, "loot.farm"));
            c.Add(new Cookie("_gid", "GA1.2.1714741804.1564910268", string.Empty, "loot.farm"));
            c.Add(new Cookie("PHPSESSID", "04594417185b8460eb79f5cc3a52736c", string.Empty, "loot.farm"));
            c.Add(new Cookie("currency", "USD", string.Empty, "loot.farm"));
            c.Add(new Cookie("lang", "ru", string.Empty, "loot.farm"));
            c.Add(new Cookie("noCancelSpam", "1", string.Empty, "loot.farm"));
            c.Add(new Cookie("TawkConnectionTime", "0", string.Empty, "loot.farm"));

            return c;
        }
        private async Task<string> MakeAccountRequest(string URL)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                using (HttpClient http = new HttpClient(handler))
                {
                    http.DefaultRequestHeaders.Add("X-Answer", "42");

                    handler.CookieContainer = GetCookies();

                    string html = await http.GetStringAsync(URL);
                    return html;
                }
            }
        }
    }
}
