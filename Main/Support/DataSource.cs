using Main.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main.Support
{
    public static class DataSource
    {
        public static List<string> Alerts = new List<string>();

        public static object GetDataSource(SwapBL Swap, List<LootItems> lootItems)
        {
            var l = Swap.swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round((x.Price.Value * 0.01) + (Swap.Comission * (x.Price.Value * 0.01)), 2),
                HaveMaxS = $"{x.Stock.Have}/{x.Stock.Max}",
                PriceLoot = Math.Round((t.Price * 0.01) + (Difference.LOOTPerc * (t.Price * 0.01)), 2),
                HaveMaxL = $"{t.Have}/{t.Max}"
            }).Where(x => Convert.ToInt32(x.HaveMaxL[0].ToString()) != 0 || Convert.ToInt32(x.HaveMaxS[0].ToString()) != 0).ToList();

            return l;
        }

        public static object MakeTradeTable(bool MakeAlert, TradeItBL tradeItCore, SwapBL Swap, List<LootItems> lootItems, int i)
        {
            var l = Swap.swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round(i == 0 ? ((x.Price.Value * 0.01) + (Swap.Comission * (x.Price.Value * 0.01))) : x.Price.Value * 0.01, 2),
                PriceLoot = Math.Round(i == 1 ? ((t.Price * 0.01) + (Difference.LOOTPerc * (t.Price * 0.01))) : ((t.Price * 0.01) - ((Difference.LOOTPerc + 0.03) * (t.Price * 0.01))), 2),
                Have1 = x.Stock.Have,
                Max1 = x.Stock.Max,
                Have2 = t.Have,
                Max2 = t.Max
            })
                .Where(x => i == 0 ?
                (x.Have1 > 0 && x.Have2 < x.Max2 && x.PriceSwap > 0.01 && x.PriceSwap <= x.PriceLoot) :
                (x.Have2 > 0 && x.Have1 < x.Max1 && x.PriceLoot > 0.01 && x.PriceSwap >= x.PriceLoot))
                .Select(x => new
                {
                    Name = x.Name,
                    Loot = x.PriceLoot,
                    Swap = x.PriceSwap,
                    Perc = (i == 0 ? x.PriceLoot / x.PriceSwap : x.PriceSwap / x.PriceLoot) * 100.0 - 100
                })
                .OrderByDescending(x => x.Perc).ToList();

            /*var k = tradeItCore.GetList().Where(x => x.Item2 > 0).Join(l, a => a.Item1, d => d.Name, (a, d) => new
            {
                Name = a.Item1,
                Loot = d.Loot + (d.Loot * Difference.LOOTPerc),
                Swap = d.Swap + (d.Swap * Swap.Comission),
                Trade = a.Item3,
                PercSwap = Math.Round(d.Swap / a.Item3, 2) * 100.0 - 100,
                PercLoot = Math.Round(d.Loot / a.Item3, 2) * 100.0 - 100
            }).Where(x => x.Trade >= x.Loot || x.Trade >= x.Swap).OrderByDescending(x => x.Trade).Distinct().ToList();

            if (i == 2)
            {
                return k;
            }*/

            if (MakeAlert)
            {
                var s = l.Select(a => new { Name = a.Name}).ToList();
                foreach (var x in s)
                {
                    if (!Alerts.Contains(x.Name))
                    {
                        Alerts.Add(x.Name);
                    }
                }
            }

            return l;
        }
    }
}
