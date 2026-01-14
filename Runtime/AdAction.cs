using System;
using GoogleMobileAds.Api;

namespace DBD.Ads
{
    public static class AdAction
    {
        public static Action<AdPaidEventData> OnAdPaid;
        public static Action<AdEventData> OnAdClicked;
        public static Action<bool> OnAppOpenAdResumeGameCanShowChanged;
    }
}