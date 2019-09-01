using Main.Log;
using System;

namespace Main.Exceptions
{
    public class SteamAuthException : Exception
    {
        public SteamAuthException(string msg) : base(msg)
        {
            CLog.Print(LogType.WARNING, msg);
        }
    }
}
