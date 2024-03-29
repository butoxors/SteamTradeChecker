﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Main.JSON_CLASSES;
//
//    var account = Account.FromJson(jsonString);

using System;
using System.Collections.Generic;

using System.Globalization;
using Main.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Main.JSON_Classes.LootFarm
{ 
    public partial class LootAccount
    {
        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("balance")]
        public long Balance { get; set; }

        [JsonProperty("shares")]
        public long Shares { get; set; }

        [JsonProperty("tradeURL")]
        public string TradeUrl { get; set; }

        [JsonProperty("bonus")]
        public long Bonus { get; set; }

        [JsonProperty("auth")]
        public string Auth { get; set; }

        [JsonProperty("avatar")]
        public Uri Avatar { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("atrade")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Atrade { get; set; }

        [JsonProperty("rcount")]
        public long Rcount { get; set; }
    }

    public partial class LootAccount
    {
        public static LootAccount FromJson(string json) => JsonConvert.DeserializeObject<LootAccount>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this LootAccount self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
