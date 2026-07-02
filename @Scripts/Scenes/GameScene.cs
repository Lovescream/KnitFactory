using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameScene : SceneBase
{
    #region Const

    public const int DockAddSlotCoin = 400;
    public const int FailedDockAddSpaceCoin = 1000;
    public const int GameClearGold = 50;

    #endregion


    #region Properties

    public static GameState GameState
    {
        get => _gameState;
        set
        {
            if (_gameState == value) return;
            _gameState = value;
            if (_gameState == GameState.InTutorial)
            {
                (Main.Scene.Current as GameScene)?.OnGameTutorial?.Invoke();
            }
            else if (_gameState == GameState.Playing)
            {
                GameEvents.OnGameStart?.Invoke();
                (Main.Scene.Current as GameScene)?.OnGameStart?.Invoke();
            }
            else if (_gameState == GameState.Success)
            {
                GameEvents.OnGameClear?.Invoke();
                if (Main.IsEditorMode) Main.Scene.Load("EditorScene");
                else
                {
                    // 성공할 경우 전면 광고 실행 검사
                    GameScene scene = Main.Scene.Current as GameScene;
                    if (scene is null) return;
                    Main.Ads.TryShowInterstitial
                    (
                        scene.SuccessGame,
                        null,
                        CurrentStage.Index < Main.So.AdsData.startInterstitial
                    );
                }
            }
            else if (_gameState == GameState.Failed)
            {
                GameEvents.OnGameOver?.Invoke();
                if (Main.IsEditorMode) Main.Scene.Load("EditorScene");
                else
                {
                    // 실패할 경우 전면 광고 실행 검사
                    GameScene scene = Main.Scene.Current as GameScene;
                    if (scene is null) return;
                    Main.Ads.TryShowInterstitial
                    (
                        scene.FailGame,
                        null,
                        CurrentStage.Index < Main.So.AdsData.startInterstitial
                    );
                }
            }
        }
    }

    public static InGameState InGameState
    {
        get => _inGameState;
        set
        {
            if (_inGameState == value) return;
            _inGameState = value;
            if (_inGameState == InGameState.Processing)
            {
                (Main.Scene.Current as GameScene)?.OnInGameProcessing?.Invoke();
            }
            else if (_inGameState == InGameState.Stop)
            {
                (Main.Scene.Current as GameScene)?.OnInGameStop?.Invoke();
            }
        }
    }
    //public static ItemType CurrentItemType { get; set; }

    public static StageDataKey CurrentStage { get; private set; }
    public UI_GameScene SceneUI { get; private set; }
    public InputController InputController { get; private set; }

    #endregion

    #region Fields

    public Camera mashimashiro;

    private static GameState _gameState = GameState.None;
    private static InGameState _inGameState = InGameState.None;

    private Coroutine _coEndGame;

    public event Action OnGameReady;
    public event Action OnGameTutorial;
    public event Action OnGameStart;
    public event Action OnInGameStop;
    public event Action OnInGameProcessing;

    #endregion

    #region MonoBehaviours

    protected override void Update()
    {
        base.Update();
        Main.Time.OnUpdate(Time.deltaTime);

        // TEMP!
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameState = GameState.Success;
        }
        else if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            GameState = GameState.Failed;
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Main.IsEditorMode)
            {
                GameState = GameState.None;
                Main.Scene.Load("EditorScene");
            }
        }
