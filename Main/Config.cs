using Main.CookieDef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Main
{
    public class Config
    {
        private Dictionary<string, List<Cookies>> myCookies = new Dictionary<string, List<Cookies>>();

        private readonly string HOME_CONFIG = Directory.GetCurrentDirectory() + @"\Configs\";
        private readonly string HOME_LOG = Directory.GetCurrentDirectory() + @"\Log\";
        private string[] configs = { "loot", "swap" };

        public Config()
        {
            foreach (var s in configs)
            {
                if (!File.Exists($"{HOME_CONFIG}{s}.cfg"))
                {
                    File.Create($"{HOME_LOG}{s}.log");
                }
                try
                {
                    Log(s, $"Try to read cookie file from {s}");

                    var str = File.ReadAllText($@"{HOME_CONFIG}{s}.cfg");

                    if (str == null || str.Length == 0)
                        throw new NullReferenceException($"Can`t read configuration on {HOME_CONFIG}{s}");

                    myCookies[s] = Cookies.FromJson(str);

                    Log(s, $"Cookies {s} was readed successly!");
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
            try
            {
                if (configs.Contains(site))
                {
                    CookieContainer c = new CookieContainer();

                    foreach (var s in myCookies[site])
                    {
                        c.Add(new Cookie(s.Name, s.Value, s.Path, s.Domain));
                    }

                    if (c.Count == 0)
                        throw new NullReferenceException($"Can`t read cookies! They are empty! Check file : {HOME_CONFIG}{site}.cfg");

                    return c;
                }
                else
                {
                    throw new ArgumentNullException("site", "Current site was not found in database! Try use {loot, swap}");
                }
            }catch(Exception ex)
            {
                Log(site, ex.Message);
                return null;
            }
        }

        public IEnumerable<Cookie> GetSteamCookies()
        {
            var cookies = new List<Cookie>();

            try
            {
                Log("steam", "Try to read Steam cookie...");
                var str = File.ReadAllText(Directory.GetCurrentDirectory() + "/Configs/Steam/cookies.cfg");

                if (str == null || str.Length == 0)
                    throw new NullReferenceException($"Can`t add cookie from 'cookies.cfg' - file is empty!");

                var data = SteamCookie.FromJson(str);

                foreach (var d in data)
                {
                    cookies.Add(new Cookie(d.Name, d.Value, d.Path, d.Domain));
                }

                Log("steam", "Steam cookie readed succefully!");
            }catch(Exception ex)
            {
                Log("steam", ex.Message);
            }

            return cookies;
        }

        public string ReadSteamKey()
        {
            try
            {
                Log("steam", "Try to read Steam API key...");

                if (!File.Exists(Directory.GetCurrentDirectory() + "/Configs/Steam/key.cfg"))
                    throw new FileNotFoundException("Error to load steam key, file not found", "key.cfg");

                return File.ReadAllText(Directory.GetCurrentDirectory() + "/Configs/Steam/key.cfg");

            }
            catch(Exception ex)
            {
                Log("steam", ex.Message);

                return null;
            }
        }

        private void Log(string s, string msg)
        {
            File.AppendAllText(Directory.GetCurrentDirectory() + $@"\Log\{s}.log", $"[{DateTime.Now}] - {msg} {s}.\r\n");
        }
    }
}
