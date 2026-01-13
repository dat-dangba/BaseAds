using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public abstract class BaseAdmob : MonoBehaviour
{
    protected bool isInit;
    protected string adUnitId = "";
    protected string adPlacement = "";

    protected bool isLoading;

    protected int retryAttempt;
    private bool needReloadAd;

    protected ResponseInfo responseInfo;

    protected abstract void LoadAd();

    protected abstract AdFormat GetAdFormat();

    public void LoadAd(string adUnitId)
    {
        if (isInit || string.IsNullOrEmpty(adUnitId)) return;
        this.adUnitId = adUnitId;
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Load");
        LoadAd();
        isInit = true;
    }

    protected virtual void Update()
    {
        if (needReloadAd && Application.internetReachability != NetworkReachability.NotReachable)
        {
            needReloadAd = false;
            LoadAd();
        }
    }

    protected void ReloadAd()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            double retryDelay = retryAttempt == 0 ? 0 : Math.Pow(2, Math.Min(6, retryAttempt));
            // Invoke(nameof(LoadAd), (float)retryDelay);
            StartCoroutine(LoadAdCoroutine((float)retryDelay));
            retryAttempt++;
        }
        else
        {
            needReloadAd = true;
        }
    }

    private IEnumerator LoadAdCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadAd();
    }

    protected virtual void OnAdLoadedEvent(ResponseInfo responseInfo)
    {
        retryAttempt = 0;
        this.responseInfo = responseInfo;
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Loaded");
    }

    protected virtual void OnAdLoadFailedEvent(LoadAdError error)
    {
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Load Failed");
    }

    // Raised when the ad is estimated to have earned money.
    protected virtual void OnAdPaidEvent(AdValue adValue)
    {
        AdPaidEventData adPaidEventData = new AdPaidEventData
        {
            AdNetwork = AdNetwork.Admob,
            AdFormat = GetAdFormat(),
            AdPlacement = adPlacement,
            Revenue = adValue.Value / 1000000f,
            Currency = adValue.CurrencyCode,
            RevenueNetwork = responseInfo != null ? responseInfo.GetLoadedAdapterResponseInfo().AdSourceName : "",
            AdRevenueUnit = responseInfo != null ? responseInfo.GetLoadedAdapterResponseInfo().AdSourceId : ""
        };
        AdAction.OnAdPaid?.Invoke(adPaidEventData);

        Debug.LogWarning($"Ads - Admob {GetAdFormat()} {adValue.Value} Revenue Paid");
    }

    // Raised when an impression is recorded for an ad.
    protected virtual void OnAdImpressionRecordedEvent()
    {
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Impression");
    }

    // Raised when a click is recorded for an ad.
    protected virtual void OnAdClickedEvent()
    {
        AdEventData adEventData = new AdEventData
        {
            AdNetwork = AdNetwork.Applovin,
            AdFormat = GetAdFormat(),
            AdPlacement = adPlacement
        };
        AdAction.OnAdClicked?.Invoke(adEventData);

        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Clicked");
    }

    // Raised when an ad opened full screen content.
    protected virtual void OnAdFullScreenContentOpenedEvent()
    {
        AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(false);
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Opened");
    }

    // Raised when the ad closed full screen content.
    protected virtual void OnAdFullScreenContentClosedEvent()
    {
        AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(true);
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Closed");
    }

    // Raised when the ad failed to open full screen content. Display failed
    protected virtual void OnAdFullScreenContentFailedEvent(AdError adError)
    {
        AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(true);
        Debug.LogWarning($"Ads - Admob {GetAdFormat()} Content Failed");
    }
}