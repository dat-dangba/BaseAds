using System;

namespace DBD.Ads
{
    public class AppOpenAdApplovin : BaseApplovin
    {
        public static event Action<bool> OnAdLoaded;
        private Action<bool> OnAdClose;

        private bool isShowResumeGame;
        private int retryAttempt;

        public void Show(Action<bool> OnAdClose, string adPlacement)
        {
            this.OnAdClose = OnAdClose;
            this.adPlacement = adPlacement;
            if (MaxSdk.IsAppOpenAdReady(adUnitId))
            {
                AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(false);
                MaxSdk.ShowAppOpenAd(adUnitId);
            }
            else
            {
                OnAdClose?.Invoke(false);
            }
        }

        public void SetShowResumeGame(bool b)
        {
            isShowResumeGame = b;
        }

        protected override AdFormat GetAdFormat()
        {
            return AdFormat.APP_OPEN;
        }

        protected override void SetAdEvent()
        {
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAdDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAdClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAdHiddenEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

        protected override void LoadAd()
        {
            MaxSdk.LoadAppOpenAd(adUnitId);
        }

        protected override void OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdLoadedEvent(adUnitId, adInfo);
            retryAttempt = 0;
            OnAdLoaded?.Invoke(true);
        }

        protected override void OnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            base.OnAdLoadFailedEvent(adUnitId, errorInfo);
            OnAdLoaded?.Invoke(false);

            if (!isShowResumeGame) return;
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            Invoke(nameof(LoadAd), (float)retryDelay);
        }

        protected override void OnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdDisplayFailedEvent(adUnitId, errorInfo, adInfo);
            OnAdClose?.Invoke(false);
            if (!isShowResumeGame) return;
            LoadAd();
        }

        protected override void OnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdHiddenEvent(adUnitId, adInfo);
            OnAdClose?.Invoke(true);
            if (!isShowResumeGame) return;
            LoadAd();
        }
    }
}