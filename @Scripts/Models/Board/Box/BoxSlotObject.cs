using UnityEngine;

public class BoxSlotObject : Entity {

    #region Properties

    public BoxSlot Slot { get; private set; }

    #endregion

    #region Initialize / Set

    public void Set(BoxSlot slot) {
        Initialize();
        Slot = slot;

        this.transform.name = $"Slot[{Slot.Index}]";
        this.transform.SetParent(Slot.Box.Object.transform);
        this.transform.localPosition = Slot.Box.GetLocalPosition(Slot);
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        if (slot.Knit != null) slot.Knit.SpawnSlot();
    }

    public override void OnRelease() {
        base.OnRelease();
    }

    #endregion

    public void Destroy() {
        Main.Object.Destroy(this);
    }
    
}