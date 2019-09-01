using Main.Log;
using System;

namespace Main.Exceptions
{
    public class ReadConfigException : Exception
    {
        public ReadConfigException(string msg) : base(msg)
        {
            CLog.Print(LogType.FATAL, msg);
        }
    }
}
