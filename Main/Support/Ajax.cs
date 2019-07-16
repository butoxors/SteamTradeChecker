using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Main.Support
{
    public static class Ajax
    {
        public static async void MakeRequest()
        {
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
                { "act", "names" }
            });
            client.DefaultRequestHeaders.Referrer = new Uri("https://skins-table.xyz/prices/ajax.php");

            var resp = await client.PostAsync("https://skins-table.xyz/prices/ajax.php", content);
            var repsStr = await resp.Content.ReadAsStringAsync();
            SaveFile.ProcessWrite(repsStr, "responce");
        }
    }
}
