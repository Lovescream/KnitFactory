using System;
using ActionFit_Plugin.SDK;
using UnityEngine;

public class AdsManager : ContentManager {

    #region Const.

    // TODO:: 광고 제거 여부 임시 false로 고정. 나중에 광고 제거 적용 로직 추가
    public const bool IsRemovedAds = false;
    private const float FailInterstitialInterval = 120f;
    private const float ClearInterstitialInterval = 120f;

    #endregion

    #region Fields

    private bool _isBannerEnabled;
    
    private float _minFailInterstitialTime;
    private float _minClearInterstitialTime;

    #endregion

    #region Initialize

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        // ADSCallback.OnDelayInvokeStarted -= OnShowLoading;
        // ADSCallback.OnDelayInvokeStarted += OnShowLoading;
        // ADSCallback.OnDelayInvokeFinished -= OnHideLoading;
        // ADSCallback.OnDelayInvokeFinished += OnHideLoading;
        
        return true;
    }

    #endregion

    #region Loading

    private void OnShowLoading() => Main.Loading.Show(LoadingType.Ads);
    private void OnHideLoading() => Main.Loading.Hide();

    #endregion

    #region Banner

    public void EnableBanner() {
        if (CanShowBanner()) {
            //ADS.EnableBanner();
            _isBannerEnabled = true;
        }
    }

    public void DisableBanner() {
        if (!_isBannerEnabled) return;
        try {
            //ADS.DisableBanner();
        }
        catch (Exception e) {
            Debug.LogWarning($"[AdsManager] DisableBanner(): An error occurred while disabling banner: {e.Message}");
        }
        finally {
            _isBannerEnabled = false;
        }
    }

    private bool CanShowBanner() {
        if (IsRemovedAds) return false;
        if (_isBannerEnabled) return false;
        return true;
    }

    #endregion

    #region Interstitial

    public void TryShowInterstitial(Action action = null, Action failAction = null, bool isAddCheck = false)
    {
        if (IsRemovedAds || isAddCheck)
        {
            action?.Invoke();
        }
        else
        {
            Main.Loading.Show(LoadingType.Ads);
            action += Main.Loading.Hide;
            failAction += Main.Loading.Hide;
            SDKManager.ShowInterstitial(action, failAction);
        }
    }
    
    public bool ShowFailInterstitial(Action<bool> cbOnCompleted = null) {
        if (IsRemovedAds) return false;
        if (!CanShowFailInterstitial()) return false;

        // ADS.ShowInterstitial(isSuccess => {
        //     cbOnCompleted?.Invoke(isSuccess);
        //     if (isSuccess) _minFailInterstitialTime = Time.time + FailInterstitialInterval;
        // }, ignoreCondition: true);
        return true;
    }
    
    private bool CanShowFailInterstitial() {
        if (Time.time < _minFailInterstitialTime) return false;
        return true;
    }

    public bool ShowClearInterstitial(Action<bool> cbOnCompleted = null) {
        if (IsRemovedAds) return false;
        if (!CanShowClearInterstitial()) return false;
        
        // ADS.ShowInterstitial(isSuccess => {
        //     cbOnCompleted?.Invoke(isSuccess);
        //     if (isSuccess) _minClearInterstitialTime = Time.time + ClearInterstitialInterval;
        // });
        return true;
    }

    private bool CanShowClearInterstitial() {
        if (Time.time < _minClearInterstitialTime) return false;
        return true;
    }
    
    #endregion

    #region Reward

    public void ShowReward(Action action = null, Action failAction = null) {
        Main.Loading.Show(LoadingType.Ads);
        action += Main.Loading.Hide;
        failAction += Main.Loading.Hide;
        SDKManager.ShowReward(action, failAction);
    }

    #endregion

}