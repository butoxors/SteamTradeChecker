Parser of data from sites for the trade of things STEAM:
-	Interaction with Steam and other network resources via Cookie;
-	Parsing json;
-	Using LINQ to fetch data from JSON objects;
-	Asynchronous programming;
-	Logging;


Needed folders:

In '/bin/Debug/' catalog need create two new folders:

1.Configs
   Files: 'loot.cfg' and 'swap.cfg'. 
   //This files contain your cookies from Loot.Farm and Swap.GG sites;

2.And into 'Config' folder named 'Steam', like 'Config/Steam/'

    Files: 'cookies.cfg' and 'key.cfg'
    
    //'cookies.cfg' contain cookie from active Steam session; this needed to auto-accepting trade offers
    
    //'key.cfg' contain API Key to your Steam account. If you don`t know how it`s, go to https://steamcommunity.com/dev/apikey link, and 
    
    // copy or generate your personal API Key;
    
If you need change location your folders, you can  rewrite paths on 'Config.cs' file.

if you don`t know how extract cookies, you can setup some extension to your browser like 'EditThisCookie', if you use Chrome browser.

/*
  Auto-accepting exact works with trade offers who don`t must needed two-factor auth and when you check radio button before you start       receive trade offers.
*/
