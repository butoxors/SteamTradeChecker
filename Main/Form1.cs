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

        public Form1()
        {
            InitializeComponent();
            
        }

        private async void btnCheck_Click(object sender, EventArgs e)
        {
            await Task.Run(() => GetXHR());
            GetLootItems("https://loot.farm/fullpriceDOTA.json");
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
                if (loot.Count != 0 && i.Stock.Have < i.Stock.Max && loot[0].Have < loot[0].Max && i.Stock.Have != 0 && loot[0].Have != 0)
                {
                    var row = dt.NewRow();
                    row["Name"] = i.MarketName;
                    row["Swap"] = Math.Round(i.Price.Value * 0.01, 2);
                    row["Loot"] = Math.Round(loot[0].Price * 0.01, 2);
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

        private async Task GetXHR()
        {
            string responseBody = "";
            
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://api.swap.gg/prices/570");
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

                swapItems = SwapItems.FromJson(responseBody);                
            }
            catch (HttpRequestException) { }
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

                    if (have1 > 0 || have1 < max1 && have2 > 0 && have2 < max2)
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
