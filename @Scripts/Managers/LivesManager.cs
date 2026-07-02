using System;
using UnityEngine;

public class LivesManager : ContentManager {

    #region Const.

    public const int RefillInterval = 30;

    #endregion

    #region Properties

    public int Max => PlayerData.MaxHeart;
    public int Current => Heart.Value;
    public long UnlimitedRemainTime => HeartUnlimitedTime.Value;
    public bool IsUnlimited => UnlimitedRemainTime != TimeUtilities.TimeMin;
    public bool IsFull => Current >= Max;
    public bool UseHeart => IsUnlimited || Current > 0;

    #endregion

    #region Fields
    
    private Data<int> Heart => PlayerData.Heart;
    private Data<long> HeartUnlimitedTime => PlayerData.HeartUnlimitedTime;
    private Data<long> HeartUseTime => PlayerData.LastHeartUsedTime;

    private int _lastRefillFrame = -1;

    #endregion

    #region Initialize

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        // #1. 앱이 꺼져 있던 동안 회복되었을 목숨과, 지났을 무한 타이머를 반영해 업데이트.
        UpdateLives(false);
        UpdateUnlimited();
        
        // #2. 리필 타이머 초기화.
        InitializeRefillTime();
        
        return true;
    }
    
    private void InitializeRefillTime() {
        // 현재 목숨이 최대치라면, 타이머 종료.
        if (Current >= Max) {
            HeartUseTime.Value = TimeUtilities.TimeMin;
            return;
        }
        
        // 타이머가 꺼져있으나 목숨이 최대치가 아니라면, 타이머 시작.
        if (HeartUseTime.Value == Def.TimeMin) HeartUseTime.Value = TimeUtilities.TimeCurrent;
    }

    #endregion

    public void Update() {
        UpdateUnlimited();
        UpdateLives();
    }

    #region Lives

    public void Add(int amount = 1) {
        if (amount <= 0) return;
        int newLives = Math.Min(Max, Current + amount);
        Heart.Value = newLives;
        if (newLives >= Max) HeartUseTime.Value = Def.TimeMin;
    }
    
    public bool Use(int amount = 1) {
        if (IsUnlimited) return true;
        if (Current < amount) return false;
        Heart.Value -= amount;
        if (HeartUseTime.Value == Def.TimeMin) HeartUseTime.Value = TimeUtilities.TimeCurrent;
        return true;
    }

    #endregion

    #region Refill

    // 다음 회복까지 남은 시간 구하기.
    public NanaTime GetRemainRefillTime() {
        // #1. 현재 상태 불러오기: 가득 찼거나 타이머가 꺼져있다면 0 리턴.
        long livesUseTime = HeartUseTime.Value;
        int currentLives = Heart.Value;
        if (livesUseTime == TimeUtilities.TimeMin || currentLives >= Max) return new(0);

        // #2. 경과한 분 수 계산.
        float elapsedMinutes = TimeUtilities.GetElapsedTime(livesUseTime);

        // #3. 막 회복이 발생해야 할 시점이라면, 먼저 업데이트.
        if (Mathf.Abs(elapsedMinutes % RefillInterval) < 0.01f && elapsedMinutes > 0) {
            UpdateLives();
            livesUseTime = HeartUseTime.Value;
            if (Current >= Max || livesUseTime == TimeUtilities.TimeMin) return new(0);
        }

        // #4. 남은 시간 리턴.
        float remainingMinutes = RefillInterval - elapsedMinutes % RefillInterval;
        if (Mathf.Abs(remainingMinutes - RefillInterval) < 0.01f) remainingMinutes = 0f;
        return new((int)(remainingMinutes * 60));
    }

    private void UpdateLives(bool frameGuard = true) {
        // #1. 프레임 가드: 한 프레임에 한 번만 계산.
        if (frameGuard) {
            if (_lastRefillFrame == Time.frameCount) return;
            _lastRefillFrame = Time.frameCount;
        }
        
        // #2. 현재 상태 불러오기: 가득 찼거나 타이머가 꺼져있다면 계산이 필요하지 않음.
        long livesUseTime = HeartUseTime.Value;
        int currentLives = Heart.Value;
        if (livesUseTime == TimeUtilities.TimeMin || currentLives >= Max) return;
        
        // #3. 경과 시간을 Interval로 나누어 회복 가능한 양을 계산.
        int refillAmount = (int)(TimeUtilities.GetElapsedTime(livesUseTime) / RefillInterval);
        if (refillAmount <= 0) return;
        
        // #4. 회북 후의 새 목숨 양 계산: 최대치를 벗어나지 않도록 함.
        int newLives = Math.Min(currentLives + refillAmount, Max);
        if (newLives == currentLives) return;
        
        // #5. 새 목숨 값 적용.
        Heart.Value = newLives;
        
        // #6. 시간 보정: 회복된 양만큼 소모 시각을 앞당겨, 남은 쿨타임이 정확히 이어지도록 함. 가득 찼다면 타이머 종료.
        HeartUseTime.Value = TimeUtilities.GetLaterTime(livesUseTime, refillAmount * RefillInterval);
        if (newLives >= Max) HeartUseTime.Value = TimeUtilities.TimeMin;
    }

    #endregion

    #region LivesUnlimited

    public void AddUnlimitedTime(int min) {
        if (min <= 0) return;
        if (UnlimitedRemainTime == TimeUtilities.TimeMin)
            HeartUnlimitedTime.Value = TimeUtilities.GetLaterTime(min);
        else {
            try {
                HeartUnlimitedTime.Value =
                    DateTime.FromBinary(UnlimitedRemainTime).AddMinutes(min).ToBinary();
            }
            catch (Exception) {
                HeartUnlimitedTime.Value = TimeUtilities.GetLaterTime(min);
            }
        }
    }
    
    public NanaTime GetRemainUnlimitedTime() {
        if (UnlimitedRemainTime == TimeUtilities.TimeMin) return new(0);
        return new(TimeUtilities.GetRemaingSeconds(UnlimitedRemainTime));
    }
    
    private void UpdateUnlimited() {
        if (UnlimitedRemainTime == TimeUtilities.TimeMin) return;
        if (TimeUtilities.GetRemaingSeconds(UnlimitedRemainTime) <= 0)
            HeartUnlimitedTime.Value = TimeUtilities.TimeMin;
    }

    #endregion

    #region BonusLives

    public void AddBonusLives() {
        //_luckyPrefs.BonusLives = 3;
        Add(3);
    }

    #endregion

}