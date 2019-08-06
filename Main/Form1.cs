using Main.BL;
using Main.JSON_Classes;
using Main.JSON_Classes.DotaMoney;
using Main.JSON_Classes.LootFarm;
using Main.JSON_Classes.Swap;
using Main.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamTrade;

namespace Main
{
    public partial class Form1 : Form
    {

        SwapBL SwapBL = new SwapBL();
        List<LootItems> lootItems = new List<LootItems>();
        List<DotaMoneyJson> DotaMoneyItems = new List<DotaMoneyJson>();

        Config cfg;

        MainTrade Trade;

        private int NewOffers = 0;
        private int AcceptedOffers = 0;

        private EventHandler Offer;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
            radioButton1.Checked = true;

            cfg = new Config();

            SetBalance();
            StartTrade();
        }

        private async void StartTrade()
        {
            Offer += (s, e) =>
            {
                toolStripStatusLabel6.Text = $"Offer status: {NewOffers}/{AcceptedOffers}";
            };
            await Task.Run(() =>
            {
                Trade = new MainTrade(cfg.GetSteamCookies(), cfg.ReadSteamKey());

                Trade.NewOffer += (e, s) =>
                {
                    NewOffers++;
                    Offer?.Invoke(this, null);
                    SetBalance();
                };
                Trade.OfferAccepted += (s, e) =>
                {
                    AcceptedOffers++;
                    Offer?.Invoke(this, null);
                    SetBalance();
                };
                
                toolStripStatusLabel5.Text = "Steam status: " + (Trade.Autenticate() ? "SUCCESS" : "FAILURE!");
                
            });
        }

        private async void SetBalance()
        {
            await Task.Run(() =>
            {
                var sbalance = Task.Run(() => GetJSONData.MakeCookieRequest(Links.SWAP_BALANCE, cfg.GenerateCookieContainer("swap")));
                toolStripStatusLabel3.Text = $"Swap.gg balance: {SwapBalance.FromJson(sbalance.Result).Result * 0.01}";

                var lbalance = Task.Run(() => GetJSONData.MakeCookieRequest(Links.LOOT_ACCOUNT_URL, cfg.GenerateCookieContainer("loot")));
                toolStripStatusLabel4.Text = $"Loot.farm balance: {LootAccount.FromJson(lbalance.Result).Balance * 0.01}";
            });
        }

        private async void MakeRequest(string swap, string loot)
        {
            Cursor = Cursors.WaitCursor;

            await Task.Run(() =>
            {
                SwapBL.Start(swap);
            });
            await Task.Run(() =>
            {
                lootItems = LootItems.FromJson(GetJSONData.GetLootItems(loot));
            });/*
            await Task.Run(() =>
            {
                tradeItCore = new TradeItBL(trade);
            });
            await Task.Run(() =>
            {
                var res = Task.Run(() => GetJSONData.GetXHR(Links.MONEY_DOTA));
                //SaveFile.ProcessWrite(res.Result, "dotamoney");
                DotaMoneyItems = DotaMoneyJson.FromJson(res.Result);
                
            });*/

            Task.WaitAll();
            toolStripStatusLabel1.Text = "Swap.gg total item: " + SwapBL.swapItems.Result.Count;
            toolStripStatusLabel2.Text = "Loot.farm total item: " + lootItems.Count;
            await Task.Run(() =>
            {
                var s = DataSource.GetDataSource(SwapBL, lootItems);
                if (s != dataGridView1.DataSource)
                    BeginInvoke(new MethodInvoker(() => dataGridView1.DataSource = s));
                
            });
            Cursor = Cursors.Default;

        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                MakeRequest(Links.SWAP_DOTA, Links.LOOT_DOTA);
                SwapBL.Comission = Difference.SWAP_DOTA_BUY;
            }
            else if (radioButton2.Checked)
            {
                MakeRequest(Links.SWAP_RUST, Links.LOOT_RUST);
            }
            else
            {
                MakeRequest(Links.SWAP_H1Z1, Links.LOOT_H1Z1);
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                double fromP = Convert.ToDouble(from.Text);
                double toP = Convert.ToDouble(to.Text);

                if (SwapBL == null || lootItems == null)
                    btnCheck.PerformClick();

                List<Tuple<string, double, double, double, long>> l = DataSource.MakeTradeTable(SwapBL, lootItems, comboBox1.SelectedIndex, fromP, toP);

                dataGridView2.DataSource = l;
                dataGridView2.Columns[0].HeaderText = "Name";
                dataGridView2.Columns[1].HeaderText = "Loot";
                dataGridView2.Columns[2].HeaderText = "Swap";
                dataGridView2.Columns[3].HeaderText = comboBox1.SelectedIndex != 2 ? "Perc" : "Trade";
                dataGridView2.Columns[4].HeaderText = comboBox1.SelectedIndex != 0 ? "SwapCount" : "LootCount";

                dataGridView2.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGridView2.Update();
            }
            catch (Exception x) { MessageBox.Show(x.Message); }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView d = (DataGridView)sender;
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    string val = d.Rows[e.RowIndex].Cells[0].Value.ToString();
                    Clipboard.SetText(val);
                }
            }
        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                double fromP = Convert.ToDouble(from.Text);
                double toP = Convert.ToDouble(to.Text);
                var r = DataSource.MakeTradeTable(SwapBL, lootItems, comboBox1.SelectedIndex, fromP, toP);
                switch (e.ColumnIndex)
                {
                    case 0:
                        r.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                        break;
                    case 1:
                        r.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                        break;
                    case 2:
                        r.Sort((x, y) => y.Item3.CompareTo(x.Item3));
                        break;
                    case 3:
                        r.Sort((x, y) => y.Item4.CompareTo(x.Item4));
                        break;
                }

                dataGridView2.DataSource = r;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Trade.AutoAccepting = checkBox1.Checked;
        }
    }
}
