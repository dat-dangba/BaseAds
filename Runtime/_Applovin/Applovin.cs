using UnityEngine;

namespace DBD.Ads
{
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
            Debug.LogWarning($"Ads - Applovin - LoadAds");
            LoadAppOpenAd();
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            if (!adsConfig.rewarded_ad_enabled || adsConfig.rewarded_ad_network != AdNetwork.applovin) return;
            rewardedAdAd.LoadAd(adsConfig.rewarded_ad_unit_id_applovin);
        }

        private void LoadInterstitialAd()
        {
            if (!adsConfig.interstitial_ad_enabled || adsConfig.interstitial_ad_network != AdNetwork.applovin) return;
            interstitialAd.LoadAd(adsConfig.interstitial_ad_unit_id_applovin);
        }

        private void LoadBannerAd()
        {
            if (!adsConfig.banner_ad_enabled || adsConfig.banner_ad_network != AdNetwork.applovin) return;
            bannerAd.LoadAd(adsConfig.banner_ad_unit_id_applovin);
        }

        private void LoadAppOpenAd()
        {
            if (!adsConfig.app_open_ad_enabled || adsConfig.app_open_ad_network != AdNetwork.applovin) return;
            appOpenAd.LoadAd(adsConfig.app_open_ad_unit_id_applovin);
        }
    }
}