using UnityEngine;

public class UI_DockObject : Entity {

    #region Properties

    public Dock Dock { get; private set; }
    public Transform StartX { get; private set; }
    public Transform EndX { get; private set; }
    public Transform AddSpaceStartX { get; private set; }
    public Transform AddSpaceEndX { get; private set; }
    public Transform SlotSpawnPos { get; private set; }

    #endregion

    #region Fields

    private UI_Button _btnSwitch;
    private UI_Text _txtGold;
    private UI_Text _txtDesc;
    
    #endregion
    
    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        StartX = gameObject.FindChild<Transform>("StartX");
        EndX = gameObject.FindChild<Transform>("EndX");
        AddSpaceStartX = gameObject.FindChild<Transform>("AddSpaceStartX");
        AddSpaceEndX = gameObject.FindChild<Transform>("AddSpaceEndX");
        SlotSpawnPos = gameObject.FindChild<Transform>("SlotSpawnPos");
        _btnSwitch = gameObject.FindChild<UI_Button>("Btn_Dock_Switch");
        _txtGold = gameObject.FindChild<UI_Text>("Txt_Gold");
        _txtDesc = gameObject.FindChild<UI_Text>("Txt_Desc");

        return true;
    }

    public void Set(Dock dock, Transform parent) {
        Initialize();
        Dock = dock;
        _btnSwitch.SetEvent(OnClickAddSlot);
        this.transform.SetParent(parent, false);
        SetAddSlotText();
    }

    #endregion

    private void OnClickAddSlot()
    {
        if (PlayerData.Gold.Value < GameScene.DockAddSlotCoin) return;
        PlayerData.Gold.Value -= GameScene.DockAddSlotCoin;
        Dock.AddDockSlot(Dock.AddSlotCount);
    }
    
    public Transform GetBtnTr() => _btnSwitch.transform;
    public void SetActiveBtn(bool active) => _btnSwitch.SetActive(active, false);

    // TODO:Local 추가 필요
    public void SetAddSlotText()
    {
        _txtGold.Text = GameScene.DockAddSlotCoin.ToString();
        _txtDesc.Text = "+4 Slot";
    }

    // TODO:Local 추가 필요
    public void SetAddSpaceText()
    {
        _txtGold.Text = GameScene.FailedDockAddSpaceCoin.ToString();
        _txtDesc.Text = "Add Space";
    }
}