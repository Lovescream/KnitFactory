using UnityEngine;

public class EUI_Panel_BoxQueueProperties : UI_Panel {

    #region Properties

    public EditorBoxQueue BoxQueue { get; private set; }

    #endregion

    #region Fields

    private EditorScene _scene;
    private EUI_BoxQueueEditor _editor;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        _editor = this.gameObject.FindChild<EUI_BoxQueueEditor>();
        this.gameObject.FindChild<UI_Button>("btnApply").SetEvent(OnButtonApply);
        this.gameObject.FindChild<UI_Button>("btnDelete").SetEvent(OnButtonDelete);
        
        return true;
    }

    public void Set(EditorBoxQueue boxQueue) {
        Initialize();
        BoxQueue = boxQueue;
        _scene = Main.Scene.Current as EditorScene;
        if (_scene == null) return;

        _editor.Set(boxQueue.Data);

        _scene.SceneUI.SetPageButtons(false, false);
        EditorScene.EditorState = EditorState.BoxQueueProperties;
    }

    #endregion

    #region Events

    private void OnButtonApply() {
        BoxQueue.Board.RecalculateCounts();
        _scene.SceneUI.SetPageButtons(true, true);
        EditorScene.EditorState = EditorState.EditBelts;
        _scene.BeltBoard.SetSelectCursor();
        Close();
    }

    private void OnButtonDelete() {
        _scene.BeltBoard.RemoveBoxQueue(BoxQueue);
        _scene.BeltBoard.RecalculateCounts();
        _scene.SceneUI.SetPageButtons(true, true);
        EditorScene.EditorState = EditorState.EditBelts;
        _scene.BeltBoard.SetSelectCursor();
        Close();
    }

    #endregion

}