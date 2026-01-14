using System;
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace DBD.Ads
{
    public class Consent : MonoBehaviour
    {
        public static Action<bool> OnRequestConsentCompleted;

        public bool CanRequestAds => ConsentInformation.CanRequestAds();

        private bool isRequesting;

        private readonly List<string> testDeviceHashedIds = new()
        {
            "9CA1D3E2210C1524430AE8AD5E43FB2F",
            "9D03B074F14540C9E2D85B996F71BE68",
            "765626E4543F069813787A0E41A85828",
            "B73463623F3D4843699CD557A2A4294A",
            "2C0A75075ABC8C8BAB0356835C7A275F",
            "77AFB31B3CF62C76DE4942458679D204",
            "7DEEDF7207EBF3D40957CD7684DF32F4",
            "769D01ECC4161046A5513940614D37A3",
            "43F12E55D196C05E675CD124E17F33C7",
            "F81160A24E9B955A69DCAC3429E44583",
            "F1DF4FB5B47E934B956E24C49543FBDE",
            "A19BC289F4CD061F232373D4AD5423D6",
            "998C61310E679AABD6FF5EABD47835FB",
            "D9C811F47909748DAECB9CD9353394BF",
            "ABD9DF6DD8F4365CDFA9BC8BBB79F0AD",
            "E5B670688D2CB3CF2570DB2253358E82",
            "B570A02F6F3697DDE40E0006D50104CE",
            "AFC4DCC2721868E8C2ECED01B2485740",
            "11C04073F0D3941B2901D20E73CFEBB7",
            "B4450C90F86F513AEF334A21987C0F66",
            "17610C08332F50C97E6CD2CA4A49C6F5",
            "7CCA69A069853ED899D680D2C35300F9",
            "F399502DA515FD5D9FE512D2377E878C",
            "56E2B11E1950D8A3DECAB0C11CB770CC",
            "A89F0A148F398FB52A0277FCD9B8C462",
            "90D9022B87F8B06E4E65B08684F6A6A7",
            "303E37FA4CDB74564D1C1E2BA6E3A09E",
            "26B54C15D6D13F05BE30E7DE97C7F447",
            "8C42079F834FBEBAF7DD96DD8EBE5487",
            "D2914ACF8DC3D1A0D7E19F95ADDE7247",
            "B4450C90F86F513AEF334A21987C0F66",
            "B730FB960974868CE3BC0B175EB33496",
            "4F1505FAAC208A3AE3C29D8BC7596AD7",
            "56E2B11E1950D8A3DECAB0C11CB770CC",
            "881A8CDA98CF3FD6F05631CA7B7B48B4",
            "81A89BDB7696BCAEB85550C5FA8FE5BD",
            "A2700B237DFADAC749D9FDB3882E22FF",
            "12FF267531127826C5924159FA14C8C1",
            "F399502DA515FD5D9FE512D2377E878C",
            "47D481BB45CD94D21977A317366EDF52",
            "17610C08332F50C97E6CD2CA4A49C6F5",
            "BF730B5C34320F67CD989155EC9B8FD4",
            "AFB325D4E9B651E9881AB3D394AED5D9",
            "1CE782C7B6D2339F8DC28FA42127D5A7",
            "52F0C06CB5FE55CF35995E2943C42503",
            "DC1E4D9EE243EAF7A008427D8AEA063F",
            "BB052A7EF4D2361E43075244EC1EDB81",
            "3C62346842D71E7A11B081FC0A9C47A4",
            "A6FBF3850FC1B311E5380FF03C0E0EC2",
            "868BED09BCFC0CB45319E0D18E118F51",
            "6DD24D5F8DA0A8D94BE55A138EFC8B41",
            "12538C8CFF4E71F4B3DA3D9FF33BE582",
            "D71314D933428CA09C4C49B26BD6422C",
            "52FD6C041B9B0981393EEB469EC9CE67",
            "3479D79715A1E7E1F89FCE280E575367"
        };

        public void Request()
        {
            if (isRequesting) return;

            isRequesting = true;

            var debugSettings = new ConsentDebugSettings
            {
                DebugGeography = DebugGeography.RegulatedUSState,
                TestDeviceHashedIds = testDeviceHashedIds
            };

            ConsentRequestParameters request = new()
            {
                ConsentDebugSettings = debugSettings,
            };

            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        private void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                Debug.LogWarning($"Consent - {consentError.ErrorCode} {consentError.Message}");
                OnRequestConsentCompleted?.Invoke(false);
                isRequesting = false;
                return;
            }

            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                    Debug.LogWarning($"Consent - {consentError.ErrorCode} {consentError.Message}");
                    OnRequestConsentCompleted?.Invoke(false);
                    isRequesting = false;
                    return;
                }

                OnRequestConsentCompleted?.Invoke(CanRequestAds);
                isRequesting = false;
            });
        }

        [ContextMenu("Reset Consent")]
        public void ResetConsentInformation()
        {
            ConsentInformation.Reset();
        }
    }
}