using Main.CookieDef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Main
{
    public class Config
    {
        private Dictionary<string, List<Cookies>> myCookies = new Dictionary<string, List<Cookies>>();

        private string[] configs = { "loot", "swap" };

        public Config()
        {
            foreach (var s in configs)
            {
                if (!File.Exists(Directory.GetCurrentDirectory() + @"\Configs\" + s + ".cfg"))
                {
                    File.Create(Directory.GetCurrentDirectory() + $@"\Log\{s}.log");
                }
                try
                {
                    Log(s, "Try to read cookie file from");
                    myCookies[s] = Cookies.FromJson(File.ReadAllText($@"{Directory.GetCurrentDirectory()}\Configs\{s}.cfg"));
                    Log(s, "Read success!");
                }
                catch (Exception ex)
                {
                    Log(s, "Error loading cookie form: " + ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public CookieContainer GenerateCookieContainer(string site)
        {
            if (configs.Contains(site)) {

                CookieContainer c = new CookieContainer();

                foreach (var s in myCookies[site])
                {
                    c.Add(new Cookie(s.Name, s.Value, s.Path, s.Domain));
                }
                return c;
            }
            else
            {
                return null;
            }
        }

        private void Log(string s, string msg)
        {
            File.AppendAllText(Directory.GetCurrentDirectory() + $@"\Log\{s}.log", $"[{DateTime.Now}] - {msg} {s}.\r\n");
        }
    }
}
