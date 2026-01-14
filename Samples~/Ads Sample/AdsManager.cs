using UnityEngine;

public class AdsManager : BaseAdsManager<AdsManager>
{
    protected override bool CanShowAppOpenAdInternal(string adPlacement)
    {
        return true;
    }

    protected override bool CanShowBannerAdInternal(string adPlacement)
    {
        return true;
    }

    // protected override bool CanShowBannerCollapsibleAdInternal(string adPlacement)
    // {
    //     return true;
    // }

    protected override bool CanShowInterstitialAdInternal(string adPlacement)
    {
        return true;
    }

    protected override float GetInterstitialAdCoolDown(string adPlacement)
    {
        return 0;
        // return adsConfig.InterstitialAdCoolDown;
    }

    protected override bool CanShowRewardedAdInternal(string adPlacement)
    {
        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        AppOpenAdApplovin.OnAdLoaded += AppOpenApplovinOnAdLoaded;
        BannerCollapsibleAdAdmob.OnAdDisplay += BannerCollapsibleAdOnAdDisplay;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        AppOpenAdApplovin.OnAdLoaded -= AppOpenApplovinOnAdLoaded;
        BannerCollapsibleAdAdmob.OnAdDisplay -= BannerCollapsibleAdOnAdDisplay;
    }

    private void BannerCollapsibleAdOnAdDisplay(bool active)
    {
        if (active)
        {
            HideBannerAd();
        }
        else
        {
            ShowBannerAd();
        }
    }

    private void AppOpenApplovinOnAdLoaded(bool isLoaded)
    {
    }

    protected override void Start()
    {
        base.Start();
        Init(false);
        ShowBanner();
    }

    protected override void UpdateAdsConfig(AdsConfig adsConfig)
    {
    }

    public void ShowAppOpenAd()
    {
        ShowAppOpenAd(b => { Debug.Log($"datdb - Close Ads Open"); });
    }

    private bool isShowBanner;

    public void ShowBanner()
    {
        isShowBanner = !isShowBanner;
        if (isShowBanner)
        {
            ShowBannerAd("banner_home");
        }
        else
        {
            HideBannerAd();
            Debug.Log($"datdb - GetBannerHeightInPixels {GetBannerHeightInPixels()}");
        }
    }

    public void ShowInterstitial()
    {
        ShowInterstitialAd(b => { }, "");
    }

    public void ShowRewardedAd()
    {
        ShowRewardedAd(isDisplayed => { Debug.Log($"datdb - isDisplayed {isDisplayed}"); },
            isReceived => { Debug.Log($"datdb - isReceived {isReceived}"); }, "");
    }

    // public void ShowBannerCollapsible()
    // {
    //     ShowBannerCollapsibleAd("");
    // }
}