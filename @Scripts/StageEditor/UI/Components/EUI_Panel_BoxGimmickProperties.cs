using System;
using UnityEngine;

public class EUI_Panel_BoxGimmickProperties : UI_Panel
{
    #region Properties

    public EUI_BoxGimmickElement Element { get; private set; }
    public BoxGimmickData Data => Element.Data;

    #endregion
    
    #region Fields

    private EUI_Dropdown _dropdownType;
    private EUI_InputField _inputCount;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        _dropdownType = this.gameObject.FindChild<EUI_Dropdown>("Type");
        _inputCount = this.gameObject.FindChild<EUI_InputField>("Count");
        this.gameObject.FindChild<UI_Button>("btnApply").SetEvent(OnButtonApply);
        this.gameObject.FindChild<UI_Button>("btnCancel").SetEvent(OnButtonCancel);
        
        return true;
    }

    public void Set(EUI_BoxGimmickElement element) {
        Initialize();
        Element = element;

        _dropdownType.OnValueChanged += OnTypeChanged;
        _dropdownType.Set("Type", typeof(BoxGimmickType), Data.Type, true);
        _inputCount.Set("Count", Data.Count.ToString());
        
        OnTypeChanged(Data.Type);
    }

    #endregion

    #region Events

    private void OnTypeChanged(Enum type) {
        switch (type) {
            case BoxGimmickType.None:
                SetActiveData(0);
                break;
            case BoxGimmickType.Hidden:
                SetActiveData(0);
                break;
            case BoxGimmickType.Connector:
                SetActiveData((int)(BoxDataGroup.Count));
                break;
        }
    }

    private void OnButtonApply() {
        Data.Type = (BoxGimmickType)_dropdownType.Value;
        Data.Count = int.Parse(_inputCount.Text);
        Element.SetTypeText();
        this.Close();
    }

    private void OnButtonCancel() {
        this.Close();
    }

    private void SetActiveData(int groupIndex)
    {
        _inputCount.gameObject.SetActive(((int)BoxDataGroup.Count & groupIndex) != 0);
    }

    #endregion
}

[Flags]
public enum BoxDataGroup
{
    Count = 1 >> 0,
}