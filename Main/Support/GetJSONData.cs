using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Main.Support
{
    public static class GetJSONData
    {
        /// <summary>
        /// Client
        /// </summary>
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Get XHR format responce from URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns>JSON format string</returns>
        public static async Task<string> GetXHR(string URL)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(URL);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) { return e.Message; }
        }
        /// <summary>
        /// Download JSON file from URL
        /// </summary>
        /// <param name="URL">file URL</param>
        /// <returns>JSON format string</returns>
        public static string GetLootItems(string URL)
        {
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    return webClient.DownloadString(URL);
                }
            }
            catch (Exception e) { return e.Message; }
        }
        /// <summary>
        /// Get XHR format responce from URL !(Don`t work yet)!
        /// </summary>
        /// <returns>JSON format string</returns>
        public static async Task<string> GetXHRDeals()
        {
            var values = new Dictionary<string, string>
            {
               { "appid", "570" }
            };

            var content = new FormUrlEncodedContent(values);
            client.DefaultRequestHeaders.Add("content-type", "application /json");
            client.DefaultRequestHeaders.Add("accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("cookie", "__cfduid=d2f9f21cb8db798b61850080116104cad1552734072; _ga=GA1.2.703749747.1552734081; sessionID=sg6vmoq81bat5t1q9kpm3dqjgs; lang=ru; _gid=GA1.2.188995789.1562681437; hideIntro=1; sessionID=sg6vmoq81bat5t1q9kpm3dqjgs");
            var response = await client.PostAsync("https://cs.deals/ajax/botsinventory", content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
