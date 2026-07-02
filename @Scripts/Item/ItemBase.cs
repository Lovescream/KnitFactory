using UnityEngine;

public abstract class ItemBase : InitBehaviour
{
    protected int Price;
    protected int UnLockLevel;

    private UI_Text _textItemCount;
    private UI_Text _textPrice;
    private UI_Text _textUnLock;
    private UI_Image _imgCountBG;
    private UI_Image _buyBG;
    private UI_Image _imgIcon;
    private UI_Button _btnSlot;
    protected Data<int> ItemData;

    protected abstract Data<int> GetItemData();
    protected abstract Sprite GetIconSprite();
    protected abstract string GetItemName();
    protected abstract string GetItemDesc();
    protected abstract int GetPrice();
    protected abstract int GetUnLockLevel();
    protected abstract bool CanUseItem();
    protected abstract void TryUseItem();

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;
        InitComponent();
        Price = GetPrice();
        UnLockLevel = GetUnLockLevel();
        _btnSlot.SetEvent(OnClickUseItem);
        _textPrice.Text = Price.ToString();
        ItemData = GetItemData();
        ItemData.OnChange += SetItemCountText;
        Set();
        GameEvents.OnGameReady += Set;
        return true;
    }

    protected virtual void Set()
    {
        SetItemCountText(ItemData.Value);
        InitUnLock();
        SetNewItemPopup();
    }

    protected virtual void OnDisable()
    {
        ItemData.OnChange -= SetItemCountText;
        GameEvents.OnGameReady -= Set;
    }

    private void InitComponent()
    {
        _textUnLock = gameObject.FindChild<UI_Text>("Text_UnLock");
        _imgIcon = gameObject.FindChild<UI_Image>("Img_Icon");
        _textItemCount = gameObject.FindChild<UI_Text>("Text_Count");
        _btnSlot = gameObject.FindChild<UI_Button>("Btn_Slot");
        _imgCountBG = gameObject.FindChild<UI_Image>("Img_CountBG");
        _buyBG = gameObject.FindChild<UI_Image>("Img_BuyBG");
        _textPrice = gameObject.FindChild<UI_Text>("Text_Price");
    }

    protected virtual void OnClickUseItem()
    {
        Initialize();
        if (!CanUseItem()) return;
        if (ItemData.Value > 0) ItemData.Value--;
        else if (PlayerData.Gold.Value >= Price) PlayerData.Gold.Value -= Price;
        else
        {
            // TODO: 상점 팝업 오픈
            return;
        }
        TryUseItem();
    }

    private void InitUnLock()
    {
        int stageLevel = GameScene.CurrentStage.Index;
        if (stageLevel < UnLockLevel)
        {
            _btnSlot.Sprite = Main.Resource.Get<Sprite>("Item_Slot_Lock");
            _imgIcon.Sprite = Main.Resource.Get<Sprite>("Icon_Lobby_Lock_Off");
            _btnSlot.SetActive(false, false);
            _imgIcon.SetSize(new Vector2(70, 70));
            _textUnLock.Text = $"Level {UnLockLevel}";
        }
        else
        {
            _btnSlot.Sprite = Main.Resource.Get<Sprite>("Item_Slot");
            _imgIcon.Sprite = GetIconSprite();
            _btnSlot.SetActive(true, false);
            _imgIcon.SetSize(new Vector2(100, 100));
        }
        
        _textUnLock.gameObject.SetActive(stageLevel < UnLockLevel);
    }

    private void SetItemCountText(int count)
    {
        int stageLevel = GameScene.CurrentStage.Index;
        Initialize();
        _textItemCount.Text = count.ToString();
        _buyBG.gameObject.SetActive(count <= 0 && stageLevel >= UnLockLevel);
        _imgCountBG.gameObject.SetActive(count > 0 && stageLevel >= UnLockLevel);
    }

    // NewItem Popup이 출력 가능한지 확인하고 가능하다면 시작하는 화면에 띄우는 함수
    private void SetNewItemPopup()
    {
        if (GameScene.CurrentStage.Index != GetUnLockLevel()) return;
        if (Main.UI.UsePopup<UI_Popup_NewItem>()) return;
        Main.UI.OpenPopup<UI_Popup_NewItem>().Set(GetItemName(), GetItemDesc(), GetIconSprite());
    }
}