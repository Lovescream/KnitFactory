using System;
using System.Collections;
using System.Collections.Generic;
using JSAM;
using UnityEngine;

public class LobbyScene : SceneBase
{
    #region Properties

    public Light DirectionalLight { get; private set; }
    public UI_LobbyScene LobbyUI { get; private set; }

    public static LobbyState LobbyState
    {
        get => _lobbyState;
        set
        {
            if (_lobbyState == value) return;
            _lobbyState = value;
            if (LobbyState == LobbyState.Ready)
            {
                (Main.Scene.Current as LobbyScene)?.OnLobbyReady?.Invoke();
            }
            else if (LobbyState == LobbyState.Start)
            {
                (Main.Scene.Current as LobbyScene)?.OnLobbyStart?.Invoke();
            }
        }
    }

    #endregion

    #region Fields
    
    // 팝업 체인 시스템
    private Queue<PopupInfo> _popupQueue = new();
    private bool _isProcessingPopups = false;
    private static LobbyState _lobbyState;

    public event Action OnLobbyReady;
    public event Action OnLobbyStart;

    #endregion

    #region Popup Info Class

    private class PopupInfo
    {
        public System.Func<bool> Condition { get; set; }
        public System.Func<UI_Popup> OpenAction { get; set; }
        public string Name { get; set; }

        public PopupInfo(string name, System.Func<bool> condition, System.Func<UI_Popup> openAction)
        {
            Name = name;
            Condition = condition;
            OpenAction = openAction;
        }
    }

    #endregion

    private void OnEnable()
    {
        LobbyState = LobbyState.Ready;
    }

    protected override bool Initialize()
    {
        if(!base.Initialize()) return false;
        
        DirectionalLight = gameObject.FindChild<Light>("Directional Light");
        
        Main.Loading.Hide();
        
        StartCoroutine(InitializeLobbySequence());

        //광고 팝업을 열 수 있는 상태라면, 시간제 광고 보유 여부에 따라 팝업을 오픈.
        // if (Main.Ads.CanShowAdsPopup())
        // {
        //     if (Main.Life.IsAdsUnlimited) Main.UI.OpenPopup<UI_AdsStandardPopup>();
        //     else Main.UI.OpenPopup<UI_AdsBuyPopup>();
        // }
        
        return true;
    }

    private IEnumerator InitializeLobbySequence()
    {
        // #00. Scene Audio Sounds(BGM)
        AudioManager.StopAllMusic();
        // AudioManager.PlayMusic(BgmKey.BgmLobby);
        
        // #01. UI Setup - 먼저 UI를 생성하고 초기화만
        LobbyUI = FindFirstObjectByType<UI_LobbyScene>();
        if (!LobbyUI)
        {
            LobbyUI = Main.UI.Instantiate<UI_LobbyScene>(false);
        }

        // #02. UI 초기화 및 홈페이지로 강제 설정
        yield return StartCoroutine(InitializeUIWithHomePage());

        yield return null;
        
        // #04. Sequence Start
        SequenceEntryPoint();
    }

    private IEnumerator InitializeUIWithHomePage()
    {
        // UI 초기화
        LobbyUI.Initialize();
        
        // 홈페이지로 즉시 설정 (애니메이션 없이)
        LobbyUI.Nav.NavigateTo(PageType.Lobby, immediate: true);
        
        // 한 프레임 대기하여 UI가 완전히 설정되도록 함
        yield return null;
        
        // 최종 Set 호출
        LobbyUI.Set(this);
    }

    private void SequenceEntryPoint()
    {
        PopupSequence();
        return;
        
        //남은 골드 이동 애니메이션이 있는지 확인하고 있다면 바로 팝업 실행 후 리턴.
        // var difference = _userPrefs.Gold.Value - _userPrefs.Gold.VisualValue;
        // if (difference <= 0)
        // {
        //     PopupSequence();
        //     return;
        // }
        //
        // var pageHome = LobbyUI.Page.GetPage<UI_PageHome>(ePageType.Lobby);
        // var startPosition = pageHome.StartRoot.position;
        //
        // RewardProvider.Play(RewardKey.Gold, (int)difference, 5, startPosition, PopupSequence);
    }

    #region Popup Chain System

    public void PopupSequence()
    {
        // 팝업 체인 설정 (순서 중요!)
        SetupPopupChain();
        
        // 체인 시작
        StartPopupChain();
    }

    private void SetupPopupChain()
    {
        _popupQueue.Clear();

        // 1. 평점 팝업 (레벨 조건 + 미수락)
        // _popupQueue.Enqueue(new PopupInfo(
        //     "Rating",
        //     () => !_userPrefs.IsAcceptedRating && _userPrefs.Level.Value == Constant.Level_Rating,
        //     () => Main.UI.OpenPopup<UI_Rating>()
        // ));

        // 추가 팝업들...
        // _popupQueue.Enqueue(new PopupInfo(...));
    }

    private void StartPopupChain()
    {
        if (_isProcessingPopups) return;
        
        _isProcessingPopups = true;
        ProcessNextPopup();
    }

    private void ProcessNextPopup()
    {
        // 큐가 비었으면 종료
        if (_popupQueue.Count == 0)
        {
            _isProcessingPopups = false;
            return;
        }

        var popupInfo = _popupQueue.Dequeue();
        if (!popupInfo.Condition())
        {
            ProcessNextPopup();
            return;
        }

        var popup = popupInfo.OpenAction();
        if (!popup)
        {
            ProcessNextPopup();
            return;
        }
        
        SetupPopupCloseCallback(popup);
    }

    private void SetupPopupCloseCallback(UI_Popup popup)
    {
        var originalCloseAction = popup.OnClosedAction;
        popup.OnClosedAction = () =>
        {
            originalCloseAction?.Invoke();
            ProcessNextPopup();
        };
    }

    /// <summary>
    /// 체인 강제 중단
    /// </summary>
    public void StopPopupChain()
    {
        _popupQueue.Clear();
        _isProcessingPopups = false;
    }

    /// <summary>
    /// 특정 팝업만 즉시 열기 (체인 무시)
    /// </summary>
    public void OpenPopupImmediate<T>() where T : UI_Popup
    {
        Main.UI.OpenPopup<T>();
    }

    #endregion
    
    private void OnDestroy()
    {
        if (!AudioManager.Instance || !AudioManagerInternal.Instance) return;
        
        AudioManager.StopAllMusic();
    }
}

public enum LobbyState
{
    None = -1,
    Ready,
    Start,
}