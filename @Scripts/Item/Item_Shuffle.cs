using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item_Shuffle : ItemBase
{
    protected override Data<int> GetItemData() => PlayerData.ItemShuffleCount;
    protected override Sprite GetIconSprite() => Main.Resource.Get<Sprite>("Icon_ItemShuffle");
    protected override string GetItemName() => $"Shuffle";
    protected override string GetItemDesc() => $"Shuffle cards in the queue";
    protected override int GetPrice() => 500;
    protected override int GetUnLockLevel() => 11;
    protected override bool CanUseItem()
    {
        // KnitArray 생성
        Main.Board.Bundle.SetKnitArray();
        
        List<KnitArray> knitArrays = Main.Board.Bundle.GroupAllBundlesKnits();

        if (knitArrays.Count < 2) return false; // TODO: 나중에 무조건 실패하는 쪽으로 바꿔야 함 

        string originalHash = GetKnitArrayStructureHash(knitArrays);
        string shuffledHash = originalHash;
    
        int currentAttempt = 0;
        const int maxShuffleAttempts = 10; // 최대 재시도 횟수 설정

        do
        {
            currentAttempt++;
        
            if (currentAttempt > maxShuffleAttempts)
            {
                Debug.LogError($"Shuffle failed after {maxShuffleAttempts} attempts. Final structure matches original or could not be changed.");
                break;
            }

            ForceShuffleAllOnce(knitArrays); 

            shuffledHash = GetKnitArrayStructureHash(knitArrays);
        } while (originalHash == shuffledHash);
    
        // 성공 여부 확인
        return originalHash != shuffledHash;
    }

    protected override void TryUseItem()
    {
        // KnitObject 파괴
        Main.Board.Bundle.DestroyAllKnitObjects();
        // Bundle DataList 삭제
        Main.Board.Bundle.ResetKnitList();
        // Data List 재생성
        Main.Board.Bundle.SetInKnitData();
        // KnitObject 재생성
        Main.Board.Bundle.SpawnAllKnitObjects();
    }
    
    
    // List<KnitArray>의 현재 순서와 내용을 기반으로 해시 문자열을 반환
    private string GetKnitArrayStructureHash(List<KnitArray> knitArrays)
    {
        var hashComponents = knitArrays
            .Select(array => $"{array.GetColorType()}:{array.GetCount()}");

        return string.Join("|", hashComponents);
    }
    
    
    private void ForceShuffleAllOnce(List<KnitArray> list)
    {
        if (list.Count < 2) return;
    
        Dictionary<KnitArray, int> swapSuccessCount = list.ToDictionary(k => k, v => 0);
    
        System.Random rng = new System.Random();
        int successfulSwapCount = 0; 
        int totalAttempts = 0;
    
        // 무한 루프 방지용 최대 시도 횟수
        int maxAttempts = list.Count * 1000; 

        while (swapSuccessCount.Any(x => x.Value == 0))
        {
            totalAttempts++;
        
            if (totalAttempts > maxAttempts) 
            {
                Debug.LogWarning($"Shuffle failed to fully complete. Reached max attempts: {totalAttempts}");
                break;
            }

            // 1. 무작위로 두 인덱스 선택
            int indexA = rng.Next(list.Count);
            int indexB = rng.Next(list.Count);
        
            // 같은 인덱스인 경우 건너뛰기
            if (indexA == indexB) continue; 

            KnitArray arrayA = list[indexA];
            KnitArray arrayB = list[indexB];
        
            // 2. 스왑 시도 및 성공 여부 확인
            bool swapped = arrayA.TrySwapKnitArray(arrayB);
        
            if (swapped)
            {
                successfulSwapCount++;
                swapSuccessCount[arrayA]++;
                swapSuccessCount[arrayB]++;
            }
        }
    }
}
