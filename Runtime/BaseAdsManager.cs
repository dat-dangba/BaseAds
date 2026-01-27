using System;
using System.IO;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEditor;
using UnityEngine;

namespace DBD.Ads
{
    public abstract class BaseAdsManager<INSTANCE> : MonoBehaviour
    {
        public static INSTANCE Instance { get; private set; }

        [SerializeField] private Consent consent;
        [SerializeField] private Admob admob;
        [SerializeField] private Applovin applovin;

        [SerializeField] protected AdsConfig adsConfig;

        private bool isStartGame;
        private bool canShowAppOpenAdResumeGame = true;

        protected bool IsStartGame => isStartGame;

        protected bool CanShowAppOpenAdResumeGame => canShowAppOpenAdResumeGame;

        private float timeShowInterstitial;

        public bool IsRemoveAds { get; private set; }
        public bool IsInit { get; private set; }

        private const string AppOpenStartGame = "app_open_ad_start_game";
        private const string AppOpenResumeGame = "app_open_ad_resume_game";

#if UNITY_EDITOR
        private void Setup()
        {
            GetAdsConfig();
            AddConsent();
            AddAdmob();
            AddApplovin();
        }

        private void GetAdsConfig()
        {
            adsConfig = Resources.Load<AdsConfig>("AdsConfig");
            if (adsConfig != null) return;
            Directory.CreateDirectory("Assets/Resources");
            adsConfig = ScriptableObject.CreateInstance<AdsConfig>();
            string assetPath = Path.Combine("Assets/Resources", "AdsConfig.asset");
            AssetDatabase.CreateAsset(adsConfig, assetPath);
            AssetDatabase.SaveAssets();
        }

        private void AddApplovin()
        {
            Transform trans = transform.Find("Applovin");
            if (trans != null)
            {
                DestroyImmediate(trans.gameObject);
            }

            GameObject applovinGO = new GameObject("Applovin");
            applovinGO.transform.SetParent(transform);
            applovin = applovinGO.AddComponent<Applovin>();
        }

        private void AddAdmob()
        {
            Transform trans = transform.Find("Admob");
            if (trans != null)
            {
                DestroyImmediate(trans.gameObject);
            }

            GameObject admobGO = new GameObject("Admob");
            admobGO.transform.SetParent(transform);
            admob = admobGO.AddComponent<Admob>();
        }

        private void AddConsent()
        {
            Transform trans = transform.Find("Consent");
            if (trans != null)
            {
                DestroyImmediate(trans.gameObject);
            }

            GameObject consentGO = new GameObject("Consent");
            consentGO.transform.SetParent(transform);
            consent = consentGO.AddComponent<Consent>();
        }

        protected virtual void Reset()
        {
            Setup();
        }
#endif

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = GetComponent<INSTANCE>();

                Transform root = transform.root;
                if (root != transform)
                {
                    DontDestroyOnLoad(root);
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }

                AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }

        protected virtual void OnApplicationFocus(bool hasFocus)
        {
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
        }

        protected virtual void OnEnable()
        {
            Consent.OnRequestConsentCompleted += OnRequestConsentCompleted;
            AdAction.OnAppOpenAdResumeGameCanShowChanged += OnAppOpenAdResumeGameCanShowChanged;
            AdAction.OnAdPaid += OnAdPaid;
            AdAction.OnAdClicked += OnAdClicked;
        }

        protected virtual void OnDisable()
        {
            Consent.OnRequestConsentCompleted -= OnRequestConsentCompleted;
            AdAction.OnAppOpenAdResumeGameCanShowChanged -= OnAppOpenAdResumeGameCanShowChanged;
            AdAction.OnAdPaid -= OnAdPaid;
            AdAction.OnAdClicked -= OnAdClicked;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
        }

        public virtual void Init(bool isRemoveAds)
        {
            if (IsInit)
            {
                return;
            }

            IsRemoveAds = isRemoveAds;

            IsInit = true;

            UpdateAdsConfig(adsConfig);

            Debug.LogWarning($"Ads - Init - isRemoveAds: {isRemoveAds}");
            if (consent.CanRequestAds)
            {
                InitAds();
            }
            else
            {
                consent.Request();
            }
        }

        protected abstract void UpdateAdsConfig(AdsConfig adsConfig);

