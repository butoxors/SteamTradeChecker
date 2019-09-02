using Main.CookieDef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Main.Log;
using Main.Exceptions;

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
                try
                {
                    var str = File.ReadAllText($@"{HOME_CONFIG}{s}.cfg");

                    if (str == null || str.Length == 0)
                        throw new ReadConfigException($"Can`t read configuration on { HOME_CONFIG }{ s}");

                    myCookies[s] = Cookies.FromJson(str);

                    if (myCookies[s] == null)
                        throw new ReadCookieException($"Can`t read cookies from { HOME_CONFIG }{ s}");

                }
                catch
                {
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
                        throw new ReadCookieException($"Can`t read cookies! They are empty! Check file : {HOME_CONFIG}{site}.cfg");

                    return c;
                }
                else
                {
                    throw new SiteNotFoundException($"Current site: {site} was not found in database! Try use 'loot, swap'");
                }
            }catch
            {
                return null;
            }
        }

        public IEnumerable<Cookie> GetSteamCookies()
        {
            var cookies = new List<Cookie>();
            string steamCookies = $@"{HOME_CONFIG}/Steam/cookies.cfg";
            try
            {
                if (File.Exists(steamCookies))
                {
                    var str = File.ReadAllText(steamCookies);
                        

                    if (str == null || str.Length == 0)
                        throw new ReadCookieException("Can`t add cookie from 'cookies.cfg' - file is empty!");

                    var data = SteamCookie.FromJson(str);

                    foreach (var d in data)
                    {
                        cookies.Add(new Cookie(d.Name, d.Value, d.Path, d.Domain));
                    }
                }
                else
                {
                    throw new FileNotFoundException($@"File {steamCookies} not found!");
                }

            }catch(FileNotFoundException ex)
            {
                CLog.Print(LogType.FATAL, ex.Message);
            }

            return cookies;
        }

        public string ReadSteamKey()
        {
            string steamKey = $@"{HOME_CONFIG}/Steam/key.cfg";

            try
            {

                if (!File.Exists(steamKey))
                    throw new FileNotFoundException("Error to load steam key, file not found", "key.cfg");
                var text = File.ReadAllText(steamKey);

                if (text == null || text == string.Empty)
                    throw new ReadConfigException("Can`t read Steam api key, because file is empty!");

                return text;

            }
            catch(FileNotFoundException ex)
            {
                CLog.Print(LogType.FATAL, ex.Message);

                return null;
            }
        }

    }
}
