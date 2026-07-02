using UnityEngine;

public class Item_AddCapacity : ItemBase
{
    #region Const

    private const int AddSpaceCount = 5;

    #endregion
    protected override Data<int> GetItemData() => PlayerData.ItemSpaceCount;
    protected override Sprite GetIconSprite() => Main.Resource.Get<Sprite>("Icon_ItemSpace");
    protected override string GetItemName() => $"Add Capacity";
    protected override string GetItemDesc() => $"Add more space for cards";
    protected override int GetPrice() => 100;
    protected override int GetUnLockLevel() => 8;
    protected override bool CanUseItem() => true;
    protected override void TryUseItem() => Main.Board.AddKnitsCountOnBelt(AddSpaceCount);
}
