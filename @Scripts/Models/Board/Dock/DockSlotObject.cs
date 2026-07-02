using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DockSlotObject : Entity {

    #region Properties

    public DockSlot Slot { get; private set; }

    #endregion

    #region Initialize / Set

    public void Set(DockSlot slot) {
        Initialize();
        Slot = slot;

        this.transform.name = $"Slot[{Slot.Index}]";
        this.transform.SetParent(slot.Dock.UI_Scene.DockDockSlots, false);
        this.transform.localRotation = Quaternion.identity;
        Vector3 startPos = Slot.Dock.Object.SlotSpawnPos.position;
        transform.position = startPos;
        StartCoroutine(SetPosition());
    }

    private IEnumerator SetPosition()
    {
        yield return null;
        transform.position = Slot.Dock.GetPositionDockSlot(Slot);
        transform.localScale = Main.Screen.Camera.ApplyScale();
    }

    public void ResetPosition()
    {
        Vector3 nextPos = Slot.Dock.GetPositionDockSlot(Slot);
        transform.DOLocalMove(nextPos, 0.5f);
    }

    #endregion
    
}