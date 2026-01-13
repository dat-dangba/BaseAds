using System;
using UnityEngine;

public class BannerAdApplovin : BaseApplovin
{
    // public static Action<bool> OnAdLoaded;
    private bool isShow;

    public void Show(bool isShow, string adPlacement)
    {
        this.adPlacement = adPlacement;
        this.isShow = isShow;

        if (!isInit) return;

        if (isShow)
        {
            MaxSdk.ShowBanner(adUnitId);
        }
        else
        {
            MaxSdk.HideBanner(adUnitId);
        }
    }

    public void Show()
    {
        isShow = true;

        if (!isInit) return;

        if (isShow)
        {
            MaxSdk.ShowBanner(adUnitId);
        }
        else
        {
            MaxSdk.HideBanner(adUnitId);
        }
    }

    public float GetHeightInPixels()
    {
        float dpHeight = MaxSdkUtils.GetAdaptiveBannerHeight() * 1920 / Screen.height;
        float pixels = dpHeight * MaxSdkUtils.GetScreenDensity();
        return pixels + 5;
    }

    protected override AdFormat GetAdFormat()
    {
        return AdFormat.BANNER;
    }

    protected override void SetAdEvent()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    protected override void OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        base.OnAdLoadedEvent(adUnitId, adInfo);
        if (isShow)
        {
            Show(isShow, adPlacement);
        }
        // OnAdLoaded?.Invoke(true);
    }

    protected override void OnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        base.OnAdDisplayFailedEvent(adUnitId, errorInfo, adInfo);
        // OnAdLoaded?.Invoke(false);
    }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo info)
    {
        Debug.LogWarning($"Ads - Applovin {GetAdFormat()} OnBannerAdCollapsedEvent");
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.LogWarning($"Ads - Applovin {GetAdFormat()} OnBannerAdExpandedEvent");
    }

    protected override void LoadAd()
    {
        MaxSdk.CreateBanner(adUnitId, new MaxSdkBase.AdViewConfiguration(MaxSdkBase.AdViewPosition.BottomCenter));
        MaxSdk.SetBannerBackgroundColor(adUnitId, Color.white);
    }
}