#endif
    }

    void OnDisable()
    {
        InputController?.OnDestroy();
    }

    #endregion

    protected override bool Initialize()
    {
        if (!base.Initialize()) return false;

        StartGame(PlayerData.ClearLevel.Value + 1);

        return true;
    }

    #region Game

    public void StartGame(int stage = -1)
    {
        ResetGame();
        StartCoroutine(InitGame(stage));
        Main.Lives.Use();
    }

    private void ResetGame()
    {
        Main.Clear();
        InputController?.OnDestroy();
        InputController.UsingItem = false;
        Main.Board.Clear();
    }

    private IEnumerator InitGame(int stage = -1)
    {
        // #1. Stage 불러오기.
        if (Main.IsEditorMode) CurrentStage = Main.Data.EditorStageDataKey;
        else
        {
            if (stage == -1) stage = PlayerData.ClearLevel.Value;
            SetStageData(stage);
        }

        Main.Screen.ResetCamera();

        yield return null;

        // #4. UI 생성.
        SceneUI = FindFirstObjectByType<UI_GameScene>();
        if (SceneUI == null) SceneUI = Main.UI.OpenSceneUI<UI_GameScene>();
        SceneUI.Set(this, CurrentStage);
        SceneUI.GenerateObject();
        yield return null;

        // #2. 개체 생성.
        Main.Board.GenerateBoard(CurrentStage, SceneUI);
        yield return null;

        // #5. 카메라 및 인풋 설정.
        Main.Screen.SetGameCameraSize();
        yield return null;
        Main.Screen.SetGameCameraPos();
        yield return null;

        // #3. 개체 오브젝트 생성.
        Main.Board.GenerateBoardObject();
        yield return null;
        SceneUI.GenerateSlotObject();
        InputController = new();
        InputController.AllowInput = true;

        // #6. 게임 시작.
        GameEvents.OnGameReady?.Invoke();
        OnGameReady?.Invoke();
        GameState = GameState.Ready;
        InGameState = InGameState.Processing;

        Main.Loading.Hide();
        yield return SpawnKnits();

        // 튜토리얼을 볼 수 있는 스테이지일 경우 튜토리얼 모드 실행
        TutorialLevelType type = GetTutorialLevel(CurrentStage.Index);
        if (type != TutorialLevelType.None)
        {
            Main.UI.OpenPanel<UI_Panel_Tutorial>().Set(type);
        }
    }

    public TutorialLevelType GetTutorialLevel(int level)
    {
        foreach (TutorialLevelType tutorialLevelType in Enum.GetValues(typeof(TutorialLevelType)))
        {
            if ((int)tutorialLevelType == level) return tutorialLevelType;
        }

        return TutorialLevelType.None;
    }

    private IEnumerator SpawnKnits()
    {
        yield return null;

        SceneUI.Bundle.SpawnAllKnitObjects();
    }

    private void SuccessGame()
    {
        PlayerData.ClearLevel.Value = CurrentStage.Index;
        PlayerData.Gold.Value += GameClearGold;
        if (_coEndGame != null) StopCoroutine(_coEndGame);
        Main.Lives.Add();
        _coEndGame = StartCoroutine(CoSuccessGame());
    }

    private IEnumerator CoSuccessGame()
    {
        yield return new WaitForSeconds(1f);
        Main.UI.OpenPanel<UI_Panel_StageCompleted>().Set();
        // Main.Audio.PlaySFX(SFX.GameClear);
    }

    private void FailGame()
    {
        if (CurrentStage.Index <= 5)
        {
            Main.UI.OpenPopup<UI_Popup_OutOfSpaceTuto>().Set();
        }
        else
        {
            Main.UI.OpenPanel<UI_Panel_NeedSpace>().Set();
        }
    }

    // 실패 후 로비로 이동
    public void FailedMoveLobby()
    {
        Main.UI.CloseAllUI();
        Main.Scene.SwitchAsync("LobbyScene");
    }

    // 리플레이 시도 하트가 부족할 경우 로비로 이동
    public void FailedGameRetry()
    {
        if (!Main.Lives.UseHeart)
        {
            FailedMoveLobby();
            return;
        }

        Main.UI.CloseAllUI();
        StartGame(CurrentStage.Index);
    }

    #endregion

    private void SetStageData(int stage)
    {
        if (stage == -1) CurrentStage = Main.Data.GetStageData(1);
        else CurrentStage = Main.Data.GetStageData(stage) ?? Main.Data.GetStageData(1);
    }

    // AddSpace형식으로 플레이를 재개하는 함수
    public bool PlayOnAddSpaceGame()
    {
        if (PlayerData.Gold.Value < FailedDockAddSpaceCoin) return false;

        GameState = GameState.Ready;
        InGameState = InGameState.Processing;

        int searchType = (int)KnitState.InSsagae |
                         (int)KnitState.MoveToBelt |
                         (int)KnitState.MoveToDockSlot |
                         (int)KnitState.MoveToSsagae |
                         (int)KnitState.OnMoveMachine |
                         (int)KnitState.OnBelt;

        // 생성된 모든 Knit을 검사해서 searchType과 같은 Knit들을 반환.
        List<Knit> sameTypeKnits = Main.Board.Bundle.GetSameTypesKnits(searchType);

        // DockSlot에 있는 마지막 털실 6개를 반환
        List<Knit> dockSlotKnits = Main.Board.Dock.GetLastDockSlots(6);

        List<Knit> addSpaceKnits = sameTypeKnits.Concat(dockSlotKnits).ToList();

        Main.Board.BeltBoard.Ssagae.RemoveAllKnits();

        foreach (Knit knit in addSpaceKnits)
        {
            knit.MoveToAddSpace();
        }

        PlayerData.Gold.Value -= FailedDockAddSpaceCoin;

        return true;
    }
}

public enum GameState
{
    None = -1,
    Ready = 0,
    InTutorial,
    Playing,
    Success,
    Failed,
}

public enum InGameState
{
    None = -1,
    Ready = 0,
    Processing,
    Stop,
    Hammer,
}