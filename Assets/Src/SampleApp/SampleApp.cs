using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using System;

	public class SampleApp : MonoBehaviour {
		
        // Define a sample plc here.
		private String plc = "1000834";
		private static String message = "";
		private static String del = "";

        private Boolean AerServPreloadReady = false;
        private Boolean AdMobPreloadReady = false;
        
        // Only a single instance should exist at any time
        public static SampleApp Instance;

    void Awake() {
        Instance = this;
    }


	// OnGUI is called for rendering and handling GUI events. 
	void OnGUI() {


			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fontSize = 22;

            plc	= GUI.TextField(new Rect(Screen.width/2-150, Screen.height/2-500, 300, 100), plc, style);

            // Load + show an interstitial
			if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2-350, 300, 100), "Load Interstitial", style))
			{

				if( plc == null || plc.Equals(""))
					plc = "1000834";	
				
				message = "";
				del = "";

            Dictionary<string, object> config = AerServ_generateTestConfig();
            AerServ.SDK.LoadInterstitial(plc, interstitialEventCB, config);
			}

            // Preload an interstitial
			if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2-240, 300, 100), "Preload Interstitial", style))
			{
				if( plc == null || plc.Equals(""))
					plc = "1000834";	

				message = "";
				del = "";

            Dictionary<string, object> config = AerServ_generateTestConfig();

            if (!AerServPreloadReady){
                AerServ.SDK.PreloadInterstitial(plc, interstitialEventCB, config);
                message += del + "[AERSERV] PreloadInterstitial !";
                MonoBehaviour.print("[AERSERV] PreloadInterstitial");
            }
            if (!AdMobPreloadReady) {
                AdMob_RequestInterstitial();
                message += del + "[ADMOB] RequestInterstitial !";
                MonoBehaviour.print("[ADMOB] RequestInterstitial");
            }
			}

        // Show an interstitial that has been preloaded
        if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 130, 300, 100), "Show Interstitial", style))
        {
            if (plc == null || plc.Equals(""))
                plc = "1000834";

            message = "";
            del = "";

            if (AerServPreloadReady)
            {
                AerServ.SDK.ShowInterstitial();
                message += del + "[AERSERV] ShowInterstitial !";
                MonoBehaviour.print("[AERSERV] ShowInterstitial");
            }
            else if (AdMobPreloadReady)
            {
                admob_interstitial.Show();
                message += del + "[ADMOB] ShowInterstitial !";
                MonoBehaviour.print("[ADMOB] ShowInterstitial");
            }
            else
            {
                MonoBehaviour.print("[AERSERV & ADMOB] do not have interstitials preloaded");
            }
        }

				
			//if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2-20, 300, 100), "Load Banner", style))
			//{

			//	if( plc == null || plc.Equals(""))
			//		plc = "1000834";	

			//	message = "";
			//	del = "";

			//	Dictionary<string, object> config = generateTestConfig();

   //         // Give preference to show AerServ first. If the aerServEventCallBack indicates that no ad was returned / shown, then use others in the 'waterfall'
			//	AerServ.SDK.ShowBanner(plc, (int) 320, (int) 50, AerServ.SDK.BANNER_BOTTOM, aerServEventCallBack, config);
			//}

			//if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2+90, 300, 100), "Kill Banner", style))
			//{
				
			//	message = "";
			//	del = "";
			//	AerServ.SDK.KillBanner();
			//}

			if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2+200, 300, 100), "Init", style))
			{
				message = "";
				del = "";

                // Initialize the AerServ SDK
				AerServ.SDK.InitSdk("101190");
 
                // Initialize the Google Mobile Ads SDK.
                MobileAds.Initialize("ca-app-pub-3940256099942544~1458002511");
			}

			GUIStyle style2 = new GUIStyle(GUI.skin.textArea);
			style2.fontSize = 20;
			message = GUI.TextArea(new Rect(Screen.width/2-350, Screen.height/2+310, 700, 300), message, style2);
		}

		/** This method acts as a bridge between native code and client code **/
		[MonoPInvokeCallback (typeof (AerServ.OnAerServEvent))]

        // Would recommend have a different event callback depending on the type of ad you are trying to show as AD_Failed will not contain much information beyond the error code
		static void interstitialEventCB (int eventType, String args) {

			String[] argsArr = (args != null) ? args.Split('|') : null;

			switch(eventType) {

				case AerServ.SDK.AD_FAILED:
                    message += del + "[AERSERV] AD_FAILED invoked! message: " + argsArr[0];
                    MonoBehaviour.print("[AERSERV] AD_FAILED, " + argsArr[0]);

					if (argsArr.Length > 1) {
						message += del + ", code: " + argsArr[1];

                    // Option 1: IF AERSERV FAILED TO FILL due to a specific reason, CALL THE ADMOB waterfall
                        // NOT DOING

					}

                // Option 2: If AERSERV failed to fill for any reason at all, begin the admob loading process

                message += " ===> ===> ===> LOADING ADMOB AS BACKFILL  ===> ===> ===>";
                MonoBehaviour.print("===> ===> ===> LOADING ADMOB AS BACKFILL  ===> ===> ===>");


                // Have the singleton call this
                Instance.AdMob_RequestInterstitial();
                    
					break;
				case AerServ.SDK.AD_LOADED:
                    message += del + "[AERSERV] AD_LOADED invoked!";
                    MonoBehaviour.print("[AERSERV] AD_LOADED");
                    
                    // Set preload ready for AS to true
                    Instance.AerServPreloadReady = true;

					break;
				case AerServ.SDK.AD_COMPLETED:
                    message += del + "[AERSERV] AD_COMPLETED invoked!";
                    MonoBehaviour.print("[AERSERV] AD_COMPLETED");
                    
                    // Set preload ready for AS to false after it has shown
                    Instance.AerServPreloadReady = false;


					break;
				case AerServ.SDK.AD_CLICKED:
                    message += del + "[AERSERV] AD_CLICKED invoked!";
                    MonoBehaviour.print("[AERSERV] AD_CLICKED");

					break;
				case AerServ.SDK.AD_DISMISSED:
                    message += del + "[AERSERV] AD_DISMISSED invoked!";
                    MonoBehaviour.print("[AERSERV] AD_DISMISSED");

					break;
				case AerServ.SDK.PRELOAD_READY:
                    message += del + "[AERSERV] PRELOAD_READY invoked!";
                    MonoBehaviour.print("[AERSERV] PRELOAD_READY");

					break;
				case AerServ.SDK.VC_READY:
                    message += del + "[AERSERV] VC_READY invoked! Name: " + argsArr[0] + ", Amount: " + argsArr[1]
							+ ", Buyer name: " + argsArr[2] + ", Buyer price: " + argsArr[3];
                    MonoBehaviour.print("[AERSERV] VC_READY");

					break;
				case AerServ.SDK.VC_REWARDED:
                    message += del + "[AERSERV] VC_REWARDED invoked! Name: " + argsArr[0] + ", Amount: " + argsArr[1]
							+ ", Buyer name: " + argsArr[2] + ", Buyer price: " + argsArr[3];
                    MonoBehaviour.print("[AERSERV] VC_REWARDED");

					break;
				default:
					break;

			}

			del = "\n";

		}

		// Returns a dictionary that can be passed into ad requests to configure ad requests.
		// This is optional and is only included here as a sample to illustrate how it is done.
    static Dictionary<string, object> AerServ_generateTestConfig() {
            
			Dictionary<string, object> config = new Dictionary<string, object>();
			config.Add(AerServ.SDK.PARAM_USER_ID, "ZeroTwo");
			config.Add(AerServ.SDK.PARAM_TIMEOUT_MILLIS, 10000);

			// Setting publisher keys
			Dictionary<string, object> pubKeys = new Dictionary<string, object>();
			pubKeys.Add("testPubKey1", "testPubKeyValue1");
			pubKeys.Add("testPubKey2", "testPubKeyValue2");
			config.Add(AerServ.SDK.PARAM_PUB_KEYS, pubKeys);

			// Setting keywords
			List<string> keywords = new List<string>();
			keywords.Add("16");
			keywords.Add("15");
			config.Add(AerServ.SDK.PARAM_KEYWORDS, keywords);

			return config;
		}


        // ADMOB CALLS
    InterstitialAd admob_interstitial = new InterstitialAd("ca-app-pub-3940256099942544/5135589807");

    private void AdMob_RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        admob_interstitial.LoadAd(request);

        message += del + "[ADMOB] AdMob_RequestInterstitial called!";
        MonoBehaviour.print("[ADMOB] AdMob_RequestInterstitial called");


        // Called when an ad request has successfully loaded.
        admob_interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        admob_interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        admob_interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        admob_interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        admob_interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        message += del + "[ADMOB] HandleAdLoaded received!";
        MonoBehaviour.print("[ADMOB] HandleAdLoaded event received");
        AdMobPreloadReady = true;

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        message += del + "[ADMOB] HandleFailedToReceiveAd received, " + args.Message;
        MonoBehaviour.print("[ADMOB] HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        AdMobPreloadReady = false;
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        message += del + "[ADMOB] HandleAdOpened received!";
        MonoBehaviour.print("[ADMOB] HandleAdOpened event received");
        AdMobPreloadReady = false;

    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        message += del + "[ADMOB] HandleAdClosed received!";
        MonoBehaviour.print("[ADMOB] HandleAdClosed event received");
        AdMobPreloadReady = false;

    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        message += del + "[ADMOB] HandleAdLeavingApplication received!";
        MonoBehaviour.print("[ADMOB] HandleAdLeavingApplication event received");
    }



	}

