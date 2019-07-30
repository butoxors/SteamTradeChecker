using Main.BL;
using Main.JSON_Classes;
using Main.JSON_Classes.DotaMoney;
using Main.JSON_Classes.LootFarm;
using Main.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class Form1 : Form
    {

        DealsItems Deals = new DealsItems();
        SwapBL SwapBL;
        List<LootItems> lootItems = new List<LootItems>();
        List<DotaMoneyJson> DotaMoneyItems = new List<DotaMoneyJson>();
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
                double fromP = Convert.ToDouble(from.Text);
                double toP = Convert.ToDouble(to.Text);
                if (tradeItCore == null || SwapBL == null || lootItems == null)
                    btnCheck.PerformClick();

                List<Tuple<string, double, double, double, long>> l = DataSource.MakeTradeTable(tradeItCore, SwapBL, lootItems, comboBox1.SelectedIndex, fromP, toP);

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

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                double fromP = Convert.ToDouble(from.Text);
                double toP = Convert.ToDouble(to.Text);
                var r = DataSource.MakeTradeTable(tradeItCore, SwapBL, lootItems, comboBox1.SelectedIndex, fromP, toP);
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
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://loot.farm/login_data.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.Host = "loot.farm";

            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("_ga", "GA1.2.363365182.1552735823") { Domain = request.Host });
            cookieContainer.Add(new Cookie("_fbp", "fb.1.1558441183960.1908168188") { Domain = request.Host });
            cookieContainer.Add(new Cookie("_gid", "GA1.2.380863218.1563038872") { Domain = request.Host });
            cookieContainer.Add(new Cookie("noCancelScam", "1") { Domain = request.Host });
            cookieContainer.Add(new Cookie("__tawkuuid", "e::loot.farm::yyDXu / 0fgsfBerohz6J87qNFs / DxaseqxdkdT5bpu4PQxl5U4q5DTwuz9roh4jZR::2") { Domain = request.Host });
            cookieContainer.Add(new Cookie("lang", "ru") { Domain = request.Host });
            cookieContainer.Add(new Cookie("currency", "USD") { Domain = request.Host });
            cookieContainer.Add(new Cookie("PHPSESSID", "c6b4ad00551255440906928cad35b508") { Domain = request.Host });
            //cookieContainer.Add(new Cookie(@"_gat_UA - 2579492 - 4", "1") { Domain = request.Host });
            cookieContainer.Add(new Cookie("TawkConnectionTime", "0") { Domain = request.Host });

            request.CookieContainer = cookieContainer;

            #region COOKIE
            /*_ga = GA1.2.363365182.1552735823;
            _fbp = fb.1.1558441183960.1908168188;
            _gid = GA1.2.380863218.1563038872;
            noCancelScam = 1;
            __tawkuuid = e::loot.farm::yyDXu / 0fgsfBerohz6J87qNFs / DxaseqxdkdT5bpu4PQxl5U4q5DTwuz9roh4jZR::2;
            lang = ru;
            currency = USD;
            PHPSESSID = c6b4ad00551255440906928cad35b508;
            _gat_UA - 2579492 - 4 = 1;
            TawkConnectionTime = 0*/
            #endregion

            request.Referer = "https://loot.farm/ru/account.html";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "ru,en-US;q=0.9,en;q=0.8,uk;q=0.7");

            WebResponse responce = request.GetResponse();

        }

    }
}
