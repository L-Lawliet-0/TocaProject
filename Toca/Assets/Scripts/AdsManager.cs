using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private static AdsManager m_Instance;
    public static AdsManager Instance { get { return m_Instance; } }

    public bool Initialized;

    private void Awake()
    {
        m_Instance = this;
        Initialized = false;
    }

    private void Start()
    {
        // call android function
        AndroidJavaClass jc = new AndroidJavaClass("com.huawei.drmdemo.MainActivity");
        if (jc != null)
        {
            jc.Call("onCreate");
            Debug.LogError("created");
        }


        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            Initialized = true;

            // privacy
            MaxSdk.SetIsAgeRestrictedUser(true);
            // MaxSdk.SetDoNotSell(true);

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            // load ads
            LoadInterstitial();

            // test shit
            // MaxSdk.ShowMediationDebugger();
        };

        MaxSdk.SetSdkKey("nhUsCTMqDF3ccTbS-2SusTcHmaiTZH8K-cBAAX_Oo2_ObbsZdIRUn3mXdT4BmDWkrhv_EuN0xNPWzF4bTW4KZr");
        //MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[] { "1294f35f-457e-4af8-be88-b0a794d40a31" });
        //MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

        StartCoroutine("AdsCounter");
    }

    public float Counter = 180;
    private IEnumerator AdsCounter()
    {
        while (true)
        {
            if (Counter <= 0 && !LoadingCtrl.Instance.Loading && LoadingCtrl.Instance.CurrentScene >= 1 && LoadingCtrl.Instance.CurrentScene <= 4)
                ShowAds();

            yield return new WaitForSeconds(1);
            Counter -= 1;
        }
    }

    // play ads when entering any of the four scenes
    // every 3 mintues in the four scenes
    public void ShowAds()
    {
        if (MaxSdk.IsInterstitialReady(adUnitId))
        {
            string adPlacements = "zhuangban";
            if (LoadingCtrl.Instance.CurrentScene == 1)
                adPlacements = "gongzhufang";
            else if (LoadingCtrl.Instance.CurrentScene == 2)
                adPlacements = "haijunfeng";
            else if (LoadingCtrl.Instance.CurrentScene == 3)
                adPlacements = "nanhaifang";
            MaxSdk.ShowInterstitial(adUnitId, adPlacements);
        }

        Counter = 180;
    }

    private int retryAttempt;
    string adUnitId = "59825548b9531499";
    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(adUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {  }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
}
