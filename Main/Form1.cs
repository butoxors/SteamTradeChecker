using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class Form1 : Form
    {
        readonly HttpClient client = new HttpClient();

        SwapItems swapItems = null;
        List<LootItems> lootItems = new List<LootItems>();
        DealsItems Deals = new DealsItems();
        List<DotaMoney> DotaMoneyItems = new List<DotaMoney>();

        private double SWAPPerc = 0.08;
        private double LOOTPerc = 0.03;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                var res = Task.Run(() => GetXHR("https://api.swap.gg/prices/252490"));
                swapItems = SwapItems.FromJson(res.Result);
                GetLootItems("https://loot.farm/fullpriceRUST.json");
                res = Task.Run(() => GetXHR("https://dota.money/570/load_bots_inventory"));
                DotaMoneyItems = DotaMoney.FromJson(res.Result);

            }
            else if (radioButton1.Checked)
            {
                var res = Task.Run(() => GetXHR("https://api.swap.gg/prices/570"));
                swapItems = SwapItems.FromJson(res.Result);
                GetLootItems("https://loot.farm/fullpriceDOTA.json");
            }
            else
            {
                var res = Task.Run(() => GetXHR("https://api.swap.gg/prices/433850"));
                swapItems = SwapItems.FromJson(res.Result);
                GetLootItems("https://loot.farm/fullpriceH1Z1.json");
            }

            //var res2 = Task.Run(() => GetXHR("https://dota.money/570/load_bots_inventory"));
            //Task.Run(() => GetXHRDeals());
            dataGridView1.AutoGenerateColumns = true;

            var l = swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round((x.Price.Value * 0.01) + (SWAPPerc * (x.Price.Value * 0.01)), 2),
                PriceLoot = Math.Round((t.Price * 0.01) - (LOOTPerc * (t.Price * 0.01)), 2)
            }).ToList();

            dataGridView1.DataSource = l;
        }
 
        private void GetLootItems(string URL)
        {
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    var json = webClient.DownloadString(URL);

                    lootItems = LootItems.FromJson(json);
                }
            }
            catch (Exception) { }
        }

        private async Task<string> GetXHR(string URL)
        {
            string responseBody = "";
            
            try
            {
                HttpResponseMessage response = await client.GetAsync(URL);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException) { }
            return string.Empty;
        }
        private async Task GetXHRDeals()
        {
            var values = new Dictionary<string, string>
            {
               { "appid", "570" }
            };

            var content = new FormUrlEncodedContent(values);
            client.DefaultRequestHeaders.Add("content-type", "application /json");
            client.DefaultRequestHeaders.Add("accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("cookie", "__cfduid=d2f9f21cb8db798b61850080116104cad1552734072; _ga=GA1.2.703749747.1552734081; sessionID=sg6vmoq81bat5t1q9kpm3dqjgs; lang=ru; _gid=GA1.2.188995789.1562681437; hideIntro=1; sessionID=sg6vmoq81bat5t1q9kpm3dqjgs");
            var response = await client.PostAsync("https://cs.deals/ajax/botsinventory", content);

            var responseString = await response.Content.ReadAsStringAsync();

            Deals = DealsItems.FromJson(responseString);
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                var l = swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
                {
                    Name = x.MarketName,
                    PriceSwap = Math.Round(comboBox1.SelectedIndex == 0 ? ((x.Price.Value * 0.01) + (SWAPPerc * (x.Price.Value * 0.01))) : ((x.Price.Value * 0.01) + ((SWAPPerc - 0.05) * (x.Price.Value * 0.01))), 2),
                    PriceLoot = Math.Round(comboBox1.SelectedIndex == 1 ? ((t.Price * 0.01) + (LOOTPerc * (t.Price * 0.01))) : ((t.Price * 0.01) - (LOOTPerc * (t.Price * 0.01))), 2),
                    Have1 = x.Stock.Have,
                    Max1 = x.Stock.Max,
                    Have2 = t.Have,
                    Max2 = t.Max
                })
                .Where(x => comboBox1.SelectedIndex == 0 ? 
                (x.Have1 > 0 && x.Have2 < x.Max2) : 
                (x.Have2 > 0 && x.Have1 < x.Max1))
                .Select(x => new { Name = x.Name, Loot = x.PriceLoot, Swap = x.PriceSwap, Perc = (comboBox1.SelectedIndex == 0 ? x.PriceLoot / x.PriceSwap : x.PriceSwap / x.PriceLoot) * 100 - 100 }).OrderByDescending(x => x.Perc).ToList();

                dataGridView2.DataSource = l;
                dataGridView2.Update();
            }
            catch (Exception) { }
        }
    }
}
