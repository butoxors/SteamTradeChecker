using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Main.JSON_Classes.TradeIt;
using Main.Support;

namespace Main.BL
{
    public class TradeItBL
    {
        private Task<string> Responce;

        public List<Tuple<string, long, double>> res;


        public TradeItBL(string link)
        {
            Responce = Task.Run(() => GetJSONData.GetXHR(link));

            res = new List<Tuple<string, long, double>>();

            var d = TradeItDota.FromJson(Responce.Result);
            var r = TradeItRust.FromJson(Responce.Result);
            var h = TradeItH1Z1.FromJson(Responce.Result);

            if (link == Links.TRADE_DOTA)
            {
                foreach (var l in d)
                {
                    foreach (var item in l.GameCode.Items)
                    {
                        res.Add(Tuple.Create(new string(item.Key.SkipWhile(m => m != '_').Skip(1).ToArray()), item.Value.X, item.Value.P * 0.01));
                    }
                }
                SaveFile.ProcessWrite(Responce.Result, "tradeit");
            }
            else if (link == Links.TRADE_RUST)
                foreach (var l in r)
                {
                    foreach (var item in l.GameCode.Items)
                    {
                        res.Add(Tuple.Create(new string(item.Key.SkipWhile(m => m != '_').Skip(1).ToArray()), item.Value.X, item.Value.P * 0.01));
                    }
                }
            else
                foreach (var l in h)
                {
                    foreach (var item in l.GameCode.Items)
                    {
                        res.Add(Tuple.Create(new string(item.Key.SkipWhile(m => m != '_').Skip(1).ToArray()), item.Value.X, item.Value.P * 0.01));
                    }
                }

        }
    }
}
