using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Main.Support;
using Main.BL;
using System.Threading;

namespace Main
{
    public partial class Form1 : Form
    {

        DealsItems Deals = new DealsItems();
        SwapBL SwapBL;
        List<LootItems> lootItems = new List<LootItems>();
        List<DotaMoney> DotaMoneyItems = new List<DotaMoney>();
        //List<TradeIt> TradeIts = new List<TradeIt>();

        TradeItBL tradeItCore;

        private bool TimerOn;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
            radioButton1.Checked = true;
            SwapBL = new SwapBL();
            TimerOn = false;
        }   

        private void MakeRequest(string swap, string loot, string trade)
        {
            SwapBL.Start(swap);
            lootItems = LootItems.FromJson(GetJSONData.GetLootItems(loot));
            //tradeItCore = new TradeItBL(trade);
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                MakeRequest(Links.SWAP_DOTA, Links.LOOT_DOTA, Links.TRADE_DOTA);
                SwapBL.Comission = Difference.SWAP_DOTA_BUY;
            }
            else if (radioButton2.Checked)
            {
                MakeRequest(Links.SWAP_RUST, Links.LOOT_RUST, "");
            }
            else
            {
                MakeRequest(Links.SWAP_H1Z1, Links.LOOT_H1Z1, "");
            }

            MakeDataSource();
        }
        private void MakeDataSource()
        {
            /*var l = swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round((x.Price.Value * 0.01) + (swapItems.Comission * (x.Price.Value * 0.01)), 2),
                HaveMaxS = $"{x.Stock.Have}/{x.Stock.Max}",
                PriceLoot = Math.Round((t.Price * 0.01) + (Difference.LOOTPerc * (t.Price * 0.01)), 2),
                HaveMaxL = $"{t.Have}/{t.Max}"
            }).Where(x => Convert.ToInt32(x.HaveMaxL[0].ToString()) != 0 || Convert.ToInt32(x.HaveMaxS[0].ToString()) != 0).ToList();*/

            /*var k = l.Join(trade, a => a.Name, w => w.Name, (a, w) => new
            {
                Name = a.Name,
                PriceSwap = a.PriceSwap,
                CountSwap = a.CountSwap,
                PriceLoot = a.PriceLoot,
                CountLoot = a.CountLoot,
                Tradeit_Price = w.Price,
                Tradeit_Count = w.Count
            }).ToList();*/
            dataGridView1.DataSource = DataSource.GetDataSource(SwapBL, lootItems);
        }
        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                var l = DataSource.MakeTradeTable(TimerOn, tradeItCore, SwapBL, lootItems, comboBox1.SelectedIndex);

                dataGridView2.DataSource = l;
                dataGridView2.Update();
            }
            catch (Exception x) { MessageBox.Show(x.Message); }
        }

        private void SelectInTradeIt(string Name)
        {
            //var trade = tradeItCore.GetList().Where(x => x.Item1 == Name).Select(x => new { Name = x.Item1, Max = x.Item2, Price = x.Item3 - (x.Item3 * Difference.TRADEPers) }).Distinct().OrderBy(x => x.Price).ToList();
            //dataGridView3.DataSource = trade;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView d = (DataGridView)sender;
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    string val = d.Rows[e.RowIndex].Cells[0].Value.ToString();
                    SelectInTradeIt(val);
                    Clipboard.SetText(val);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (timerAutoMode.Enabled)
            {
                timerAutoMode.Stop();
                timerAutoMode.Enabled = false;
                TimerOn = false;
            }
            else
            {
                timerAutoMode.Enabled = true;
                timerAutoMode.Start();
                TimerOn = true;
            }
        }

        private void timerAutoMode_Tick(object sender, EventArgs e)
        {
            TimerOn = true;
            btnCheck.PerformClick();
            btnCalc.PerformClick();
            foreach (var x in DataSource.Alerts)
            {
                Thread t = new Thread(new ThreadStart(ShowAlertForm));
                t.Start();
            }
        }

        private void ShowAlertForm()
        {
            int count = DataSource.Alerts.Count;
            Alert a = new Alert(DataSource.Alerts.First(), new System.Drawing.Point(Screen.PrimaryScreen.Bounds.Width - 250, 50 * (count - DataSource.Alerts.Count) + 10));
            a.Show();
        }
    }
}
