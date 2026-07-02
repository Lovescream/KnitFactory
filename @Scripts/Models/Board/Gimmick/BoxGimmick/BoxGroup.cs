using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxGroup : BoxGimmickBase
{
    public override BoxGimmickType Type => BoxGimmickType.Connector;
    public BoxGroupObject Object => (BoxGroupObject)GimmickObject;
    private IReadOnlyDictionary<int, List<Box>> BoxGroupGimmick => Box.Queue.Board.BoxGroupGimmick;

    public int Count
    {
        get => _count;
        set => _count = value;
    }
    
    private int _count;
    
    public BoxGroup(Box box, int count) : base(box)
    {
        _count = count;
        Box.Queue.Board.AddGroupGimmick(Count, Box);
    }

    public override BoxGimmickData GetData()
    {
        return new BoxGimmickData
        {
            Type = Type,
            Count = Count,
        };
    }

    public override void GenerateNewObject()
    {
        GimmickObject = Main.Object.Instantiate<BoxGroupObject>();
        Object.Set(this);
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        Object.Destroy();
    }

    // 그룹의 박스들에게 완성 액션을 시도하는 메서드
    public void TryCompleteAction(int groupCount)
    {
        bool canComplete = BoxGroupGimmick[groupCount]
            .All(x => x.State == BoxState.CompleteWaiting);
        if (!canComplete) return;
        foreach (Box box in BoxGroupGimmick[groupCount])
        {
            box.OnCompletedAction();
            if(box.TryGetGimmick(out BoxGroup group)) box.RemoveGimmick(group);
        }
    }
    
    // 그룹의 박스들의 소환을 시도하는 메서드
    public void TrySpawnGroupGimmick(int groupCount)
    {
        bool canSpawn = BoxGroupGimmick[groupCount]
            .All(x => x.State == BoxState.SpawnWaiting);
        if (!canSpawn) return;
        foreach (Box box in BoxGroupGimmick[groupCount])
        {
            box.Spawn();
        }
    }

    // 그룹들에게 Pull을 시도하는 메서드
    public void TryPullGroupGimmick(int groupCount)
    {
        bool canPull = BoxGroupGimmick[groupCount]
            .All(x => x.Queue.CurrentBox == null);
        if (!canPull) return;
        foreach (Box box in BoxGroupGimmick[groupCount])
        {
            box.Queue.ForcePull();
            box.Queue.Spawn();
        }
    }
}
