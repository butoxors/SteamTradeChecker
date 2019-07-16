using Main.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Main.BL
{
    public class SwapBL
    {
        public double Comission { get; set; }

        public SwapItems swapItems;

        public SwapBL()
        {
            swapItems = new SwapItems();
            Comission = Difference.SWAP_OTHER_BUY;
        }

        public void Start(string link)
        {
            var res = Task.Run(() => GetJSONData.GetXHR(link));
            swapItems = SwapItems.FromJson(res.Result);
        }
    }
}
