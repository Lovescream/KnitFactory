using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AddSpaceSlotObject : Entity {

    #region Properties

    public AddSpaceSlot Slot { get; private set; }
    private Sequence _sequence;

    #endregion

    #region Initialize / Set

    public void Set(AddSpaceSlot slot) {
        Initialize();
        Slot = slot;

        this.transform.name = $"Slot[{Slot.Index}]";
        this.transform.SetParent(slot.Dock.UI_Scene.AddSpaceSlots, false);
        this.transform.localRotation = Quaternion.identity;
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
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(nextPos, 0.5f));
    }

    #endregion
    
}