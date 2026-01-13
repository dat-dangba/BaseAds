using UnityEngine;

[RequireComponent(typeof(AppOpenAdApplovin))]
[RequireComponent(typeof(BannerAdApplovin))]
[RequireComponent(typeof(InterstitialAdApplovin))]
[RequireComponent(typeof(RewardedAdApplovin))]
public class Applovin : MonoBehaviour
{
    private AdsConfig adsConfig;

    [SerializeField] private AppOpenAdApplovin appOpenAd;
    [SerializeField] private BannerAdApplovin bannerAd;
    [SerializeField] private InterstitialAdApplovin interstitialAd;
    [SerializeField] private RewardedAdApplovin rewardedAdAd;

    public AppOpenAdApplovin AppOpenAd => appOpenAd;
    public BannerAdApplovin BannerAd => bannerAd;
    public InterstitialAdApplovin InterstitialAd => interstitialAd;
    public RewardedAdApplovin RewardedAdAd => rewardedAdAd;

    private void Reset()
    {
        appOpenAd = GetComponent<AppOpenAdApplovin>();
        bannerAd = GetComponent<BannerAdApplovin>();
        interstitialAd = GetComponent<InterstitialAdApplovin>();
        interstitialAd = GetComponent<InterstitialAdApplovin>();
        rewardedAdAd = GetComponent<RewardedAdApplovin>();
    }

    public void Init(AdsConfig adsConfig)
    {
        if (MaxSdk.IsInitialized()) return;

        this.adsConfig = adsConfig;

        MaxSdkCallbacks.OnSdkInitializedEvent += _ => { LoadAds(); };
        string[] deviceTests = new string[]
        {
        };
        MaxSdk.SetTestDeviceAdvertisingIdentifiers(deviceTests);
        MaxSdk.InitializeSdk();
    }

    private void LoadAds()
    {
        Debug.Log($"Ads - Applovin - LoadAds");
        appOpenAd.LoadAd(adsConfig.AppOpenAdUnitIdApplovin);
        bannerAd.LoadAd(adsConfig.BannerAdUnitIdApplovin);
        interstitialAd.LoadAd(adsConfig.InterstitialAdUnitIdApplovin);
        rewardedAdAd.LoadAd(adsConfig.RewardedAdUnitIdApplovin);
    }
}