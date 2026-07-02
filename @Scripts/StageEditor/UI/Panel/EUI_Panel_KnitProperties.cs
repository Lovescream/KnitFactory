using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EUI_Panel_KnitProperties : UI_Panel {

    #region Properties

    public KnitData KnitData { get; private set; }
    public EUI_Bundle Bundle { get; private set; }

    #endregion

    #region Fields

    private TMP_InputField _fieldIndex;
    private EUI_Dropdown _dropdownColor;
    private EUI_KnitGimmickEditor _gimmickEditor;
    private List<EUI_Knit> _selectKnits = new();
    private readonly List<EUI_Knit> _copyKnits = new();
    private KnitLayoutGroup _knitGroup;

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
        _gimmickEditor = this.gameObject.FindChild<EUI_KnitGimmickEditor>();
        this.gameObject.FindChild<UI_Button>("btnApply").SetEvent(OnButtonApply);
        this.gameObject.FindChild<UI_Button>("btnDeleteBox").SetEvent(OnButtonDelete);
        _knitGroup = this.gameObject.FindChild<KnitLayoutGroup>("KnitBundle");
        
        return true;
    }
    public void NewKnit(KnitData data) {
        EUI_Knit knit = Main.UI.CreateComponent<EUI_Knit>(parent: _knitGroup.transform);
        knit.Set(Bundle, data);
        _copyKnits.Add(knit);
    }
    
    public void Set(EUI_Bundle bundle, List<EUI_Knit> dataList) {
        Initialize();
        foreach (EUI_Knit knit in _copyKnits)
        {
            Main.UI.Destroy(knit);
        }
        _copyKnits.Clear();
        Bundle = bundle;
        KnitData = dataList[0].Data;
        _selectKnits = dataList;
        _dropdownColor.Set("Color", typeof(ColorType), dataList[0].Data.Color, false);
        _dropdownColor.OnValueChanged += SetColor;
        _gimmickEditor.Set(dataList);
        foreach (EUI_Knit knit in _selectKnits)
        {
            NewKnit(knit.Data);
        };
        SetColor(dataList[0].Data.Color);
    }

    #endregion

    #region Events

    private void OnButtonApply() {
        ColorType color = (ColorType)_dropdownColor.Value;
        
        _selectKnits[0].SetGimmick(_gimmickEditor.GetGimmickOriginDataList());
        for (int i = 1; i < _selectKnits.Count; i++)
        {
            _selectKnits[i].ChangeColor(color);
            _selectKnits[i].SetGimmick(_gimmickEditor.GetGimmickRemoveDataList());
        }
        Bundle.ResetShowKnits();
        this.Close();
    }

    private void OnButtonDelete() {
        Bundle.RemoveKnit(_selectKnits);
        Bundle.ResetShowKnits();
        this.Close();
    }

    #endregion

    public void SetColor(Enum color)
    {
        foreach (EUI_Knit knit in _copyKnits)
        {
            knit.Sprite = Main.Resource.Get<Sprite>($"Knit_{color}");
        }
    }
}