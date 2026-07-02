using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dock
{
    #region Const.

    private const float OddAddXRatio = -0.4f;
    public const int AddSlotCount = 4;

    #endregion

    #region Properties

    public UI_GameScene UI_Scene { get; }
    public UI_DockObject Object { get; private set; }
    private float DockSlotGapY => Object.EndX.position.y - Object.StartX.position.y;

    #endregion

    #region Fields

    private readonly List<DockSlot> _slots = new();
    private readonly List<AddSpaceSlot> _slotsAddSpace = new();

    #endregion

    #region Constructor

    public Dock(UI_GameScene uiScene, StageDataKey stageDataKey)
    {
        UI_Scene = uiScene;
        for (int i = 0; i < stageDataKey.MaxKnitsOnDock; i++)
        {
            _slots.Add(new(this, i));
            _slotsAddSpace.Add(new(this, i));
        }
    }

    public void SetAddSlotText() => Object?.SetAddSlotText();
    public void SetAddSpaceText() => Object?.SetAddSpaceText();

    // TODO:: 추후에 비슷한 Dock, AddSpace 로직을 하나로 합칠 필요가 있음.
    public void AddDockSlot(int count)
    {
        int curCount = _slots.Count;
        for (int i = curCount; i < curCount + count; i++)
        {
            DockSlot slot = new(this, i);
            _slots.Add(slot);
            slot.GenerateObject();
        }

        foreach (DockSlot slot in _slots)
        {
            slot.Object.ResetPosition();
        }
    }

    public AddSpaceSlot AddSlotSpace()
    {
        int curCount = _slotsAddSpace.Count;
        AddSpaceSlot newSlot = new(this, curCount);
        _slotsAddSpace.Add(newSlot);
        newSlot.GenerateObject();
        foreach (AddSpaceSlot slot in _slotsAddSpace)
        {
            slot.Object.ResetPosition();
        }

        return newSlot;
    }

    public void GenerateObject(Transform parent)
    {
        Object = Main.Object.Instantiate<UI_DockObject>();
        Object.Set(this, parent);
    }

    public void GenerateObjectSlot()
    {
        foreach (DockSlot slot in _slots) slot.GenerateObject();
        foreach (AddSpaceSlot slot in _slotsAddSpace) slot.GenerateObject();
    }

    #endregion

    public void SetActiveBtn(bool active) => Object?.SetActiveBtn(active);

    // 대기중인 Slot의 localPosition 계산.
    public Vector3 GetPositionDockSlot(DockSlot slot)
    {
        if (!_slots.Contains(slot)) return Vector3.zero;
        int index = _slots.IndexOf(slot);
        return GetPositionDockSlot(index, Object.StartX.position.x, Object.EndX.position.x, _slots.Count);
    }

    public Vector3 GetPositionDockSlot(AddSpaceSlot slot)
    {
        if (!_slotsAddSpace.Contains(slot)) return Vector3.zero;
        int index = _slotsAddSpace.IndexOf(slot);
        return GetPositionAddSpaceSlot(index, Object.AddSpaceStartX.position.x, Object.AddSpaceEndX.position.x,
            _slotsAddSpace.Count);
    }

    private Vector3 GetPositionDockSlot(int index, float startX, float endX, int count)
    {
        if (count <= 0) return Vector3.zero;
        if (index < 0) return Vector3.zero;
        if (index >= count) index = count - 1;

        float t = index / (float)(count - 1);
        float x = Mathf.Lerp(startX, endX, t);

        float y = (index % 2 == 0) ? Object.StartX.position.y - DockSlotGapY : Object.StartX.position.y + DockSlotGapY;
        if (index % 2 == 1)
        {
            float prevX = Mathf.Lerp(startX, endX, (index - 1) / (float)(count - 1));
            x += (x - prevX) * OddAddXRatio;
        }

        return new Vector3(x, y, 0);
    }

    private Vector3 GetPositionAddSpaceSlot(int index, float startX, float endX, int count)
    {
        if (count <= 0) return Vector3.zero;
        if (index < 0) return Vector3.zero;
        if (index >= count) index = count - 1;

        float t = index / (float)(count - 1);
        float x = Mathf.Lerp(startX, endX, t);

        float y = (index % 2 == 0)
            ? Object.AddSpaceStartX.position.y - DockSlotGapY
            : Object.AddSpaceStartX.position.y + DockSlotGapY;
        if (index % 2 == 1)
        {
            float prevX = Mathf.Lerp(startX, endX, (index - 1) / (float)(count - 1));
            x += (x - prevX) * OddAddXRatio;
        }

        return new Vector3(x, y, 0);
    }

    #region Slots

    public void MoveToSsagae(DockSlot slot)
    {
        if (!TryGetSameColorDockKnits(slot, out List<Knit> knits)) return;
        foreach (Knit knit in knits) knit.MoveToSsagae();
        PullAllDockSlots();
    }

    public void MoveToSsagae(AddSpaceSlot slot)
    {
        if (!TryGetSameColorAddSpaceKnits(slot, out List<Knit> knits)) return;
        foreach (Knit knit in knits) knit.MoveToSsagae();
        PullAllAddSpaceSlots();
    }

    // 해당 컬러의 털실이 DockSlot에서 클릭 가능한지에 대한 여부를 반환
    private bool TryGetSameColorDockKnits(DockSlot slot, out List<Knit> knits)
    {
        ColorType searchColor = slot.Knit.Color;
        knits = _slots
            .Where(s => s.Knit != null && s.Knit.Color == searchColor)
            .Select(s => s.Knit)
            .ToList();

        if (Main.Board.CanKnitsOnBeltCount < knits.Count) return false;
        return true;
    }
    
    // 해당 컬러의 털실이 AddSpaceSlot에서 클릭 가능한지에 대한 여부를 반환
    private bool TryGetSameColorAddSpaceKnits(AddSpaceSlot slot, out List<Knit> knits)
    {
        ColorType searchColor = slot.Knit.Color;
        knits = _slotsAddSpace
            .Where(s => s.Knit != null && s.Knit.Color == searchColor)
            .Select(s => s.Knit)
            .ToList();

        if (Main.Board.CanKnitsOnBeltCount < knits.Count) return false;
        return true;
    }

    private void PullAllDockSlots()
    {
        _slots.ForEach(Pull);
    }

    private void PullAllAddSpaceSlots()
    {
        _slotsAddSpace.ForEach(Pull);
    }

    private void Pull(DockSlot slot)
    {
        if (slot.IsEmpty) return;

        // #1. 해당 슬롯이 최대로 당겨질 수 있는 위치 찾기.
        int index = _slots.IndexOf(slot);
        bool kanade = false;
        for (int i = index; i >= 0; i--)
        {
            if (!kanade && _slots[i].IsEmpty) kanade = true;
            else if (kanade)
            {
                if (!_slots[i].IsEmpty)
                {
                    index = i + 1;
                    break;
                }

                if (i == 0)
                {
                    index = 0;
                    break;
                }
            }
        }

        // #2. 해당 슬롯까지 이동.
        slot.Knit.MoveToDockSlot(_slots[index]);
    }

    private void Pull(AddSpaceSlot slot)
    {
        if (slot.IsEmpty) return;

        // #1. 해당 슬롯이 최대로 당겨질 수 있는 위치 찾기.
        int index = _slotsAddSpace.IndexOf(slot);
        bool kanade = false;
        for (int i = index; i >= 0; i--)
        {
            if (!kanade && _slotsAddSpace[i].IsEmpty) kanade = true;
            else if (kanade)
            {
                if (!_slotsAddSpace[i].IsEmpty)
                {
                    index = i + 1;
                    break;
                }

                if (i == 0)
                {
                    index = 0;
                    break;
                }
            }
        }

        // #2. 해당 슬롯까지 이동.
        slot.Knit.MoveToAddSpaceSlot(_slotsAddSpace[index]);
    }

    // 해당 Knit이 들어갈 dockSlot을 마련.  
    public DockSlot FreeUpDockSlot(Knit knit)
    {
        // #0. 빈 공간이 없으면 들어갈 Slot을 준비할 수 없음.
        if (_slots.All(s => !s.IsEmpty)) return null;
        if (!_slots[^1].IsEmpty) return null; // 마지막 슬롯이 비어야 함을 보장. (없어도 되지만 안전성을 위해)

        // #1. 처음 슬롯부터 차례대로 검사: 빈 슬롯이나 첫 같은 색상 슬롯을 찾는다.
        int onajiiro = 0;
        for (int i = 0; i < _slots.Count; i++)
        {
            DockSlot slot = _slots[i];
            if (slot.IsEmpty) return slot;
            if (slot.Knit.Color == knit.Color)
            {
                onajiiro = i;
                break;
            }
        }

        // #2. 같은 색상이 끝나는 슬롯을 찾는다.
        for (int i = onajiiro; i < _slots.Count; i++)
        {
            DockSlot slot = _slots[i];
            if (slot.IsEmpty || slot.Knit.Color != knit.Color)
            {
                onajiiro = i - 1;
                break;
            }
        }

        // #3. 그 뒤 슬롯을 모두 뒤로 차례로 민다.
        for (int i = _slots.Count - 1; i > onajiiro; i--)
        {
            DockSlot slot = _slots[i];
            if (slot.IsEmpty) continue;
            DockSlot nextSlot = _slots[i + 1];
            slot.Knit.MoveToDockSlot(nextSlot);
        }

        // #4. 빈 공간 반환.
        return _slots[onajiiro + 1];
    }

    // 해당 Knit이 들어갈 addSpaceSlot을 마련.  
    public AddSpaceSlot FreeUpAddSpaceSlot(Knit knit)
    {
        // #0. 빈 공간이 없으면 들어갈 Slot을 준비할 수 없음.
        if (_slotsAddSpace.All(s => !s.IsEmpty)) return null;
        if (!_slotsAddSpace[^1].IsEmpty) return null; // 마지막 슬롯이 비어야 함을 보장. (없어도 되지만 안전성을 위해)

        // #1. 처음 슬롯부터 차례대로 검사: 빈 슬롯이나 첫 같은 색상 슬롯을 찾는다.
        int onajiiro = 0;
        for (int i = 0; i < _slotsAddSpace.Count; i++)
        {
            AddSpaceSlot slot = _slotsAddSpace[i];
            if (slot.IsEmpty) return slot;
            if (slot.Knit.Color == knit.Color)
            {
                onajiiro = i;
                break;
            }
        }

        // #2. 같은 색상이 끝나는 슬롯을 찾는다.
        for (int i = onajiiro; i < _slotsAddSpace.Count; i++)
        {
            AddSpaceSlot slot = _slotsAddSpace[i];
            if (slot.IsEmpty || slot.Knit.Color != knit.Color)
            {
                onajiiro = i - 1;
                break;
            }
        }

        // #3. 그 뒤 슬롯을 모두 뒤로 차례로 민다.
        for (int i = _slotsAddSpace.Count - 1; i > onajiiro; i--)
        {
            AddSpaceSlot slot = _slotsAddSpace[i];
            if (slot.IsEmpty) continue;
            AddSpaceSlot nextSlot = _slotsAddSpace[i + 1];
            slot.Knit.MoveToAddSpaceSlot(nextSlot);
        }

        // #4. 빈 공간 반환.
        return _slotsAddSpace[onajiiro + 1];
    }

    // Dock Slot에 있는 마지막 Knit부터 count의 수만큼 반환하는 함수
    public List<Knit> GetLastDockSlots(int count)
    {
        List<Knit> result = _slots
            .Where(slot => !slot.IsEmpty)
            .TakeLast(count)
            .Select(slot => slot.Knit)
            .ToList();
        return result;
    }

    #endregion
}