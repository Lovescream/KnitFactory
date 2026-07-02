using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item_Hammer : ItemBase
{
    protected override Data<int> GetItemData() => PlayerData.ItemHammerCount;
    protected override Sprite GetIconSprite() => Main.Resource.Get<Sprite>("Icon_ItemHammer");
    protected override string GetItemName() => $"Meow Punch";
    protected override string GetItemDesc() => $"Pick a box to break it";
    protected override int GetPrice() => 400;
    protected override int GetUnLockLevel() => 15;

    public override bool Initialize()
    {
        if(!base.Initialize()) return false;

        GameEvents.OnHammerItem += RunItem3Destroy;
        
        return true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEvents.OnHammerItem -= RunItem3Destroy;
    }

    protected override bool CanUseItem()
    {
        return ItemData.Value > 0 || PlayerData.Gold.Value >= Price;
    }

    protected override void OnClickUseItem()
    {
        Initialize();
        if (!CanUseItem()) return;
        TryUseItem();
    }

    protected override void TryUseItem()
    {
        GameScene.InGameState = InGameState.Hammer;
        Main.Board.Current.BeltBoard.SetMaskCurrentBox();
        Main.Screen.SetActiveRayCast2D(true);
    }

    private void RunItem3Destroy(Box box)
    {
        Main.Screen.SetActiveRayCast2D(false);
        if (ItemData.Value > 0) ItemData.Value--;
        else if (PlayerData.Gold.Value >= Price) PlayerData.Gold.Value -= Price;
        
        ColorType color = box.Color;
        int count = 0;
        foreach (BoxSlot slot in box.Slots)
        {
            if (slot.IsEmpty) count++;
        }
        
        List<Dictionary<int, Knit>> bundleGroups = new();
        IReadOnlyList<Bundle> bundles = Main.Board.Bundle.Bundles;
        
        foreach (Bundle bundle in Main.Board.Bundle.Bundles)
        {
            bundleGroups.Add(bundle.GetKnitForColorIndex(color));
        }
        
        if (bundleGroups.Count == 0) return;
        
        List<Knit> list = bundleGroups
            .SelectMany(dict => dict)      
            .GroupBy(kv => kv.Key)         
            .OrderByDescending(g => g.Key) 
            .SelectMany(g => g)           
            .Select(kv => kv.Value)        
            .ToList();

        if (list.Count < count)
        {
            Debug.LogError($"Bundle returned {count}.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            list[i].RemoveKnit();
        }

        foreach (Bundle bundle in bundles)
        {
            bundle.PullAllKnits();
        }
        
        box.Destroy(() =>
        {
            Main.Board.Current.SpriteDim.Object.CloseObjectDim();
            GameScene.InGameState = InGameState.Processing;
        });
    }
}
