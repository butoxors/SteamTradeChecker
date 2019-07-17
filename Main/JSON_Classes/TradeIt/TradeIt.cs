using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.JSON_Classes.TradeIt
{
    public partial class MainTradeIt
    {
        [JsonProperty("570")]
        public virtual GameCode GameCode { get; set; }

        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class GameCode
    {
        [JsonProperty("items")]
        public Dictionary<string, Item> Items { get; set; }

        [JsonProperty("gems")]
        public List<Gem> Gems { get; set; }
    }

    public partial class Gem
    {
        [JsonProperty("l")]
        public string L { get; set; }

        [JsonProperty("n")]
        public string N { get; set; }

        [JsonProperty("t")]
        public string T { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("d")]
        public List<Dd> Dd { get; set; }

        /// <summary>
        /// Price to bye
        /// </summary>
        [JsonProperty("p")]
        public long P { get; set; }

        [JsonProperty("a")]
        public long A { get; set; }
        /// <summary>
        /// Count of items
        /// </summary>
        [JsonProperty("x")]
        public long X { get; set; }

        [JsonProperty("q")]
        public long Q { get; set; }

        [JsonProperty("hi")]
        public long Hi { get; set; }
    }

    public partial class Dd
    {
        [JsonProperty("i")]
        public string I { get; set; }
    }

}
