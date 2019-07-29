using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamApp
{
    class Program
    {
        static SteamClient steamClient;

        static CallbackManager manager;

        static SteamUser steamUser;

        static string user, pass;

        static void Main(string[] args)
        {
            Console.Write("Username: ");
            user = Console.ReadLine();

            Console.Write("Password: ");
            pass = Console.ReadLine();

            SteamLogIn();
        }

        static void SteamLogIn()
        {
            steamClient = new SteamClient();

            manager = new CallbackManager(steamClient);

            steamUser = steamClient.GetHandler<SteamUser>();

            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
        }

        private static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.JobID.)
        }

    }
}
