using Main.BL;
using Main.JSON_Classes;
using Main.JSON_Classes.LootFarm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Main.Support
{
    public static class DataSource
    {
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

        public static List<Tuple<string, double, double, double, long>> MakeTradeTable(SwapBL Swap, List<LootItems> lootItems, int i, double from, double to, TradeItBL tradeItCore = null)
        {
            var l = Swap.swapItems.Result.Join(lootItems, x => x.MarketName, t => t.Name, (x, t) => new
            {
                Name = x.MarketName,
                PriceSwap = Math.Round(i == 0 ? ((x.Price.Value * 0.01) + (Swap.Comission * (x.Price.Value * 0.01))) : x.Price.Value * 0.01, 2),
                PriceLoot = Math.Round(i == 1 ? ((t.Price * 0.01) + (Difference.LOOTPerc * (t.Price * 0.01))) : ((t.Price * 0.01) - ((Difference.LOOTPerc + 0.03) * (t.Price * 0.01))), 2),
                Have1 = x.Stock.Have,
                Max1 = x.Stock.Max,
                Have2 = t.Have, //loot
                Max2 = t.Max //loot
            })
                .Where(x => i == 0 ?
                (x.Have1 > 0 && x.Have2 < x.Max2) :
                (x.Have2 > 0 && x.Have1 < x.Max1))
                .Select(x => new
                {
                    Name = x.Name,
                    Loot = x.PriceLoot,
                    Swap = x.PriceSwap,
                    Perc = (i == 0 ? x.PriceLoot / x.PriceSwap : x.PriceSwap / x.PriceLoot) * 100.0 - 100,
                    SwapCount = x.Max1 - x.Have1,
                    LootCount = x.Max2 - x.Have2
                }).
                Where(x => x.Perc > from && x.Perc < to)
                .OrderByDescending(x => x.Perc).ToList();

            List<Tuple<string, double, double, double, long>> w = new List<Tuple<string, double, double, double, long>>();

            foreach (var s in l)
            {
                if (i == 0)
                    w.Add(Tuple.Create(s.Name, s.Loot, s.Swap, s.Perc, s.LootCount));
                else if (i == 1)
                    w.Add(Tuple.Create(s.Name, s.Loot, s.Swap, s.Perc, s.SwapCount));
            }
            return w;
        }
    }
}
