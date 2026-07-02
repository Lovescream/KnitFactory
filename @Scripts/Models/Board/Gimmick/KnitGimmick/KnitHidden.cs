using UnityEngine;

public class KnitHidden : KnitGimmickBase
{
    public override KnitGimmickType Type => KnitGimmickType.Hidden;
    public KnitHiddenObject Object => (KnitHiddenObject)GimmickObject;
    public KnitHidden(Knit knit) : base(knit)
    {
    }

    public override KnitGimmickData GetData()
    {
        return new KnitGimmickData()
        {
            Type = Type,
            Count = 0,
        };
    }

    public override void GenerateNewObject()
    {
        GimmickObject = Main.Object.Instantiate<KnitHiddenObject>();
        Object?.Set(this);
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        Object?.Destroy();
    }
}
