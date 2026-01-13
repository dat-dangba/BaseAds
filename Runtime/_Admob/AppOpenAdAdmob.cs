using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdAdmob : BaseAdmob
{
    public static event Action<bool> OnAdLoaded;
    private Action<bool> OnAdClose;
    private bool isShowResumeGame;
    private AppOpenAd appOpenAd;

    public void Show(Action<bool> OnAdClose, string adPlacement)
    {
        this.adPlacement = adPlacement;
        this.OnAdClose = OnAdClose;
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            appOpenAd.Show();
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

    protected override void LoadAd()
    {
        if (isLoading)
        {
            return;
        }

        if (appOpenAd != null)
        {
            if (appOpenAd.CanShowAd())
            {
                return;
            }

            appOpenAd.Destroy();
            appOpenAd = null;
        }

        var adRequest = new AdRequest();
        isLoading = true;
        AppOpenAd.Load(adUnitId, adRequest, (ad, error) =>
        {
            isLoading = false;
            if (error != null || ad == null)
            {
                ReloadAd();
                return;
            }

            appOpenAd = ad;
            OnAdLoadedEvent(ad.GetResponseInfo());
            SetAdEvent(ad);
            OnAdLoaded?.Invoke(true);
        });
    }

    private void SetAdEvent(AppOpenAd ad)
    {
        ad.OnAdPaid += OnAdPaidEvent;
        ad.OnAdImpressionRecorded += OnAdImpressionRecordedEvent;
        ad.OnAdClicked += OnAdClickedEvent;
        ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpenedEvent;
        ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosedEvent;
        ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailedEvent;
    }

    protected override void OnAdFullScreenContentClosedEvent()
    {
        base.OnAdFullScreenContentClosedEvent();
        OnAdClose?.Invoke(true);
        if (isShowResumeGame)
        {
            ReloadAd();
        }
    }

    protected override void OnAdFullScreenContentFailedEvent(AdError adError)
    {
        base.OnAdFullScreenContentFailedEvent(adError);
        OnAdClose?.Invoke(false);
        if (isShowResumeGame)
        {
            ReloadAd();
        }
    }

    protected override AdFormat GetAdFormat()
    {
        return AdFormat.APP_OPEN;
    }
}