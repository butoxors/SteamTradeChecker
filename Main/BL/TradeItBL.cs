using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Main.Support;

namespace Main.BL
{
    public class TradeItBL
    {
        private Task<string> Responce;

        private List<TradeIt> TradeIts = new List<TradeIt>();

        public TradeItBL(string link)
        {
            Responce = Task.Run(() => GetJSONData.GetXHR(link));
            TradeIts = TradeIt.FromJson(Responce.Result);
        }
        public List<Tuple<string, long, long>> GetList()
        {
            List<Tuple<string, long, long>> res = new List<Tuple<string, long, long>>();

            foreach (var l in TradeIts)
            {
                foreach (var item in l.The570.Items)
                {
                    res.Add(Tuple.Create(new string(item.Key.SkipWhile(m => m != '_').Skip(1).ToArray()), item.Value.X, item.Value.P));
                }
            }

            return res;
        }
    }
}
