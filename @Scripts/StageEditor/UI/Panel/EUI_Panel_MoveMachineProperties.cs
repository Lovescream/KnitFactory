using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EUI_Panel_MoveMachineProperties : UI_Panel {

    #region Properties

    public EditorMoveMachine EditorMoveMachine { get; private set; }

    #endregion

    #region Fields

    private EditorScene _scene;
    private TMP_InputField _fieldIndex;
    private EUI_Dropdown _dropdownAngle;
    private EUI_BoxQueueElement _queueElement;
    private UI_Image _imgMM;

    #endregion

    #region MonoBehaviours

    protected override void Update() {
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame) {
            OnButtonApply();
        }
    }

    #endregion
    
    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _fieldIndex = this.gameObject.FindChild<TMP_InputField>("InputField");
        _dropdownAngle = this.gameObject.FindChild<EUI_Dropdown>("Setting_Angle");
        this.gameObject.FindChild<UI_Button>("btnApply").SetEvent(OnButtonApply);
        this.gameObject.FindChild<UI_Button>("btnDeleteBox").SetEvent(OnButtonDelete);
        _imgMM = this.gameObject.FindChild<UI_Image>("Img_MM");
        
        return true;
    }

    public void Set(EditorMoveMachine editorMoveMachine) {
        Initialize();
        _scene = Main.Scene.Current as EditorScene;
        EditorMoveMachine = editorMoveMachine;
        SetAngle(editorMoveMachine.Orientation);
        _dropdownAngle.Set("Angle", typeof(Orientation), editorMoveMachine.Orientation, false);
        _dropdownAngle.OnValueChanged += SetAngle;
        _fieldIndex.text = editorMoveMachine.ConnectIndex.ToString();
    }

    #endregion

    #region Events

    private void OnButtonApply() {
        _dropdownAngle.OnValueChanged -= SetAngle;
        Orientation selectOrientation = (Orientation)_dropdownAngle.Value;
        EditorMoveMachine.SetOrientation(selectOrientation);
        EditorMoveMachine.SetSprite();
        EditorMoveMachine.SetConnectIndex(int.Parse(_fieldIndex.text));
        EditorMoveMachine.Board.RecalculateCounts();
        _scene.SceneUI.SetPageButtons(true, true);
        EditorScene.EditorState = EditorState.EditBelts;
        _scene.BeltBoard.SetSelectCursor();
        Close();
    }

    private void OnButtonDelete() {
        _dropdownAngle.OnValueChanged -= SetAngle;
        EditorMoveMachine.Board.RemoveMoveMachine(EditorMoveMachine);
        EditorMoveMachine.Board.RecalculateCounts();
        _scene.SceneUI.SetPageButtons(true, true);
        EditorScene.EditorState = EditorState.EditBelts;
        _scene.BeltBoard.SetSelectCursor();
        Close();
    }

    #endregion

    public void SetAngle(Enum angle)
    {
        _imgMM.Sprite = Main.Resource.Get<Sprite>($"Knit_MoveMachine_{angle}");
    }
}