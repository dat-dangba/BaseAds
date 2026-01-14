using System;

namespace DBD.Ads
{
    public class RewardedAdApplovin : BaseApplovin
    {
        private Action<bool> OnAdDisplayed;
        private Action<bool> OnAdReceived;

        private int retryAttempt;
        private bool isRewarded;

        public void Show(Action<bool> OnAdDisplayed, Action<bool> OnAdReceived, string adPlacement)
        {
            isRewarded = false;
            if (IsAdReady())
            {
                this.adPlacement = adPlacement;
                this.OnAdDisplayed = OnAdDisplayed;
                this.OnAdReceived = OnAdReceived;

                AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(false);
                MaxSdk.ShowRewardedAd(adUnitId);
            }
            else
            {
                OnAdDisplayed?.Invoke(false);
            }
        }

        public bool IsAdReady()
        {
            return MaxSdk.IsRewardedAdReady(adUnitId);
        }

        protected override AdFormat GetAdFormat()
        {
            return AdFormat.REWARDED;
        }

        protected override void SetAdEvent()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
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

        protected override void OnAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdDisplayedEvent(adUnitId, adInfo);
            OnAdDisplayed?.Invoke(true);
        }

        protected override void OnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdDisplayFailedEvent(adUnitId, errorInfo, adInfo);
            OnAdDisplayed?.Invoke(false);
            LoadAd();
        }

        protected override void OnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            base.OnAdHiddenEvent(adUnitId, adInfo);
            LoadAd();
            OnAdReceived?.Invoke(isRewarded);
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            isRewarded = true;
        }

        protected override void LoadAd()
        {
            MaxSdk.LoadRewardedAd(adUnitId);
        }
    }
}