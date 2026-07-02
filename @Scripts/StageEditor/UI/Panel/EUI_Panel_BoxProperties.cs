using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EUI_Panel_BoxProperties : UI_Panel {

    #region Properties

    public BoxData BoxData { get; private set; }

    #endregion

    #region Fields

    private TMP_InputField _fieldIndex;
    private EUI_Dropdown _dropdownColor;
    private EUI_BoxGimmickEditor _gimmickEditor;
    private EUI_BoxQueueElement _queueElement;
    private UI_Image _imgBox;

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

        _fieldIndex = this.gameObject.FindChild<TMP_InputField>("fieldIndex");
        _dropdownColor = this.gameObject.FindChild<EUI_Dropdown>("Setting_Color");
        _gimmickEditor = this.gameObject.FindChild<EUI_BoxGimmickEditor>();
        this.gameObject.FindChild<UI_Button>("btnApply").SetEvent(OnButtonApply);
        this.gameObject.FindChild<UI_Button>("btnDeleteBox").SetEvent(OnButtonDelete);
        _imgBox = this.gameObject.FindChild<UI_Image>("Img_Box");
        
        return true;
    }

    public void Set(EUI_BoxQueueElement editor) {
        Initialize();
        BoxData = editor.Data;
        SetColor(editor.Data.Color);
        _queueElement = editor;
        _dropdownColor.Set("Color", typeof(ColorType), editor.Data.Color, false);
        _dropdownColor.OnValueChanged += SetColor;
        _gimmickEditor.Set(editor.Data);
    }

    #endregion

    #region Events

    private void OnButtonApply() {
        ColorType color = (ColorType)_dropdownColor.Value;
        _queueElement.ChangeAngle(color);
        _queueElement.SetGimmick(_gimmickEditor.GetGimmickDataList());
        this.Close();
    }

    private void OnButtonDelete() {
        _queueElement.BoxQueueEditor.DeleteElement(BoxData);
        
        this.Close();
    }

    #endregion

    public void SetColor(Enum color)
    {
        _imgBox.Sprite = Main.Resource.Get<Sprite>($"Box_Vertical_{color}_B");
    }
}