using Main.Log;
using System;


namespace Main.Exceptions
{
    public class TradeOfferException : Exception
    {
        public TradeOfferException(string msg) : base(msg)
        {
            CLog.Print(LogType.INFO, msg);
        }
    }
}
