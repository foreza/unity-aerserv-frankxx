# unity-frankxx

This project demonstrates how to integrate multiple Platform Plugins on the Unity platform.
This one uses the AdMob Platform Plugin combined with the AerServ Platform Plugin as primary. 

This path is typically used with Admob, MoPub, and other larger ad networks that don't just want to use Unity Ads.
A common use case is to display banner ads in-game. Unity makes it difficult / does not directly support banner ads.

Usually, a "CORE" SDK is selected that can mediate to other platforms rather than have requests go to all of the ad networks.
A reason why you might not want to do that is due to low usage rate, which might impact the application's revenue and overall performance as well.

A typical game dev might integrate the core SDK (AerServ in this example) and then implement other SDKs and do internal 'mediation'.
This approach demonstrates a waterfall that is actually taken care of in the app rather than in AerServ's OpenAuction / platform mediation solution.
Here, the app publisher loads both plugins for AerServ + Admob, resolves any conflicts in the global name space, and then attempts an ad request.
If the ad fails to fill on AerServ side, AdMob's preloaded ad might be shown.

Overall, this strategy allows for decent fill rate across the board along with complete control over the waterfall procedure itself, at the cost of possible performance / bloat if more networks are added.
By not taking advantage of either Admob or AerServ's ability to mediate internally, they increase the amount of processing / decision making done.



## AerServ Unity Platform Plugin:

The Unity Plugin is a .unitypackage file that is essentially adapter code (C#) that will extract and speak with the relevant AerServ classes. 
https://support.aerserv.com/hc/en-us/articles/203226480?

## AdMob Unity Platform Plugin:

https://developers.google.com/admob/unity/start

## Notes:

SampleApp.cs demonstrates most of this function. 

WORK IN PROGRESS
