using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Board;
using UnityEngine;
using UnityEngine.UIElements;

public class Box
{
    #region properties

    public BoxQueue Queue { get; }

    public ColorType Color { get; }

    public BoxState State { get; set; }

    public Orientation Orientation { get; private set; }

    public bool IsHideColor { get; private set; }

    public BoxObject Object { get; private set; }

    public IReadOnlyList<BoxSlot> Slots => _slots;
    public IReadOnlyList<IBoxGimmick> Gimmicks => _gimmicks;

    #endregion

    #region Fields

    private readonly List<BoxSlot> _slots = new();
    private readonly List<IBoxGimmick> _gimmicks = new();
    private CompleteBoxAnim _completeBoxAnim;
    private SmokeEffect _smokeEffect;

    #endregion

    #region Constructor

    public Box(BoxQueue queue, BoxData data)
    {
        _gimmicks.Clear();
        Queue = queue;
        Color = data.Color;
        Orientation = Queue.Direction == Direction.Top || Queue.Direction == Direction.Bottom
            ? Orientation.Vertical
            : Orientation.Horizontal;

        _completeBoxAnim = new CompleteBoxAnim(this);
        _smokeEffect = new SmokeEffect(this);

        SetGimmick(data.Gimmicks);

        for (int i = 0; i < 6; i++)
        {
            _slots.Add(new(this, i));
        }
    }

    public void PlayEffect() => _smokeEffect?.PlayEffect();
    public void StartBoxCompleteAnim() => _completeBoxAnim.StartAnimation();

    private void SetGimmick(List<BoxGimmickData> gimmicks)
    {
        if (gimmicks == null) return;
        foreach (BoxGimmickData gimmick in gimmicks)
        {
            switch (gimmick.Type)
            {
                case BoxGimmickType.Hidden:
                    _gimmicks.Add(new BoxHidden(this));
                    break;
                case BoxGimmickType.Connector:
                    _gimmicks.Add(new BoxGroup(this, gimmick.Count));
                    break;
            }
        }
    }

    public void Destroy(Action onCompleteAction = null)
    {
        Main.Board.CheckClear();
        onCompleteAction += () =>
        {
            Queue.TrySetBoxNull(this);
            Queue.Pull();
            Queue.Spawn();
            _smokeEffect.Destroy();
            _completeBoxAnim.Destroy();
        };
        Object?.Destroy(onCompleteAction);
    }

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<BoxObject>();
        Object.Set(this);
        foreach (BoxSlot slot in _slots) slot.GenerateObject();
        _completeBoxAnim.GenerateObject();
        _smokeEffect.GenerateObject();
    }

    public BoxData GetData()
    {
        return new BoxData
        {
            Color = Color,
        };
    }

    public MaskData GetMaskData()
    {
        SpriteMask mask = Object.SpriteMaskBack;
        SpriteRenderer renderer = Object.BackSpriter;
        if (mask == null || renderer == null)
        {
            Debug.LogError($"data is null");
            return null;
        }

        MaskData maskData = new MaskData(mask, renderer);
        return maskData;
    }

    //원래 컬러 색을 보여주려고 시도하는 메서드
    public void TryShowColor()
    {
        if (!IsHideColor) return;
    }

    #endregion

    // 대기중인 Slot의 localPosition 계산.
    public Vector2 GetLocalPosition(BoxSlot slot)
    {
        if (!_slots.Contains(slot)) return Vector2.zero;
        int index = _slots.IndexOf(slot);
        return Queue.Object.GetCurrentSlotLocalPosition(index);
    }

    public Vector2 GetSlotPosition(BoxSlot slot)
    {
        if (!_slots.Contains(slot)) return Vector2.zero;
        int index = _slots.IndexOf(slot);
        return Queue.Object.GetCurrentSlotPosition(index);
    }

    #region InQueue

    public void Spawn()
    {
        GenerateObject();
        State = BoxState.Spawning;
        Object.Spawn();
    }

    public void Pull()
    {
        State = BoxState.Pulling;
        Object.Pull();
    }

    public bool HasGimmick(BoxGimmickType type)
    {
        return _gimmicks.Any(x => x.Type == type);
    }

    public bool TryGetGimmick<T>(out T gimmick) where T : class, IBoxGimmick
    {
        gimmick = GetGimmick<T>();
        return gimmick != null;
    }

    public T GetGimmick<T>() where T : class, IBoxGimmick
    {
        return _gimmicks.OfType<T>().FirstOrDefault();
    }

    public void RemoveGimmick(IBoxGimmick gimmick)
    {
        if (_gimmicks.Remove(gimmick)) gimmick.OnRemoved();
    }

    #endregion

    #region Knit

    // Knit를 이 Box로 이동시킴. 색상이 일치하지 않거나 슬롯이 없다면 실패.
    public bool EnterBox(Knit knit)
    {
        if (this.Color != knit.Color) return false;
        if (State == BoxState.Complete) return false;

        BoxSlot emptySlot = _slots.FirstOrDefault(s => s.IsEmpty);
        if (emptySlot == null) return false;
        knit.MoveToBox(emptySlot);
        return true;
    }

    public bool EnterBoxItem(Knit knit)
    {
        if (this.Color != knit.Color) return false;

        BoxSlot emptySlot = _slots.FirstOrDefault(s => s.IsEmpty);
        if (emptySlot == null) return false;

        if (knit != null)
        {
            knit.Destroy();
        }

        knit.MoveToBoxData(emptySlot);
        knit.SpawnSlot();
        return true;
    }

    //박스 슬롯에 들어있는 모든 Knit을 파괴
    public void DestroyAllKnits()
    {
        foreach (BoxSlot slot in _slots)
        {
            slot.Knit?.Destroy();
        }
    }

    public void OnCompletedMove()
    {
        if (State == BoxState.None) return;
        if (State == BoxState.Complete) return;
        if (State == BoxState.CompleteWaiting) return;
        if (Queue.NextBox == this) return;
        if (!CheckCompleteBox()) return; // 상자가 가득차야 함.

        if (Object != null && Queue.CurrentBox == this)
        {
            if (TryGetGimmick(out BoxGroup group))
            {
                State = BoxState.CompleteWaiting;
                group.TryCompleteAction(group.Count);
            }
            else
            {
                OnCompletedAction();
            }
        }
        else
        {
            Queue.RemoveBox(this);
            State = BoxState.None;
        }
    }

    public void OnCompletedAction()
    {
        State = BoxState.Complete;
        GameEvents.OnCompleteBox?.Invoke();
        StartBoxCompleteAnim();
    }

    //모든 Knit가 들어있는지 확인
    public bool CheckCompleteBox()
    {
        if (_slots.Any(s => s.IsEmpty)) return false;
        if (_slots.Any(s => s.Knit.State != KnitState.OnBox)) return false;
        return true;
    }

    #endregion
}

public enum BoxState
{
    None = 0,
    Idle = 1 << 0,          // 기본 상태.
    Spawning = 1 << 1,      // 스폰 상태.
    Pulling = 1 << 2,       // 밀리고 있는 상태.
    Complete = 1 << 3,      // 완성된 상태.
    SpawnWaiting = 1 << 4,      //스폰 대기 중인 상태.
    CompleteWaiting = 1 << 5,   //완성 대기 중인 상태.
    PullingWaiting = 1 << 6,    //밀리기 대기 중인 상태.
}