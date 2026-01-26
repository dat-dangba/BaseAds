using UnityEngine;
using static MaxSdkBase;

namespace DBD.Ads
{
    public abstract class BaseApplovin : MonoBehaviour
    {
        protected bool isInit;
        protected string adUnitId = "";
        protected string adPlacement = "";

        protected abstract AdFormat GetAdFormat();

        protected abstract void SetAdEvent();

        protected abstract void LoadAd();

        public void LoadAd(string adUnitId)
        {
            if (isInit || string.IsNullOrEmpty(adUnitId)) return;
            this.adUnitId = adUnitId;
            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Load");
            SetAdEvent();
            LoadAd();
            isInit = true;
        }

        protected virtual void OnAdLoadedEvent(string adUnitId, AdInfo adInfo)
        {
            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Loaded");
        }

        protected virtual void OnAdLoadFailedEvent(string adUnitId, ErrorInfo errorInfo)
        {
            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Load Failed");
        }

        protected virtual void OnAdDisplayedEvent(string adUnitId, AdInfo adInfo)
        {
            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Displayed");
        }

        protected virtual void OnAdDisplayFailedEvent(string adUnitId, ErrorInfo errorInfo, AdInfo adInfo)
        {
            AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(true);
            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Display Failed");
        }

        protected virtual void OnAdClickedEvent(string adUnitId, AdInfo adInfo)
        {
            AdEventData adEventData = new AdEventData
            {
                AdNetwork = AdNetwork.applovin,
                AdFormat = GetAdFormat(),
                AdPlacement = adPlacement
            };
            AdAction.OnAdClicked?.Invoke(adEventData);

            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Clicked");
        }

        protected virtual void OnAdHiddenEvent(string adUnitId, AdInfo adInfo)
        {
            AdAction.OnAppOpenAdResumeGameCanShowChanged?.Invoke(true);
            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Hidden");
        }

        protected virtual void OnAdRevenuePaidEvent(string adUnitId, AdInfo adInfo)
        {
            AdPaidEventData adPaidEventData = new AdPaidEventData
            {
                AdNetwork = AdNetwork.applovin,
                AdFormat = GetAdFormat(),
                AdPlacement = adPlacement,
                Revenue = adInfo.Revenue,
                Currency = "USD",
                RevenueNetwork = adInfo.NetworkName,
                AdRevenueUnit = adInfo.AdUnitIdentifier
            };
            AdAction.OnAdPaid?.Invoke(adPaidEventData);

            Debug.LogWarning($"Ads - Applovin {GetAdFormat()} Revenue Paid");
        }
    }
}