using System;
using GoogleMobileAds.Api;

public class RewardedAdAdmob : BaseAdmob
{
    private RewardedAd rewardedAd;

    private Action<bool> OnAdReceived;
    private Action<bool> OnAdDisplayed;
    private bool isUserRewardEarned;

    public void Show(Action<bool> OnAdDisplayed, Action<bool> OnAdReceived, string adPlacement)
    {
        this.adPlacement = adPlacement;
        this.OnAdDisplayed = OnAdDisplayed;
        this.OnAdReceived = OnAdReceived;
        isUserRewardEarned = false;
        if (IsAdReady())
        {
            AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(false);
            rewardedAd.Show(_ => { isUserRewardEarned = true; });
        }
        else
        {
            this.OnAdDisplayed?.Invoke(false);
        }
    }

    public bool IsAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    protected override void LoadAd()
    {
        if (isLoading)
        {
            return;
        }

        if (rewardedAd != null)
        {
            if (rewardedAd.CanShowAd())
            {
                return;
            }

            rewardedAd.Destroy();
            rewardedAd = null;
        }

        isLoading = true;
        var adRequest = new AdRequest();

        RewardedAd.Load(adUnitId, adRequest,
            (ad, error) =>
            {
                isLoading = false;
                if (error != null || ad == null)
                {
                    OnAdLoadFailedEvent(error);
                    ReloadAd();
                    return;
                }

                rewardedAd = ad;
                OnAdLoadedEvent(rewardedAd.GetResponseInfo());
                SetAdEvent(ad);
            });
    }

    private void SetAdEvent(RewardedAd ad)
    {
        ad.OnAdPaid += OnAdPaidEvent;
        ad.OnAdImpressionRecorded += OnAdImpressionRecordedEvent;
        ad.OnAdClicked += OnAdClickedEvent;
        ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpenedEvent;
        ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosedEvent;
        ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailedEvent;
    }

    protected override void OnAdFullScreenContentOpenedEvent()
    {
        base.OnAdFullScreenContentOpenedEvent();
        OnAdDisplayed?.Invoke(true);
    }

    protected override void OnAdFullScreenContentClosedEvent()
    {
        base.OnAdFullScreenContentClosedEvent();
        ReloadAd();
        OnAdReceived?.Invoke(isUserRewardEarned);
    }

    protected override void OnAdFullScreenContentFailedEvent(AdError adError)
    {
        base.OnAdFullScreenContentFailedEvent(adError);
        ReloadAd();
        OnAdDisplayed?.Invoke(false);
    }

    protected override AdFormat GetAdFormat()
    {
        return AdFormat.REWARDED;
    }
}