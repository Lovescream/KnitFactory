using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnitGroup : KnitGimmickBase
{
    public override KnitGimmickType Type => KnitGimmickType.Rope;
    public KnitGroupObject Object => (KnitGroupObject)GimmickObject;
    public StudRope LeftRope { get; set; }
    public StudRope RightRope { get; set; }
    public event Action<KnitGroupObject> OnConnectRope;


    public int Count
    {
        get => _count;
        set => _count = value;
    }

    private int _count;

    public KnitGroup(Knit knit, int count) : base(knit)
    {
        _count = count;
        Knit.Bundle.Board.AddGroupGimmick(Count, Knit);
        Knit.Bundle.AddGroupKnit(Count, Knit);
    }

    public override KnitGimmickData GetData()
    {
        return new KnitGimmickData()
        {
            Type = Type,
            Count = Count,
        };
    }

    public override void GenerateNewObject()
    {
        GimmickObject = Main.Object.Instantiate<KnitGroupObject>();
        Object.Set(this);
        OnConnectRope?.Invoke(Object);
        SetRope(true);
        SetRope(false);
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        Object.Destroy();
        LeftRope?.Object?.Destroy();
        RightRope?.Object?.Destroy();
        LeftRope = null;
        RightRope = null;
    }

    // 로프를 생성하는 함수
    private void SetRope(bool isLeft)
    {
        StudRope rope = isLeft ? LeftRope : RightRope;
        if (rope != null)
        {
            rope.SetEnd(Object?.transform);
        }
        else
        {
            Knit knit = isLeft 
                ? SearchGroup(true, Knit.Bundle, Count) 
                : SearchGroup(false, Knit.Bundle, Count);
            if (knit == null) return;
            rope = new StudRope(this);
            rope.GenerateObject();
            rope.SetStart(Object?.transform);
            rope.SetEnd(knit.Bundle.Object.GetQueueEnter());
            
            if (knit.TryGet(out KnitGroup group))
            {
                if (isLeft) group.RightRope = rope;
                else group.LeftRope = rope;
            }
        }
    }

    // 로프의 목적지를 찾는 함수
    private Knit SearchGroup(bool isLeft, Bundle bundle, int groupCount)
    {
        Bundle nextBundle = isLeft ? bundle.Board.LeftBundle(bundle) : bundle.Board.RightBundle(bundle);
        if (nextBundle == null) return null;
        if (nextBundle.KnitsGroup.TryGetValue(groupCount, out var result)) return result;
        return SearchGroup(isLeft, nextBundle, groupCount);
    }

    // 로프 제거를 시도하는 함수
    public void TryRemoveRope(int groupCount)
    {
        bool canRemove = Knit.Bundle.Board.KnitsGroupGimmick[groupCount]
            .All(x => x.Bundle.IsFirstKnit(x));
        if (!canRemove) return;
        foreach (Knit knit in Knit.Bundle.Board.KnitsGroupGimmick[groupCount])
        {
            knit.State = KnitState.Waiting;
            if(knit.TryGet(out KnitGroup group)) knit.RemoveGimmick(group);
        }
    }
}