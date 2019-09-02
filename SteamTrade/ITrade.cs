using SteamKit2;
using SteamTrade.TradeOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamTrade
{
    public interface ITrade
    {
        SteamID SteamID { get; set; }
        bool IsWorking { get; set; }

        event EventHandler<Offer> NewOffer;
        event EventHandler<Offer> OfferAccepted;
        event EventHandler<Offer> OfferDeclined;

        bool Autenticate();
        bool CandelOffer(Offer offer);
        bool DeclineOffer(Offer offer);

        GenericInventory GetInventory(ulong targetId, int appId = 570, int context = 2);
        OffersResponse GetTradeOffers();

        string SendOfferWithToken(List<GenericInventory.Item> outItems, List<GenericInventory.Item> theirItems, string token, string message);
        string SendOffer(List<GenericInventory.Item> outItems, List<GenericInventory.Item> theirItems, int accountIdOther, string message);
    }
}
