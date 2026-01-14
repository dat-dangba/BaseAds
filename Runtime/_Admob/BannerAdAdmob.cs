using GoogleMobileAds.Api;
using UnityEngine;

namespace DBD.Ads
{
    public class BannerAdAdmob : BaseAdmob
    {
        private BannerView bannerView;
        private bool isShow;

        public void Show(bool isShow, string adPlacement)
        {
            this.adPlacement = adPlacement;
            this.isShow = isShow;

            if (!isInit) return;

            if (isShow)
            {
                bannerView?.Show();
            }
            else
            {
                bannerView?.Hide();
            }
        }

        public void Show()
        {
            isShow = true;

            if (!isInit) return;

            if (isShow)
            {
                bannerView?.Show();
            }
            else
            {
                bannerView?.Hide();
            }
        }

        public float GetHeightInPixels()
        {
            return bannerView?.GetHeightInPixels() * 1920 / Screen.height + 5 ?? 0;
        }

        protected override void LoadAd()
        {
            if (bannerView == null)
            {
                CreateBanner();
            }

            var adRequest = new AdRequest();
            bannerView?.LoadAd(adRequest);
            bannerView?.Hide();
        }

        private void CreateBanner()
        {
            if (bannerView != null)
            {
                DestroyBanner();
            }

            CreateBannerView();
            SetAdEvent();
        }

        private void CreateBannerView()
        {
            bannerView = new BannerView(adUnitId,
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
            // bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        }

        private void SetAdEvent()
        {
            bannerView.OnBannerAdLoaded += OnBannerAdLoadedEvent;

            bannerView.OnBannerAdLoadFailed += OnAdLoadFailedEvent;
            bannerView.OnAdPaid += OnAdPaidEvent;
            bannerView.OnAdImpressionRecorded += OnAdImpressionRecordedEvent;
            bannerView.OnAdClicked += OnAdClickedEvent;
        }

        protected override void OnAdLoadFailedEvent(LoadAdError error)
        {
            base.OnAdLoadFailedEvent(error);
            ReloadAd();
        }

        private void OnBannerAdLoadedEvent()
        {
            retryAttempt = 0;
            responseInfo = bannerView.GetResponseInfo();
            if (isShow)
            {
                Show(isShow, adPlacement);
            }

            Debug.LogWarning($"Ads - Admob {GetAdFormat()} Loaded");
        }

        private void DestroyBanner()
        {
            if (bannerView == null) return;

            Show(false, "");
            bannerView.Destroy();
            bannerView = null;
        }

        protected override AdFormat GetAdFormat()
        {
            return AdFormat.BANNER;
        }
    }
}