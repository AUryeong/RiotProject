using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager : Singleton<AdMobManager>
{
    private BannerView bannerViewAd;
    private static readonly AdSize AD_BANNER_SIZE = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
    private const string AD_BANNER_UNIT_ID = "ca-app-pub-3940256099942544/6300978111";
    
    private RewardedAd rewardAd;
    private const string AD_REWARD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917";

    public void ShowBannerView()
    {
        if (bannerViewAd == null)
        {
            bannerViewAd = new BannerView(AD_BANNER_UNIT_ID, AD_BANNER_SIZE, AdPosition.Top);
            bannerViewAd.LoadAd(new AdRequest());
        }
    }

    public int GetADSizeY()
    {
        if (bannerViewAd == null)
            ShowBannerView();

        return Mathf.CeilToInt(bannerViewAd.GetHeightInPixels());
    }

    public void ShowRewardAd(Action action)
    {
        if (rewardAd != null)
        {
            rewardAd.Destroy();
            rewardAd = null;
        }

        RewardedAd.Load(AD_REWARD_UNIT_ID, new AdRequest(),
            (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.Log("rewarded ad failed to load an ad " +
                              "with error : " + error);
                    return;
                }

                rewardAd = ad;
                if (rewardAd != null && rewardAd.CanShowAd())
                {
                    rewardAd.Show((Reward reward) => { action?.Invoke(); });
                }
            });
    }
}