using Main.Log;
using System;

namespace Main.Exceptions
{
    public class ReadCookieException : Exception
    {
        public ReadCookieException(string msg) : base(msg)
        {
            CLog.Print(LogType.FATAL, msg);
        }
    }
}
