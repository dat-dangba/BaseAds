using GoogleMobileAds.Api;
using UnityEngine;

namespace DBD.Ads
{
    [RequireComponent(typeof(AppOpenAdAdmob))]
    [RequireComponent(typeof(BannerAdAdmob))]
// [RequireComponent(typeof(BannerCollapsibleAdAdmob))]
    [RequireComponent(typeof(InterstitialAdAdmob))]
    [RequireComponent(typeof(RewardedAdAdmob))]
    public class Admob : MonoBehaviour
    {
        private AdsConfig adsConfig;

        [SerializeField] private AppOpenAdAdmob appOpenAd;

        [SerializeField] private BannerAdAdmob bannerAd;

        // [SerializeField] private BannerCollapsibleAdAdmob bannerCollapsibleAd;
        [SerializeField] private InterstitialAdAdmob interstitialAd;
        [SerializeField] private RewardedAdAdmob rewardedAd;

        public AppOpenAdAdmob AppOpenAd => appOpenAd;

        public BannerAdAdmob BannerAd => bannerAd;

        // public BannerCollapsibleAdAdmob BannerCollapsibleAd => bannerCollapsibleAd;
        public InterstitialAdAdmob InterstitialAd => interstitialAd;
        public RewardedAdAdmob RewardedAd => rewardedAd;

        private void Reset()
        {
            appOpenAd = GetComponent<AppOpenAdAdmob>();
            bannerAd = GetComponent<BannerAdAdmob>();
            // bannerCollapsibleAd = GetComponent<BannerCollapsibleAdAdmob>();
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
            Debug.LogWarning($"Ads - Admob - LoadAds");
            LoadAppOpenAd();
            LoadBannerAd();
            // LoadBannerCollapsibleAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            if (!adsConfig.rewarded_ad_enabled || adsConfig.rewarded_ad_network != AdNetwork.admob) return;
            rewardedAd.LoadAd(adsConfig.rewarded_ad_unit_id_admob);
        }

        private void LoadInterstitialAd()
        {
            if (!adsConfig.interstitial_ad_enabled || adsConfig.interstitial_ad_network != AdNetwork.admob) return;
            interstitialAd.LoadAd(adsConfig.interstitial_ad_unit_id_admob);
        }

        // private void LoadBannerCollapsibleAd()
        // {
        //     if (!adsConfig.BannerCollapsibleAdEnabled) return;
        //     bannerCollapsibleAd.LoadAd(adsConfig.BannerCollapsibleAdUnitIdAdmob);
        // }

        private void LoadBannerAd()
        {
            if (!adsConfig.banner_ad_enabled || adsConfig.banner_ad_network != AdNetwork.admob) return;
            bannerAd.LoadAd(adsConfig.banner_ad_unit_id_admob);
        }

        private void LoadAppOpenAd()
        {
            if (!adsConfig.app_open_ad_enabled || adsConfig.app_open_ad_network != AdNetwork.admob) return;
            appOpenAd.LoadAd(adsConfig.app_open_ad_unit_id_admob);
        }
    }
}