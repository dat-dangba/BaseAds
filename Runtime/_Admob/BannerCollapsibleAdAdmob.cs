using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class BannerCollapsibleAdAdmob : BaseAdmob
{
    public static Action<bool> OnAdDisplay;

    private BannerView bannerView;

    public void Show(string adPlacement)
    {
        if (bannerView == null) return;
        if (bannerView.IsCollapsible())
        {
            this.adPlacement = adPlacement;
            bannerView.Show();
        }
        else
        {
            ReloadAd();
        }
    }

    protected override void LoadAd()
    {
        if (bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        adRequest.Extras.Add("collapsible", "bottom");
        bannerView?.LoadAd(adRequest);
        bannerView?.Hide();
    }

    private void CreateBannerView()
    {
        if (bannerView != null)
        {
            DestroyBanner();
        }

        bannerView = new BannerView(adUnitId,
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);

        SetAdEvent();
    }

    private void SetAdEvent()
    {
        bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        bannerView.OnBannerAdLoadFailed += OnAdLoadFailedEvent;

        bannerView.OnAdPaid += OnAdPaidEvent;
        bannerView.OnAdImpressionRecorded += OnAdImpressionRecordedEvent;
        bannerView.OnAdClicked += OnAdClickedEvent;

        bannerView.OnAdFullScreenContentOpened += OnAdFullScreenContentOpenedEvent;
        bannerView.OnAdFullScreenContentClosed += OnAdFullScreenContentClosedEvent;
    }

    protected override void OnAdLoadFailedEvent(LoadAdError error)
    {
        base.OnAdLoadFailedEvent(error);
        ReloadAd();
    }

    private void OnBannerAdLoaded()
    {
        retryAttempt = 0;
        responseInfo = bannerView.GetResponseInfo();
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Loaded");
    }

    protected override void OnAdFullScreenContentOpenedEvent()
    {
        base.OnAdFullScreenContentOpenedEvent();
        OnAdDisplay?.Invoke(true);
    }

    protected override void OnAdFullScreenContentClosedEvent()
    {
        base.OnAdFullScreenContentClosedEvent();
        DestroyBanner();
        OnAdDisplay?.Invoke(false);
        ReloadAd();
    }

    private void DestroyBanner()
    {
        if (bannerView == null) return;

        bannerView.Hide();
        bannerView.Destroy();
        bannerView = null;
    }

    protected override AdFormat GetAdFormat()
    {
        return AdFormat.BANNER_COLLAPSIBLE;
    }
}