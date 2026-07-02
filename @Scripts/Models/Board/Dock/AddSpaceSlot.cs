using UnityEngine;

public class AddSpaceSlot {

    #region Properties

    public Dock Dock { get; }
    public int Index { get; }

    public Knit Knit { get; set; }
    public bool IsEmpty => Knit == null;
    public Vector2 Position => Object?.transform.position ?? new(-62, -62);

    public AddSpaceSlotObject Object { get; private set; }

    #endregion

    #region Constructor

    public AddSpaceSlot(Dock dock, int index) {
        Dock = dock;
        Index = index;
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<AddSpaceSlotObject>();
        Object.Set(this);
    }

    #endregion
    
}