        protected virtual void OnRequestConsentCompleted(bool isSuccess)
        {
            Debug.LogWarning($"Ads - OnRequestConsentCompleted {isSuccess}");
            if (isSuccess)
            {
                InitAds();
            }
        }

        protected virtual void InitAds()
        {
            admob.Init(adsConfig);
            applovin.Init(adsConfig);
            if (!adsConfig.app_open_ad_enabled) return;
            admob.AppOpenAd.SetShowResumeGame(CanShowAppOpenAd(AppOpenResumeGame));
            applovin.AppOpenAd.SetShowResumeGame(CanShowAppOpenAd(AppOpenResumeGame));
        }

        protected virtual void OnAdClicked(AdEventData adEventData)
        {
        }

        protected virtual void OnAdPaid(AdPaidEventData adPaidEventData)
        {
        }

        #region AppOpenAd

        private void OnAppOpenAdResumeGameCanShowChanged(bool canShow)
        {
            canShowAppOpenAdResumeGame = canShow;
        }

        protected virtual void OnAppStateChanged(AppState state)
        {
            if (state == AppState.Foreground && canShowAppOpenAdResumeGame && isStartGame)
            {
                Invoke(nameof(ShowAppOpenAdResumeGame), 0.1f);
            }
        }

        public virtual void ShowAppOpenAd(Action<bool> OnAdClose, string adPlacement = AppOpenStartGame)
        {
            if (IsRemoveAds || !CanShowAppOpenAd(adPlacement))
            {
                OnAppOpenAdClose(false, OnAdClose, adPlacement);
                return;
            }

            switch (adsConfig.app_open_ad_network)
            {
                case AdNetwork.applovin:
                    applovin.AppOpenAd.Show(b => { OnAppOpenAdClose(b, OnAdClose, adPlacement); }, adPlacement);
                    break;
                case AdNetwork.admob:
                    admob.AppOpenAd.Show(b => { OnAppOpenAdClose(b, OnAdClose, adPlacement); }, adPlacement);
                    break;
                default:
                    break;
            }
        }

        private void OnAppOpenAdClose(bool b, Action<bool> OnAdClose, string adPlacement)
        {
            if (adPlacement == AppOpenStartGame)
            {
                isStartGame = true;
            }

            OnAdClose?.Invoke(b);
        }

        protected virtual void ShowAppOpenAdResumeGame()
        {
            ShowAppOpenAd(null, AppOpenResumeGame);
        }

        public virtual bool CanShowAppOpenAd(string adPlacement)
        {
            return adsConfig.app_open_ad_enabled && CanShowAppOpenAdInternal(adPlacement);
        }

        protected abstract bool CanShowAppOpenAdInternal(string adPlacement);

        #endregion

        #region Banner

        public void HideBannerAd()
        {
            switch (adsConfig.banner_ad_network)
            {
                case AdNetwork.admob:
                    admob.BannerAd.Show(false, "");
                    break;
                case AdNetwork.applovin:
                    applovin.BannerAd.Show(false, "");
                    break;
            }
        }

        public virtual void ShowBannerAd(string adPlacement)
        {
            if (IsRemoveAds || !CanShowBannerAd(adPlacement)) return;

            switch (adsConfig.banner_ad_network)
            {
                case AdNetwork.admob:
                    admob.BannerAd.Show(true, adPlacement);
                    break;
                case AdNetwork.applovin:
                    applovin.BannerAd.Show(true, adPlacement);
                    break;
            }
        }

        public virtual void ShowBannerAd()
        {
            if (IsRemoveAds || !adsConfig.banner_ad_enabled) return;

            switch (adsConfig.banner_ad_network)
            {
                case AdNetwork.admob:
                    admob.BannerAd.Show();
                    break;
                case AdNetwork.applovin:
                    applovin.BannerAd.Show();
                    break;
            }
        }

        public virtual bool CanShowBannerAd(string adPlacement)
        {
            return adsConfig.banner_ad_enabled && CanShowBannerAdInternal(adPlacement);
        }

        protected abstract bool CanShowBannerAdInternal(string adPlacement);

        public float GetBannerHeightInPixels()
        {
#if UNITY_EDITOR
            return 170;
#elif UNITY_ANDROID || UNITY_IOS
            return adsConfig.banner_ad_network switch
            {
                AdNetwork.admob => admob.BannerAd.GetHeightInPixels(),
                AdNetwork.applovin => applovin.BannerAd.GetHeightInPixels(),
                _ => 170
            };
#endif
        }

