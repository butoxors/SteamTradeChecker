using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SteamKit2;
using SteamTrade.TradeWebAPI;

namespace SteamTrade
{

    /// <summary>
    /// Generic Steam Backpack Interface
    /// </summary>
    public class GenericInventory
    {
        private readonly SteamWeb SteamWeb;

        public GenericInventory(SteamWeb steamWeb)
        {
            SteamWeb = steamWeb;
        }

        public List<NormalItem> NormalItems
        {
            get
            {
                var normItems = new List<NormalItem>();

                foreach (var desc in Descriptions.Values)
                {
                    var item = GetNormalItem(desc.Name);
                    if (!normItems.Contains(item))
                        normItems.Add(item);
                }
                return normItems;
            }
        }

        public Dictionary<ulong, Item> Items
        {
            get
            {
                return _items;
            }
        }

        public Dictionary<string, ItemDescription> Descriptions
        {
            get
            {
                return _descriptions;
            }
        }

        public List<string> Errors
        {
            get
            {
                if (_loadTask == null)
                    return null;
                _loadTask.Wait();
                return _errors;
            }
        }

        public bool IsLoaded = false;

        public NormalItem GetNormalItem(string name)
        {
            var descriptions = Descriptions.Where(d => d.Value.Name.Equals(name));

            if (descriptions != null && descriptions.Count() > 0)
            {
                var desc = descriptions.First();
                var item = Items.Where(i => i.Value.DescriptionId == desc.Key);

                if (item != null && item.Count() > 0)
                    return new NormalItem(item.First().Value, desc.Value);
                else
                    return null;
            }
            else
                return null;
        }

        private Task _loadTask;
        private Dictionary<string, ItemDescription> _descriptions = new Dictionary<string, ItemDescription>();
        private Dictionary<ulong, Item> _items = new Dictionary<ulong, Item>();
        private List<string> _errors = new List<string>();

        public class Item : TradeUserAssets
        {
            public Item(int appid, long contextid, ulong assetid, string descriptionid, int amount = 1) : base(appid, contextid, assetid, amount)
            {
                this.DescriptionId = descriptionid;
            }

            public string DescriptionId { get; private set; }

            public override string ToString()
            {
                return string.Format("id:{0}, appid:{1}, contextid:{2}, amount:{3}, descriptionid:{4}",
                    AssetId, AppId, ContextId, Amount, DescriptionId);
            }
        }

        public class ItemDescription
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool Tradable { get; set; }
            public bool Marketable { get; set; }
            public string Url { get; set; }
            public long Classid { get; set; }

            public Dictionary<string, string> AppData { get; set; }

            public void DebugAppData()
            {
                Console.WriteLine("\n\"" + Name + "\"");
                if (AppData == null)
                {
                    Console.WriteLine("Doesn't have app_data");
                    return;
                }

                foreach (var value in AppData)
                {
                    Console.WriteLine(string.Format("{0} = {1}", value.Key, value.Value));
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Returns information (such as item name, etc) about the given item.
        /// This call can fail, usually when the user's inventory is private.
        /// </summary>
        public ItemDescription GetDescription(ulong id)
        {
            if (_loadTask == null)
                return null;
            _loadTask.Wait();

            try
            {
                return _descriptions[_items[id].DescriptionId];
            }
            catch
            {
                return null;
            }
        }

        public void LoadInTask(int appid, IEnumerable<long> contextIds, SteamID steamid)
        {
            List<long> contextIdsCopy = contextIds.ToList();
            _loadTask = Task.Factory.StartNew(() => Load(appid, contextIdsCopy, steamid));
        }

        public void Load(int appid, IEnumerable<long> contextIds, SteamID steamid)
        {
            dynamic invResponse;
            IsLoaded = false;
            Dictionary<string, string> tmpAppData;

            _items.Clear();
            _descriptions.Clear();
            _errors.Clear();

            try
            {
                foreach (long contextId in contextIds)
                {
                    string response = SteamWeb.Fetch(string.Format("http://steamcommunity.com/profiles/{0}/inventory/json/{1}/{2}/", steamid.ConvertToUInt64(), appid, contextId), "GET", null, true);
                    invResponse = JsonConvert.DeserializeObject(response);

                    if (invResponse.success == false)
                    {
                        _errors.Add("Fail to open backpack: " + invResponse.Error);
                        continue;
                    }

                    //rgInventory = Items on Steam Inventory 
                    foreach (var item in invResponse.rgInventory)
                    {

                        foreach (var itemId in item)
                        {
                            string descriptionid = itemId.classid + "_" + itemId.instanceid;
                            _items.Add((ulong)itemId.id, new Item(appid, contextId, (ulong)itemId.id, descriptionid));
                            break;
                        }
                    }

                    // rgDescriptions = Item Schema (sort of)
                    foreach (var description in invResponse.rgDescriptions)
                    {
                        foreach (var class_instance in description)// classid + '_' + instenceid 
                        {
                            if (class_instance.app_data != null)
                            {
                                tmpAppData = new Dictionary<string, string>();
                                foreach (var value in class_instance.app_data)
                                {
                                    tmpAppData.Add("" + value.Name, "" + value.Value);
                                }
                            }
                            else
                            {
                                tmpAppData = null;
                            }

                            _descriptions.Add("" + (class_instance.classid ?? '0') + "_" + (class_instance.instanceid ?? '0'),
                                new ItemDescription()
                                {
                                    Name = class_instance.market_hash_name,
                                    Type = class_instance.type,
                                    Marketable = (bool)class_instance.marketable,
                                    Tradable = (bool)class_instance.tradable,
                                    Classid = long.Parse((string)class_instance.classid),
                                    Url = (class_instance.actions != null && class_instance.actions.First["link"] != null ? class_instance.actions.First["link"] : ""),
                                    AppData = tmpAppData
                                }
                            );
                            break;
                        }
                    }

                }//end for (contextId)
            }//end try
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _errors.Add("Exception: " + e.Message);
            }
            IsLoaded = true;
        }
    }

    public class NormalItem
    {
        public string Name
        {
            get
            {
                return GIItemDesc.Name;
            }
        }
        public bool IsTradable
        {
            get
            {
                return GIItemDesc.Tradable;
            }
        }
        public string AssetId
        {
            get
            {
                return GIItem.AssetId.ToString();
            }
        }
        public string ContextId
        {
            get
            {
                return GIItem.ContextId.ToString();
            }
        }
        public string AppId
        {
            get
            {
                return GIItem.AppId.ToString();
            }
        }
        public string ClassId
        {
            get
            {
                return GIItemDesc.Classid.ToString();
            }
        }
        public string Type
        {
            get
            {
                return GIItemDesc.Type;
            }
        }
        public int Amount
        {
            get
            {
                return GIItem.Amount;
            }
        }

        public GenericInventory.Item GIItem { get; set; }
        public GenericInventory.ItemDescription GIItemDesc { get; set; }

        public NormalItem(GenericInventory.Item i, GenericInventory.ItemDescription d)
        {
            GIItem = i;
            GIItemDesc = d;
        }
    }
}
