using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Main.Support;
using Main.BL;

namespace Main
{
    public partial class Form1 : Form
    {

        SwapItems swapItems = new SwapItems();
        DealsItems Deals = new DealsItems();

        List<LootItems> lootItems = new List<LootItems>();
        List<DotaMoney> DotaMoneyItems = new List<DotaMoney>();
        //List<TradeIt> TradeIts = new List<TradeIt>();

        TradeItBL tradeItCore;
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        private void MakeRequest(string swap, string loot, string trade)
        {
            var res = Task.Run(() => GetJSONData.GetXHR(swap));
            swapItems = SwapItems.FromJson(res.Result);
            lootItems = LootItems.FromJson(GetJSONData.GetLootItems(loot));
            tradeItCore = new TradeItBL(trade);
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                MakeRequest(Links.SWAP_DOTA, Links.LOOT_DOTA, Links.TRADE_DOTA);
            }
            /*else if (radioButton2.Checked)
            {
                MakeRequest(Links.SWAP_RUST, Links.LOOT_RUST, "");
            }
            else
            {
                MakeRequest(Links.SWAP_H1Z1, Links.LOOT_H1Z1, "");
            }*/

            MakeDataSource();
        }
        private void MakeDataSource()
        {
            var trade = tradeItCore.GetList().Select(x => new { Name = x.Item1, Count = x.Item2, Price = x.Item3 * 0.01 }).ToList();

            var l = swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round((x.Price.Value * 0.01) + (Difference.SWAPPercSell * (x.Price.Value * 0.01)), 2),
                CountSwap = $"Have:{x.Stock.Have}/Max:{x.Stock.Max}",
                PriceLoot = Math.Round((t.Price * 0.01) - (Difference.LOOTPerc * (t.Price * 0.01)), 2),
                CountLoot = $"Have:{t.Have}/Max:{t.Max}"
            }).ToList();

            var k = l.Join(trade, a => a.Name, w => w.Name, (a, w) => new
            {
                Name = a.Name,
                PriceSwap = a.PriceSwap,
                CountSwap = a.CountSwap,
                PriceLoot = a.PriceLoot,
                CountLoot = a.CountLoot,
                Tradeit_Price = w.Price,
                Tradeit_Count = w.Count
            }).ToList();

            dataGridView1.DataSource = k;
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
