using UnityEngine;

public class BoxHidden : BoxGimmickBase
{
    public override BoxGimmickType Type => BoxGimmickType.Hidden;
    public BoxHiddenObject Object => (BoxHiddenObject)GimmickObject;
    public BoxHidden(Box box) : base(box)
    {
    }

    public override BoxGimmickData GetData()
    {
        return new BoxGimmickData()
        {
            Type = Type,
            Count = 0,
        };
    }

    public override void GenerateNewObject()
    {
        GimmickObject = Main.Object.Instantiate<BoxHiddenObject>();
        Object.Set(this);
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
        Object.Destroy();
    }
}
