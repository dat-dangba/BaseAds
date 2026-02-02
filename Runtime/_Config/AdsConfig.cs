using UnityEngine;

namespace DBD.Ads
{
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

        [HideInInspector] public bool is_remove_ads;

        [BuildKey(true, false)] [HideInInspector]
        public bool google_analytics_default_allow_analytics_storage = true;

        [BuildKey(true, false)] [HideInInspector]
        public bool google_analytics_default_allow_ad_storage = true;

        [BuildKey(true, false)] [HideInInspector]
        public bool google_analytics_default_allow_ad_user_data = true;

        [BuildKey(true, false)] [HideInInspector]
        public bool google_analytics_default_allow_ad_personalization_signals = true;

        [Header("Applovin")] [BuildKey] public string app_open_ad_unit_id_applovin;

        [BuildKey] public string banner_ad_unit_id_applovin;

        [BuildKey] public string interstitial_ad_unit_id_applovin;

        [BuildKey] public string rewarded_ad_unit_id_applovin;

        [Space(10)] [Header("Admob")] [BuildKey]
        public string app_open_ad_unit_id_admob;

        [BuildKey] public string banner_ad_unit_id_admob;

        // [BuildKey("banner_collapsible_ad_unit_id_admob")]
        // public string BannerCollapsibleAdUnitIdAdmob;

        [BuildKey] public string interstitial_ad_unit_id_admob;

        [BuildKey] public string rewarded_ad_unit_id_admob;

        [Space(10)] [Header("Config")] [BuildKey(false, false)]
        public bool app_open_ad_enabled;

        [BuildKey(false, false)] public bool banner_ad_enabled;

        // [BuildKey("banner_collapsible_ad_enabled")]
        // public bool BannerCollapsibleAdEnabled;

        [BuildKey(false, false)] public bool interstitial_ad_enabled = true;

        [BuildKey(false, false)] public bool rewarded_ad_enabled = true;

        [Space(10)] [BuildKey(false, false)] public AdNetwork app_open_ad_network = AdNetwork.admob;

        [BuildKey(false, false)] public AdNetwork banner_ad_network = AdNetwork.applovin;

        [BuildKey(false, false)] public AdNetwork interstitial_ad_network = AdNetwork.applovin;

        [BuildKey(false, false)] public AdNetwork rewarded_ad_network = AdNetwork.applovin;

        [Space(10)] [BuildKey(false, false)] public float interstitial_ad_cool_down = 60;

        public string GetUnitId(AdFormat format, AdNetwork network)
        {
            return (format, network) switch
            {
                (AdFormat.APP_OPEN, AdNetwork.applovin) => app_open_ad_unit_id_applovin,
                (AdFormat.BANNER, AdNetwork.applovin) => banner_ad_unit_id_applovin,
                (AdFormat.INTERSTITIAL, AdNetwork.applovin) => interstitial_ad_unit_id_applovin,
                (AdFormat.REWARDED, AdNetwork.applovin) => rewarded_ad_unit_id_applovin,

                (AdFormat.APP_OPEN, AdNetwork.admob) => app_open_ad_unit_id_admob,
                (AdFormat.BANNER, AdNetwork.admob) => banner_ad_unit_id_admob,
                // (AdFormat.BANNER_COLLAPSIBLE, AdNetwork.Admob) => BannerCollapsibleAdUnitIdAdmob,
                (AdFormat.INTERSTITIAL, AdNetwork.admob) => interstitial_ad_unit_id_admob,
                (AdFormat.REWARDED, AdNetwork.admob) => rewarded_ad_unit_id_admob,

                _ => string.Empty
            };
        }

        public void ApplyAndroidConfig()
        {
            google_analytics_default_allow_analytics_storage = true;
            google_analytics_default_allow_ad_storage = true;
            google_analytics_default_allow_ad_user_data = true;
            google_analytics_default_allow_ad_personalization_signals = true;

            app_open_ad_unit_id_applovin = "413984c574766b2e";
            banner_ad_unit_id_applovin = "c1db37c89406332c";
            interstitial_ad_unit_id_applovin = "c9638fd386ac26f8";
            rewarded_ad_unit_id_applovin = "eb2dd406960e6661";

            app_open_ad_unit_id_admob = "ca-app-pub-3940256099942544/9257395921";
            banner_ad_unit_id_admob = "ca-app-pub-3940256099942544/6300978111";
            // BannerCollapsibleAdUnitIdAdmob = "ca-app-pub-3940256099942544/2014213617";
            interstitial_ad_unit_id_admob = "ca-app-pub-3940256099942544/1033173712";
            rewarded_ad_unit_id_admob = "ca-app-pub-3940256099942544/5224354917";
        }

        public void ApplyIOSConfig()
        {
            google_analytics_default_allow_analytics_storage = true;
            google_analytics_default_allow_ad_storage = true;
            google_analytics_default_allow_ad_user_data = true;
            google_analytics_default_allow_ad_personalization_signals = true;

            app_open_ad_unit_id_applovin = "07acf81398225f5c";
            banner_ad_unit_id_applovin = "c5264147c2ead9c4";
            interstitial_ad_unit_id_applovin = "3f220ce0044243ac";
            rewarded_ad_unit_id_applovin = "a66614e192e32163";

            app_open_ad_unit_id_admob = "ca-app-pub-3940256099942544/9257395921";
            banner_ad_unit_id_admob = "ca-app-pub-3940256099942544/6300978111";
            // BannerCollapsibleAdUnitIdAdmob = "ca-app-pub-3940256099942544/2014213617";
            interstitial_ad_unit_id_admob = "ca-app-pub-3940256099942544/1033173712";
            rewarded_ad_unit_id_admob = "ca-app-pub-3940256099942544/5224354917";
        }
    }
}