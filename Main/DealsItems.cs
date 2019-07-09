﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Main;
//
//    var dealsItems = DealsItems.FromJson(jsonString);

namespace Main
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class DealsItems
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("items")]
        public Items Items { get; set; }

        [JsonProperty("reserved")]
        public List<object> Reserved { get; set; }
    }

    public partial class Items
    {
        [JsonProperty("570")]
        public List<The570> The570 { get; set; }
    }

    public partial class The570
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("c")]
        public string C { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        [JsonProperty("i")]
        public double I { get; set; }
    }

    public partial class DealsItems
    {
        public static DealsItems FromJson(string json) => JsonConvert.DeserializeObject<DealsItems>(json, Main.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this DealsItems self) => JsonConvert.SerializeObject(self, Main.Converter.Settings);
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
