using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorScene : SceneBase {

    #region Properties

    public static StageDataKey CurrentStage { get; set; }
    public static EditorState EditorState { get; set; } = EditorState = EditorState.None;
    public UI_EditorScene SceneUI { get; private set; }
    public EditorController InputController { get; private set; }
    public EditorBeltBoard BeltBoard { get; private set; }
    public IReadOnlyList<BundleData> BundleDataList => CurrentStage.Bundles;

    #endregion

    public static bool CanClick = true; 

    #region MonoBehaviours

    void OnDisable() {
        InputController?.OnDestroy();
        InputController = null;
    }

    protected override void Update() {
        base.Update();
        if (!CanClick) return;
        
        if (Keyboard.current.digit1Key.wasPressedThisFrame) BeltBoard?.SetBeltCursor();
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) BeltBoard?.SetBoxQueueCursor();
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) BeltBoard?.SetSelectCursor();
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) BeltBoard?.SetMoveMachineCursor();
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) BeltBoard?.SetKnitSsagaeCursor();
        else if (Keyboard.current.digit6Key.wasPressedThisFrame) BeltBoard?.SetKnitKeeperCursor();
    }

    #endregion

    #region Initialize / Set

    protected override bool Initialize() {
        if (!base.Initialize()) return false;

        // #1. 상태 초기화.
        EditorState = EditorState.None;
        
        // #2. 새 스테이지 데이터 생성.
        CurrentStage = new();
        
        // #3. UI 설정.
        SceneUI = FindFirstObjectByType<UI_EditorScene>();
        SceneUI.Set(this);
        Main.UI.OpenPanel<EUI_Panel_StageProperties>().Set(true);
        
        // #4. InputController 설정.
        InputController = new();
        
        return true;
    }

    #endregion

    #region Generate / Edit Board
    
    public void GenerateNewBeltBoard() {
        BeltBoard = new EditorBeltBoard();
        EditorState = EditorState.EditBelts;
        SceneUI.SetPageButtons(true, true);
        BeltBoard?.SetSelectCursor();
    }
    
    public void EditBeltBoard() {
        if (BeltBoard == null) return; // 이런 경우는 없어야 하느니라.
        BeltBoard.Show();
        EditorState = EditorState.EditBelts;
        SceneUI.SetPageButtons(true, true);
    }

    public void ApplyBeltBoard() {
        CurrentStage.Belts = BeltBoard.GetBeltData();
        CurrentStage.BoxQueues = BeltBoard.GetBoxQueueData();
        CurrentStage.MoveMachines = BeltBoard.GetMoveMachineData();
        CurrentStage.KnitKeepers = BeltBoard.GetKnitKeeperData();
        CurrentStage.KnitSsagaes = BeltBoard.GetKnitSsagaeData();
    }

    #endregion

    #region Edit Bundles

    public void ApplyBundles(List<BundleData> bundles) {
        CurrentStage.Bundles = bundles;
    }

    #endregion

}

public enum EditorState {
    None = -1,
    StageProperties = 0,
    EditBelts,
    BoxQueueProperties,
    BundlePanel,
    ConfirmInfo,
}