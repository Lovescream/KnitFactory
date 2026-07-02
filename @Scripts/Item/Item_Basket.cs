using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item_Basket : ItemBase
{
    protected override Data<int> GetItemData() => PlayerData.ItemBasketCount;
    protected override Sprite GetIconSprite() => Main.Resource.Get<Sprite>("Icon_ItemBasket");
    protected override string GetItemName() => $"Annyeong";
    protected override string GetItemDesc() => $"Hasaeyo?";
    protected override int GetPrice() => 700;
    protected override int GetUnLockLevel() => 25;
    protected override bool CanUseItem() => true;

    protected override void TryUseItem()
    {
        List<List<Knit>> knits = Main.Board.Bundle.GetLastKnitGroup();
        List<List<Box>> boxes = new();
        foreach (BoxQueue queue in Main.Board.BeltBoard.BoxesQueue)
        {
            boxes.Add(queue.GetBoxList());
        }
        
        //List의 인덱스가 빠른 순서대로 박스들을 정렬
        List<Box> arrayBoxes = boxes
            .SelectMany(boxList => boxList.Select((box, index) => new { Box = box, InnerIndex = index }))
            .OrderBy(item => item.InnerIndex) 
            .Select(item => item.Box)
            .ToList();

        foreach (List<Knit> knitList in knits)
        {
            int boxIndex = 0;
            foreach (Knit knit in knitList)
            {
                while (boxIndex < arrayBoxes.Count)
                {
                    Box box = arrayBoxes[boxIndex];
                    if (box == null)
                    {
                        boxIndex++;
                        break;
                    }
                    if (box.EnterBoxItem(knit)) break;
                    boxIndex++;
                }
            }
        }
        
        foreach (Bundle bundle in Main.Board.Bundle.Bundles)
        {
            bundle.PullAllKnits();
        }
    }
}
