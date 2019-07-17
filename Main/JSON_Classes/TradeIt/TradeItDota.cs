﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Main;
//
//    var tradeit = Tradeit.FromJson(jsonString);

namespace Main.JSON_Classes.TradeIt
{
    using System.Collections.Generic;
    using Support;
    using Newtonsoft.Json;

    public partial class TradeItDota : MainTradeIt
    {

    }
    
    public static class SerializeTradeIt
    {
        public static string ToJson(this List<TradeItDota> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
    public partial class TradeItDota
    {
        public static List<TradeItDota> FromJson(string json) => JsonConvert.DeserializeObject<List<TradeItDota>>(json, Main.Support.Converter.Settings);
    }
}