        #endregion

        // #region BannerCollapsible
        //
        // public void ShowBannerCollapsibleAd(string adPlacement)
        // {
        //     if (!CanShowBannerCollapsibleAd(adPlacement)) return;
        //
        //     admob.BannerCollapsibleAd.Show(adPlacement);
        // }
        //
        // public virtual bool CanShowBannerCollapsibleAd(string adPlacement)
        // {
        //     return adsConfig.BannerCollapsibleAdEnabled && CanShowBannerCollapsibleAdInternal(adPlacement);
        // }
        //
        // protected abstract bool CanShowBannerCollapsibleAdInternal(string adPlacement);
        //
        // #endregion

        #region Interstitial

        public bool IsInterstitialAdReady()
        {
            return adsConfig.interstitial_ad_network switch
            {
                AdNetwork.applovin => applovin.InterstitialAd.IsAdReady(),
                AdNetwork.admob => admob.InterstitialAd.IsAdReady(),
                _ => false
            };
        }

        public void ShowInterstitialAd(Action<bool> OnAdClose, string adPlacement)
        {
            if (IsRemoveAds
                || !CanShowInterstitialAd(adPlacement)
                || !IsInterstitialAdReady()
                || !CanShowInterstitialAfterCooldown(adPlacement)
               )
            {
                OnAdClose?.Invoke(false);
                return;
            }

            switch (adsConfig.interstitial_ad_network)
            {
                case AdNetwork.applovin:
                    applovin.InterstitialAd.Show(
                        isShowBeforeClose => { OnInterstitialAdClose(isShowBeforeClose, OnAdClose); },
                        adPlacement);
                    break;
                case AdNetwork.admob:
                    admob.InterstitialAd.Show(
                        isShowBeforeClose => { OnInterstitialAdClose(isShowBeforeClose, OnAdClose); },
                        adPlacement);
                    break;
            }
        }

        private void OnInterstitialAdClose(bool isShowBeforeClose, Action<bool> OnAdClose)
        {
            if (isShowBeforeClose)
            {
                timeShowInterstitial = Time.time;
            }

            OnAdClose?.Invoke(isShowBeforeClose);
        }

        protected virtual bool CanShowInterstitialAfterCooldown(string adPlacement)
        {
            return timeShowInterstitial == 0 ||
                   Time.time - timeShowInterstitial >= GetInterstitialAdCoolDown(adPlacement);
        }

        public virtual bool CanShowInterstitialAd(string adPlacement)
        {
            return adsConfig.interstitial_ad_enabled && CanShowInterstitialAdInternal(adPlacement);
        }

        protected abstract bool CanShowInterstitialAdInternal(string adPlacement);

        protected abstract float GetInterstitialAdCoolDown(string adPlacement);

        #endregion

        #region RewardedAd

        public bool IsRewardedAdReady()
        {
            switch (adsConfig.rewarded_ad_network)
            {
                case AdNetwork.admob:
                    return admob.RewardedAd.IsAdReady();
                case AdNetwork.applovin:
                    return applovin.RewardedAdAd.IsAdReady();
                default:
                    return false;
            }
        }

        public void ShowRewardedAd(Action<bool> OnAdDisplayed, Action<bool> OnAdReceived, string adPlacement)
        {
            if (!CanShowRewardedAd(adPlacement))
            {
                OnAdDisplayed?.Invoke(false);
                return;
            }

            switch (adsConfig.rewarded_ad_network)
            {
                case AdNetwork.admob:
                    admob.RewardedAd.Show(OnAdDisplayed, OnAdReceived, adPlacement);
                    break;
                case AdNetwork.applovin:
                    applovin.RewardedAdAd.Show(OnAdDisplayed, OnAdReceived, adPlacement);
                    break;
            }
        }

        public virtual bool CanShowRewardedAd(string adPlacement)
        {
            return adsConfig.rewarded_ad_enabled && CanShowRewardedAdInternal(adPlacement);
        }

        protected abstract bool CanShowRewardedAdInternal(string adPlacement);

        #endregion

        public virtual void RemoveAds()
        {
            IsRemoveAds = true;
            HideBannerAd();
        }
    }
}