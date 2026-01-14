using UnityEngine;

public class AdsConfig : BuildReflectionConfigBase
{
    private void Reset()
    {
#if UNITY_EDITOR && UNITY_IOS
        ApplyIOSConfig();
#else
        ApplyAndroidConfig();
#endif
    }

    [BuildKey] [HideInInspector] public bool google_analytics_default_allow_analytics_storage = true;
    [BuildKey] [HideInInspector] public bool google_analytics_default_allow_ad_storage = true;
    [BuildKey] [HideInInspector] public bool google_analytics_default_allow_ad_user_data = true;
    [BuildKey] [HideInInspector] public bool google_analytics_default_allow_ad_personalization_signals = true;

    [Header("Applovin")] [BuildKey("app_open_ad_unit_id_applovin")]
    public string AppOpenAdUnitIdApplovin;

    [BuildKey("banner_ad_unit_id_applovin")]
    public string BannerAdUnitIdApplovin;

    [BuildKey("interstitial_ad_unit_id_applovin")]
    public string InterstitialAdUnitIdApplovin;

    [BuildKey("rewarded_ad_unit_id_applovin")]
    public string RewardedAdUnitIdApplovin;

    [Space(10)] [Header("Admob")] [BuildKey("app_open_ad_unit_id_admob")]
    public string AppOpenAdUnitIdAdmob;

    [BuildKey("banner_ad_unit_id_admob")] public string BannerAdUnitIdAdmob;

    // [BuildKey("banner_collapsible_ad_unit_id_admob")]
    // public string BannerCollapsibleAdUnitIdAdmob;

    [BuildKey("interstitial_ad_unit_id_admob")]
    public string InterstitialAdUnitIdAdmob;

    [BuildKey("rewarded_ad_unit_id_admob")]
    public string RewardedAdUnitIdAdmob;

    [Space(10)] [Header("Config")] [BuildKey("app_open_ad_enabled")]
    public bool AppOpenAdEnabled;

    [BuildKey("banner_ad_enabled")] public bool BannerAdEnabled;

    // [BuildKey("banner_collapsible_ad_enabled")]
    // public bool BannerCollapsibleAdEnabled;

    [BuildKey("interstitial_ad_enabled")] public bool InterstitialAdEnabled = true;
    [BuildKey("rewarded_ad_enabled")] public bool RewardedAdEnabled = true;

    [Space(10)] [BuildKey("app_open_ad_network")]
    public AdNetwork AppOpenAdNetWork = AdNetwork.Admob;

    [BuildKey("banner_ad_network")] public AdNetwork BannerAdNetWork = AdNetwork.Applovin;
    [BuildKey("interstitial_ad_network")] public AdNetwork InterstitialAdNetWork = AdNetwork.Applovin;
    [BuildKey("rewarded_ad_network")] public AdNetwork RewardedAdNetWork = AdNetwork.Applovin;

    [Space(10)] [BuildKey("interstitial_ad_cool_down")]
    public float InterstitialAdCoolDown = 60;

    public string GetUnitId(AdFormat format, AdNetwork network)
    {
        return (format, network) switch
        {
            (AdFormat.APP_OPEN, AdNetwork.Applovin) => AppOpenAdUnitIdApplovin,
            (AdFormat.BANNER, AdNetwork.Applovin) => BannerAdUnitIdApplovin,
            (AdFormat.INTERSTITIAL, AdNetwork.Applovin) => InterstitialAdUnitIdApplovin,
            (AdFormat.REWARDED, AdNetwork.Applovin) => RewardedAdUnitIdApplovin,

            (AdFormat.APP_OPEN, AdNetwork.Admob) => AppOpenAdUnitIdAdmob,
            (AdFormat.BANNER, AdNetwork.Admob) => BannerAdUnitIdAdmob,
            // (AdFormat.BANNER_COLLAPSIBLE, AdNetwork.Admob) => BannerCollapsibleAdUnitIdAdmob,
            (AdFormat.INTERSTITIAL, AdNetwork.Admob) => InterstitialAdUnitIdAdmob,
            (AdFormat.REWARDED, AdNetwork.Admob) => RewardedAdUnitIdAdmob,

            _ => string.Empty
        };
    }

    public void ApplyAndroidConfig()
    {
        google_analytics_default_allow_analytics_storage = true;
        google_analytics_default_allow_ad_storage = true;
        google_analytics_default_allow_ad_user_data = true;
        google_analytics_default_allow_ad_personalization_signals = true;

        AppOpenAdUnitIdApplovin = "413984c574766b2e";
        BannerAdUnitIdApplovin = "c1db37c89406332c";
        InterstitialAdUnitIdApplovin = "c9638fd386ac26f8";
        RewardedAdUnitIdApplovin = "eb2dd406960e6661";

        AppOpenAdUnitIdAdmob = "ca-app-pub-3940256099942544/9257395921";
        BannerAdUnitIdAdmob = "ca-app-pub-3940256099942544/6300978111";
        // BannerCollapsibleAdUnitIdAdmob = "ca-app-pub-3940256099942544/2014213617";
        InterstitialAdUnitIdAdmob = "ca-app-pub-3940256099942544/1033173712";
        RewardedAdUnitIdAdmob = "ca-app-pub-3940256099942544/5224354917";
    }

    public void ApplyIOSConfig()
    {
        google_analytics_default_allow_analytics_storage = true;
        google_analytics_default_allow_ad_storage = true;
        google_analytics_default_allow_ad_user_data = true;
        google_analytics_default_allow_ad_personalization_signals = true;

        AppOpenAdUnitIdApplovin = "07acf81398225f5c";
        BannerAdUnitIdApplovin = "c5264147c2ead9c4";
        InterstitialAdUnitIdApplovin = "3f220ce0044243ac";
        RewardedAdUnitIdApplovin = "a66614e192e32163";

        AppOpenAdUnitIdAdmob = "ca-app-pub-3940256099942544/9257395921";
        BannerAdUnitIdAdmob = "ca-app-pub-3940256099942544/6300978111";
        // BannerCollapsibleAdUnitIdAdmob = "ca-app-pub-3940256099942544/2014213617";
        InterstitialAdUnitIdAdmob = "ca-app-pub-3940256099942544/1033173712";
        RewardedAdUnitIdAdmob = "ca-app-pub-3940256099942544/5224354917";
    }
}