using UnityEngine;

public class DockSlot {

    #region Properties

    public Dock Dock { get; }
    public int Index { get; }

    public Knit Knit { get; set; }
    public bool IsEmpty => Knit == null;
    public Vector2 Position => Object?.transform.position ?? new(-62, -62);

    public DockSlotObject Object { get; private set; }

    #endregion

    #region Constructor

    public DockSlot(Dock dock, int index) {
        Dock = dock;
        Index = index;
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<DockSlotObject>();
        Object.Set(this);
    }

    #endregion
    
}