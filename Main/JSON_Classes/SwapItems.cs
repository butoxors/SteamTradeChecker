﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Main;
//
//    var itemInfo = ItemInfo.FromJson(jsonString);

namespace Main.JSON_Classes
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Support;

    public partial class SwapItems
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("result")]
        public List<Result> Result { get; set; }

    }

    public partial class Result
    {
        [JsonProperty("marketName")]
        public string MarketName { get; set; }

        [JsonProperty("price")]
        public Price Price { get; set; }

        [JsonProperty("stock")]
        public Stock Stock { get; set; }
    }

    public partial class Price
    {
        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("sides")]
        public Sides Sides { get; set; }

        [JsonProperty("factor")]
        public double Factor { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Sides
    {
        [JsonProperty("user")]
        public long User { get; set; }

        [JsonProperty("bot")]
        public long Bot { get; set; }
    }

    public partial class Stock
    {
        [JsonProperty("have")]
        public long Have { get; set; }

        [JsonProperty("max")]
        public long Max { get; set; }
    }

    public partial class SwapItems
    {
        public static SwapItems FromJson(string json) => JsonConvert.DeserializeObject<SwapItems>(json, Converter.Settings);
    }

    public static class SerializeTrade
    {
        public static string ToJson(this SwapItems self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
