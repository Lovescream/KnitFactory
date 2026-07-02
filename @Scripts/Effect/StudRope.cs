using UnityEngine;

public class StudRope
{
    public StudRopeObject Object { get; private set; }
    public KnitGroup Group { get; private set; }

    public StudRope(KnitGroup group)
    {
        Group = group;
    }

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<StudRopeObject>();
        Object.Set(this);
    }

    public void Destroy() => Object?.Destroy();
    public void SetStart(Transform tr) => Object?.SetStartTransform(tr);
    public void SetEnd(Transform tr) => Object?.SetEndTransform(tr);
}
