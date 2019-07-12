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
using Main.Support;

namespace Main
{
    public partial class Form1 : Form
    {

        SwapItems swapItems = new SwapItems();
        DealsItems Deals = new DealsItems();

        List<LootItems> lootItems = new List<LootItems>();
        List<DotaMoney> DotaMoneyItems = new List<DotaMoney>();
        List<TradeIt> TradeIts = new List<TradeIt>();


        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                var res = Task.Run(() => GetJSONData.GetXHR(Links.SWAP_RUST));
                swapItems = SwapItems.FromJson(res.Result);
                lootItems = LootItems.FromJson(GetJSONData.GetLootItems(Links.LOOT_RUST));
                //res = Task.Run(() => GetXHR("https://dota.money/570/load_bots_inventory")); //TODO : 
                //DotaMoneyItems = DotaMoney.FromJson(res.Result); //TODO : 

            }
            else if (radioButton1.Checked)
            {
                var res = Task.Run(() => GetJSONData.GetXHR(Links.SWAP_DOTA));
                swapItems = SwapItems.FromJson(res.Result);
                lootItems = LootItems.FromJson(GetJSONData.GetLootItems(Links.LOOT_DOTA));
            }
            else
            {
                var res = Task.Run(() => GetJSONData.GetXHR(Links.SWAP_H1Z1));
                swapItems = SwapItems.FromJson(res.Result);
                GetJSONData.GetLootItems(Links.LOOT_RUST);
            }

            //var res2 = Task.Run(() => GetXHR("https://dota.money/570/load_bots_inventory"));
            //Task.Run(() => GetXHRDeals());
            dataGridView1.AutoGenerateColumns = true;

            var l = swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round((x.Price.Value * 0.01) + (Difference.SWAPPercSell * (x.Price.Value * 0.01)), 2),
                PriceLoot = Math.Round((t.Price * 0.01) - (Difference.LOOTPerc * (t.Price * 0.01)), 2)
            }).ToList();

            dataGridView1.DataSource = l;

            // TODO : 
            var tradeit = Task.Run(() => GetJSONData.GetXHR(Links.TRADE_DOTA));
            TradeIts = TradeIt.FromJson(tradeit.Result);

        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                var l = swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
                {
                    Name = x.MarketName,
                    PriceSwap = Math.Round(comboBox1.SelectedIndex == 0 ? ((x.Price.Value * 0.01) + (Difference.SWAPPercSell * (x.Price.Value * 0.01))) : ((x.Price.Value * 0.01) + ((Difference.SWAPPercBuy) * (x.Price.Value * 0.01))), 2),
                    PriceLoot = Math.Round(comboBox1.SelectedIndex == 1 ? ((t.Price * 0.01) + (Difference.LOOTPerc * (t.Price * 0.01))) : ((t.Price * 0.01) - (Difference.LOOTPerc * (t.Price * 0.01))), 2),
                    Have1 = x.Stock.Have,
                    Max1 = x.Stock.Max,
                    Have2 = t.Have,
                    Max2 = t.Max
                })
                .Where(x => comboBox1.SelectedIndex == 0 ? 
                (x.Have1 > 0 && x.Have2 < x.Max2) : 
                (x.Have2 > 0 && x.Have1 < x.Max1))
                .Select(x => new {
                    Name = x.Name,
                    Loot = x.PriceLoot,
                    Swap = x.PriceSwap,
                    Perc = (comboBox1.SelectedIndex == 0 ? x.PriceLoot / x.PriceSwap : x.PriceSwap / x.PriceLoot) * 100 - 100
                })
                .OrderByDescending(x => x.Perc).ToList();

                dataGridView2.DataSource = l;
                dataGridView2.Update();
            }
            catch (Exception) { }
        }
    }
}
