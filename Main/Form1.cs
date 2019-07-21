using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Main.Support;
using Main.BL;
using Main.JSON_Classes.LootFarm;
using Main.JSON_Classes.DotaMoney;
using Main.JSON_Classes;

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

        private bool AutoMode;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
            radioButton1.Checked = true;
            SwapBL = new SwapBL();
            AutoMode = false;
        }   

        private void MakeRequest(string swap, string loot, string trade)
        {
            SwapBL.Start(swap);
            lootItems = LootItems.FromJson(GetJSONData.GetLootItems(loot));

            tradeItCore = new TradeItBL(trade);
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
                MakeRequest(Links.SWAP_RUST, Links.LOOT_RUST, Links.TRADE_RUST);
            }
            else
            {
                MakeRequest(Links.SWAP_H1Z1, Links.LOOT_H1Z1, Links.TRADE_H1Z1);
            }

            var s = DataSource.GetDataSource(SwapBL, lootItems);

            if (s != dataGridView1.DataSource)
                dataGridView1.DataSource = s;

        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                var l = DataSource.MakeTradeTable(tradeItCore, SwapBL, lootItems, comboBox1.SelectedIndex);

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
                AutoMode = false;
            }
            else
            {
                timerAutoMode.Enabled = true;
                timerAutoMode.Start();
                AutoMode = true;
            }
        }

        private void timerAutoMode_Tick(object sender, EventArgs e)
        {
            Alert a = new Alert("Good news!");

            btnCheck.PerformClick();
            btnCalc.PerformClick();
            if (dataGridView2.Rows.Count != 0 && a.WindowState != FormWindowState.Normal)
            {
                a.Show();
            }
        }

    }
}
