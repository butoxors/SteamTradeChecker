Parser of data from sites for the trade of things STEAM:
-	Interaction with Steam and other network resources via Cookie;
-	Parsing json;
-	Using LINQ to fetch data from JSON objects;
-	Asynchronous programming;
-	Logging;
---
Now program exact works with only two sites, it's <http://LOOT.FARM> and <http://SWAP.GG>
---
Needed folders:

In '/bin/Debug/' catalog need create two new folders:

1. Configs
Files: ***'loot.cfg'*** and ***'swap.cfg'***. 
2. Into ***'Config'*** folder named ***'Steam'***, like ***'Config/Steam/'***
***
**Files: 'loot.cfg', 'swap.cfg', 'cookies.cfg' and 'key.cfg'**
    
***'loot.cfg'*** contain cookies from <http://Loot.Farm> site;
    
***'swap.gg'*** contain cookies from <http://Swap.GG> site;
    
***'cookies.cfg'*** contain cookie from active Steam session; this needed to auto-accepting trade offers
    
***'key.cfg'*** contain API Key to your Steam account. If you don't know how it's, go to <https://steamcommunity.com/dev/apikey> link, and copy or generate your personal API Key;

***

If you need change location your folders, you can  rewrite paths on ***'Config.cs'*** file.

if you don't know how extract cookies, you can setup some extension to your browser like [EditThisCookie](https://chrome.google.com/webstore/detail/editthiscookie/fngmhnnpilhplaeedifhccceomclgfbg?hl=ru), if you use Chrome browser.

---

***Auto-accepting exact works with trade offers who don't must needed two-factor auth and when you check radio button before you start       receive trade offers.***
