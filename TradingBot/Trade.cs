using SteamKit2;
using SteamTrade;
using SteamTrade.TradeOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TradingBot
{
    public class Trade : ITrade
    {
        #region SteamTrade Objects
        private SteamWeb web;
        private TradeOfferWebAPI api;
        private TradeOfferManager manager;
        private OfferSession session;
        #endregion

        #region Private fields
        private IEnumerable<Cookie> cookies;
        private string apiKey;

        private List<Offer> LastReceivedOffersData;
        private List<Offer> LastSentOffersData;
        #endregion

        public Trade(IEnumerable<Cookie> _cookies, string _apiKey)
        {
            apiKey = _apiKey;
            cookies = _cookies;
        }

        #region ITrade
        public SteamID SteamID { get; set; }
        public bool IsWorking { get; set; }

        public event EventHandler<Offer> NewOffer;
        public event EventHandler<Offer> OfferAccepted;
        public event EventHandler<Offer> OfferDeclined;

        public bool Autenticate()
        {
            web = new SteamWeb();
            web.Authenticate(cookies);

            if (!web.VerifyCookies())
                return false;

            api = new TradeOfferWebAPI(apiKey, web);
            manager = new TradeOfferManager(apiKey, web);
            session = new OfferSession(api, web);

            SteamID = new SteamID(76561198043356049);
            TradeOfferCheckingLoop();

            return true;
        }

        private void TradeOfferCheckingLoop()
        {
            var oldOffers = GetTradeOffers();
            LastReceivedOffersData = oldOffers.TradeOffersReceived;
            LastSentOffersData = oldOffers.TradeOffersSent;

            Task.Run(() =>
            {
                IsWorking = true;
                while (IsWorking)
                {
                    Thread.Sleep(5000);
                    var offerResponce = api.GetAllTradeOffers();

                    var received = offerResponce.TradeOffersReceived;
                    var sent = offerResponce.TradeOffersSent;

                    CheckForReceivedOfferChanges(received);
                    CheckForSentOfferChanges(sent);
                }
            });
        }
        private void CheckForSentOfferChanges(List<Offer> offers)
        {
            if (offers != null)
            {
                var newOffers = offers.Where(o => o.TradeOfferState == TradeOfferState.TradeOfferStateActive || o.TradeOfferState == TradeOfferState.TradeOfferStateAccepted);

                foreach (var aOffer in newOffers)
                {
                    var wasRaised = false;
                    foreach (var oldsOffer in LastSentOffersData)
                    {
                        if (aOffer.TradeOfferId == oldsOffer.TradeOfferId)
                        {
                            if (oldsOffer.TradeOfferState == TradeOfferState.TradeOfferStateAccepted)
                                wasRaised = true;
                        }
                        if (!wasRaised)
                            //AcceptOffer(aOffer);
                            OfferAccepted?.Invoke(this, aOffer);
                    }
                }

                var declined = offers.Where(o => o.TradeOfferState == TradeOfferState.TradeOfferStateDeclined);

                foreach (var aOffer in declined)
                {
                    var wasRaised = false;
                    foreach (var oldsOffer in LastSentOffersData)
                    {
                        if (aOffer.TradeOfferId == oldsOffer.TradeOfferId)
                        {
                            if (oldsOffer.TradeOfferState == TradeOfferState.TradeOfferStateDeclined)
                                wasRaised = true;
                        }
                        if (!wasRaised)
                            OfferDeclined?.Invoke(this, aOffer);
                    }
                }

                LastSentOffersData = new List<Offer>(offers);
            }
        }
        private void CheckForReceivedOfferChanges(List<Offer> offers)
        {
            foreach(var newOffer in offers)
            {
                var isOld = false;
                foreach(var oldsOffer in LastReceivedOffersData)
                {
                    if(newOffer.TradeOfferId == oldsOffer.TradeOfferId)
                    {
                        isOld = true;
                        break;
                    }
                }
                if (!isOld)
                {
                    NewOffer?.Invoke(this, newOffer);
                    //AcceptOffer(newOffer);
                    OfferAccepted?.Invoke(this, newOffer);
                }
            }
            LastReceivedOffersData = new List<Offer>(offers);
        }

        private SteamID GetSelfId()
        {
            var html = web.Fetch(@"https://steamcommunity.com/", "get");
            var temp = html.Split(new[] { "data-miniprofile" }, StringSplitOptions.None)[0];
            var temp2 = temp.Split(new[] { "href=" }, StringSplitOptions.None);
            var id = temp2[temp2.Length - 1].Replace("\"", "");
            if (id.Contains("/id/"))
            {
                string profileVanity = id.Split(new[] { "id/" }, StringSplitOptions.None)[1].Split('/')[0];
                var xml = web.Fetch($@"http://steamcommunity.com/id/{profileVanity}/?xml=1", "get");

                string profile64 = xml.Split(new[] { "<steamID64>" }, StringSplitOptions.None)[1].Split(new[] { "</steamID64>" }, StringSplitOptions.None)[0];
                return new SteamID(ulong.Parse(profile64));
            }
            else
            {
                string profile64 = id.Split(new[] { "profiles/" }, StringSplitOptions.None)[1].Split('/')[0];
                return new SteamID(ulong.Parse(profile64));
            }
        }

        public bool CandelOffer(Offer offer)
        {
            TradeOffer tradeOffer = new TradeOffer(session, offer);
            return tradeOffer.Cancel();
        }

        public bool DeclineOffer(Offer offer)
        {
            var tradeOffer = new TradeOffer(session, offer);
            return tradeOffer.Decline();
        }

        public bool AcceptOffer(Offer offer)
        {
            var tradeOffer = new TradeOffer(session, offer);
            var accepted = tradeOffer.Accept();
            return accepted.Accepted;
        }

        public GenericInventory GetInventory(ulong targetId, int appId = 730, int context = 2)
        {
            return GetInventoryBase(new SteamID(targetId), appId, new long[] { context });
        }

        private GenericInventory GetInventoryBase(SteamID steamID, int appId, IEnumerable<long> contexts)
        {
            var inventory = new GenericInventory(web);
            inventory.Load(appId, contexts, steamID);
            return inventory;
        }

        public OffersResponse GetTradeOffers()
        {
            return api.GetAllTradeOffers();
        }

        public string SendOffer(List<GenericInventory.Item> outItems, List<GenericInventory.Item> theirItems, int accountIdOther, string message = "")
        {
            string tradeOfferId;
            var tradeOffer = FormTradeOffer(outItems, theirItems, accountIdOther);
            tradeOffer.Send(out tradeOfferId, message);
            return tradeOfferId;
        }

        private TradeOffer FormTradeOffer(List<GenericInventory.Item> outItems, List<GenericInventory.Item> theirItems, int accountIdOther = 0)
        {
            var itemsToGive = ConvertToCecon(outItems);
            var itemsToReceive = ConvertToCecon(theirItems);

            var offer = new Offer();
            if (accountIdOther != 0)
                offer.AccountIdOther = accountIdOther;
            offer.ItemsToGive = itemsToGive;
            offer.ItemsToReceive = itemsToReceive;

            var tradeOffer = new TradeOffer(session, offer);

            return tradeOffer;
        }

        private List<CEconAsset> ConvertToCecon(List<GenericInventory.Item> items)
        {
            var result = new List<CEconAsset>();

            if (items != null && items.Count > 0)
                foreach (var item in items)
                    result.Add(new CEconAsset
                    {
                        AssetId = item.AssetId.ToString(),
                        ContextId = item.ContextId.ToString(),
                        AppId = item.AppId.ToString()
                    });

            return result;
        }

        public string SendOfferWithToken(List<GenericInventory.Item> outItems, List<GenericInventory.Item> theirItems, string token, string message = "")
        {
            string tradeOfferId;
            var accIdOfferId = token.Split(new[] { "partner=" }, StringSplitOptions.None)[1].Split('&')[0];
            var tradeOffer = FormTradeOffer(outItems, theirItems, int.Parse(accIdOfferId));

            var splitter = "token=";
            token = token.Split(new[] { splitter }, StringSplitOptions.None)[1];

            tradeOffer.SendWithToken(out tradeOfferId, token, message);
            return tradeOfferId;
        }
        #endregion
    }
}
