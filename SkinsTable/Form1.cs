using Main;
using Main.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkinsTable
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CookieContainer cookieContainer = new CookieContainer();

            cookieContainer.Add(new Cookie("__cfduid", "d2d0a0ea3af455afb863b44da7e84522d1552734078", string.Empty, ".skins-table.xyz"));
            cookieContainer.Add(new Cookie("_ga", "GA1.2.1782441241.1552734216", string.Empty, ".skins-table.xyz"));
            cookieContainer.Add(new Cookie("_gid", "GA1.2.1536503649.1565181354", string.Empty, ".skins-table.xyz"));
            cookieContainer.Add(new Cookie("jv_enter_ts_2kd6zCIQva", "1565181347632", string.Empty, ".skins-table.xyz"));
            cookieContainer.Add(new Cookie("jv_visits_count_2kd6zCIQva", "81", string.Empty, ".skins-table.xyz"));
            cookieContainer.Add(new Cookie("PHPSESSID", "000b7usau8uvjh7jh0blfa1uh5", string.Empty, ".skins-table.xyz"));

            var request = (HttpWebRequest)WebRequest.Create("https://skins-table.xyz/prices/ajax.php");

            var postData = "act=names";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = data.Length;
            request.Referer = "https://skins-table.xyz/prices/";
            request.Headers.Add("x-request-with", "XMLHttpRequest");
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //SaveFile.ProcessWrite(responseString, "skins");
        }
    }
}
