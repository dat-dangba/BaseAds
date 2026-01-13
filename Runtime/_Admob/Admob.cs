using GoogleMobileAds.Api;
using UnityEngine;

[RequireComponent(typeof(AppOpenAdAdmob))]
[RequireComponent(typeof(BannerAdAdmob))]
[RequireComponent(typeof(BannerCollapsibleAdAdmob))]
[RequireComponent(typeof(InterstitialAdAdmob))]
[RequireComponent(typeof(RewardedAdAdmob))]
public class Admob : MonoBehaviour
{
    private AdsConfig adsConfig;

    [SerializeField] private AppOpenAdAdmob appOpenAd;
    [SerializeField] private BannerAdAdmob bannerAd;
    [SerializeField] private BannerCollapsibleAdAdmob bannerCollapsibleAd;
    [SerializeField] private InterstitialAdAdmob interstitialAd;
    [SerializeField] private RewardedAdAdmob rewardedAd;

    public AppOpenAdAdmob AppOpenAd => appOpenAd;
    public BannerAdAdmob BannerAd => bannerAd;
    public BannerCollapsibleAdAdmob BannerCollapsibleAd => bannerCollapsibleAd;
    public InterstitialAdAdmob InterstitialAd => interstitialAd;
    public RewardedAdAdmob RewardedAd => rewardedAd;

    private void Reset()
    {
        appOpenAd = GetComponent<AppOpenAdAdmob>();
        bannerAd = GetComponent<BannerAdAdmob>();
        bannerCollapsibleAd = GetComponent<BannerCollapsibleAdAdmob>();
        interstitialAd = GetComponent<InterstitialAdAdmob>();
        rewardedAd = GetComponent<RewardedAdAdmob>();
    }

    public void Init(AdsConfig adsConfig)
    {
        this.adsConfig = adsConfig;

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(initStatus => { LoadAds(); });
    }

    private void LoadAds()
    {
        appOpenAd.LoadAd(adsConfig.AppOpenAdUnitIdAdmob);
        bannerAd.LoadAd(adsConfig.BannerAdUnitIdAdmob);
        bannerCollapsibleAd.LoadAd(adsConfig.BannerCollapsibleAdUnitIdAdmob);
        interstitialAd.LoadAd(adsConfig.InterstitialAdUnitIdAdmob);
        rewardedAd.LoadAd(adsConfig.RewardedAdUnitIdAdmob);
    }
}