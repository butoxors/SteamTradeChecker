using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class Form1 : Form
    {
        public SwapItems swapItems = null;
        List<LootItems> lootItems = new List<LootItems>();
        readonly HttpClient client = new HttpClient();
        DealsItems Deals = new DealsItems();

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
            //textBox1.Text = res2.Result;
            //Task.Run(() => GetXHRDeals());

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = RowGenerator();
        }
        private DataTable RowGenerator()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Name", typeof(string)),
                new DataColumn("Swap", typeof(double)),
                //new DataColumn("Deals", typeof(double)),
                new DataColumn("Have(Swap)", typeof(int)),
                new DataColumn("Max(Swap)", typeof(int)),
                new DataColumn("Loot", typeof(double)),
                new DataColumn("Have(Loot)", typeof(int)),
                new DataColumn("Max(Loot)", typeof(int)),
            });
            var l = swapItems.Result.Where(x => x.Price.Value > 0).OrderByDescending(x => x.Price.Value);
            foreach (var i in l)
            {
                var loot = lootItems.Where(x => x.Name == i.MarketName).ToList();
                //var deals = Deals.Response.Items.The570.Where(x => x.C == i.MarketName).ToList();

                if (loot.Count != 0 && i.Stock.Have < i.Stock.Max && loot[0].Have < loot[0].Max && i.Stock.Have != 0 && loot[0].Have != 0)
                {
                    var row = dt.NewRow();
                    row["Name"] = i.MarketName;
                    row["Swap"] = Math.Round(i.Price.Value * 0.01 , 2);
                    row["Loot"] = Math.Round(loot[0].Price * 0.01, 2);
                    //row["Deals"] = Math.Round(deals[0].I * 0.01, 2);
                    row["Have(Swap)"] = i.Stock.Have;
                    row["Max(Swap)"] = i.Stock.Have;
                    row["Have(Loot)"] = loot[0].Have;
                    row["Max(Loot)"] = loot[0].Max;
                    dt.Rows.Add(row);
                }
            }
            return dt;
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
                DataTable dt = new DataTable();

                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("Name", typeof(string)),
                    new DataColumn("Swap", typeof(double)),
                    new DataColumn("Loot", typeof(double)),
                    new DataColumn("%", typeof(double))
                });

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    var x = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                    var y = Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value);
                    var have1 = Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                    var max1 = Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value);
                    var have2 = Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
                    var max2 = Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value);

                    if (have1 > 0 || (have1 < max1 || comboBox1.SelectedIndex == 0) && have2 > 0 && (have2 < max2 || comboBox1.SelectedIndex == 0))
                    {
                        var row = dt.NewRow();

                        row["Name"] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        row["Swap"] = Math.Round(x, 2);
                        row["Loot"] = Math.Round(y, 2);
                        row["%"] = 100 - Math.Round((x / y) * 100, 2);

                        dt.Rows.Add(row);
                    }
                }

                dataGridView2.DataSource = dt;
                dataGridView2.Update();
            }
            catch (Exception) { }
        }
    }
}
