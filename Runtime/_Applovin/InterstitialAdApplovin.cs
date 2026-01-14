using System;

namespace DBD.Ads
{
    public class InterstitialAdApplovin : BaseApplovin
    {
        private Action<bool> OnAdClose;

        private int retryAttempt;

        public void Show(Action<bool> OnAdClose, string adPlacement)
        {
            this.OnAdClose = OnAdClose;
            this.adPlacement = adPlacement;
            if (IsAdReady())
            {
                AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(false);
                MaxSdk.ShowInterstitial(adUnitId);
            }
            else
            {
                this.OnAdClose?.Invoke(false);
            }
        }

        public bool IsAdReady()
        {
            return MaxSdk.IsInterstitialReady(adUnitId);
        }

        protected override AdFormat GetAdFormat()
        {
            return AdFormat.INTERSTITIAL;
        }

        protected override void SetAdEvent()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnAdDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnAdClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

        protected override void OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdLoadedEvent(adUnitId, adInfo);
            retryAttempt = 0;
        }

        protected override void OnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            base.OnAdLoadFailedEvent(adUnitId, errorInfo);
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            Invoke(nameof(LoadAd), (float)retryDelay);
        }

        protected override void OnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdDisplayFailedEvent(adUnitId, errorInfo, adInfo);
            OnAdClose?.Invoke(false);
            LoadAd();
        }

        protected override void OnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdHiddenEvent(adUnitId, adInfo);
            OnAdClose?.Invoke(true);
            LoadAd();
        }

        protected override void LoadAd()
        {
            MaxSdk.LoadInterstitial(adUnitId);
        }
    }
}