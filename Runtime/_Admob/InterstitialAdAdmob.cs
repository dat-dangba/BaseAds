using System;
using GoogleMobileAds.Api;

namespace DBD.Ads
{
    public class InterstitialAdAdmob : BaseAdmob
    {
        private InterstitialAd interstitialAd;

        private Action<bool> OnAdClose;

        public void Show(Action<bool> OnAdClose, string adPlacement)
        {
            this.OnAdClose = OnAdClose;
            this.adPlacement = adPlacement;
            if (IsAdReady())
            {
                AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(false);
                interstitialAd.Show();
            }
            else
            {
                this.OnAdClose?.Invoke(false);
            }
        }

        public bool IsAdReady()
        {
            return interstitialAd != null && interstitialAd.CanShowAd();
        }

        protected override void LoadAd()
        {
            if (isLoading)
            {
                return;
            }

            if (interstitialAd != null)
            {
                if (interstitialAd.CanShowAd())
                {
                    return;
                }

                interstitialAd.Destroy();
                interstitialAd = null;
            }

            var adRequest = new AdRequest();

            isLoading = true;
            InterstitialAd.Load(adUnitId, adRequest, (ad, error) =>
            {
                isLoading = false;
                if (error != null || ad == null)
                {
                    OnAdLoadFailedEvent(error);
                    ReloadAd();
                    return;
                }

                interstitialAd = ad;
                OnAdLoadedEvent(interstitialAd.GetResponseInfo());
                SetAdEvent(interstitialAd);
            });
        }

        private void SetAdEvent(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdPaid += OnAdPaidEvent;
            interstitialAd.OnAdImpressionRecorded += OnAdImpressionRecordedEvent;
            interstitialAd.OnAdClicked += OnAdClickedEvent;
            interstitialAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpenedEvent;
            interstitialAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosedEvent;
            interstitialAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailedEvent;
        }

        protected override void OnAdFullScreenContentClosedEvent()
        {
            base.OnAdFullScreenContentClosedEvent();
            OnAdClose?.Invoke(true);
            ReloadAd();
        }

        protected override void OnAdFullScreenContentFailedEvent(AdError adError)
        {
            base.OnAdFullScreenContentFailedEvent(adError);
            OnAdClose?.Invoke(false);
            ReloadAd();
        }

        protected override AdFormat GetAdFormat()
        {
            return AdFormat.INTERSTITIAL;
        }
    }
}