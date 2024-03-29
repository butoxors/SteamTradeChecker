﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Main.JSON_CLASSES;
//
//    var tradeItRust = TradeItRust.FromJson(jsonString);

namespace Main.JSON_Classes.TradeIt
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class TradeItRust : MainTradeIt
    {
        [JsonProperty("252490")]
        public override GameCode GameCode { get; set; }
    }

    public static class SerializeTradeItRust
    {
        public static string ToJson(this List<TradeItRust> self) => JsonConvert.SerializeObject(self, Main.Support.Converter.Settings);
    }
    public partial class TradeItRust
    {
        public static List<TradeItRust> FromJson(string json) => JsonConvert.DeserializeObject<List<TradeItRust>>(json, Main.Support.Converter.Settings);
    }
}
