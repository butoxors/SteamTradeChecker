using Main.Log;
using System;

namespace Main.Exceptions
{
    public class SiteNotFoundException : Exception
    {
        public SiteNotFoundException(string msg) : base(msg)
        {
            CLog.Print(LogType.FATAL, msg);
        }
    }
}
