using UnityEngine;

public class BoxSlot {
    
    #region Properties

    public Box Box { get; }
    public int Index { get; }

    public Knit Knit { get; set; }
    public bool IsEmpty => Knit == null;
    public Vector2 Position => Object?.transform.position ?? new(-62, -62);

    public BoxSlotObject Object { get; private set; }

    #endregion

    #region Constructor

    public BoxSlot(Box box, int index) {
        Box = box;
        Index = index;
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<BoxSlotObject>();
        Object.Set(this);
    }

    #endregion

    public Vector2 GetSlotPosition() {
        return Box.GetSlotPosition(this);
    }
}