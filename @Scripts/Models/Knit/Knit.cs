using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knit
{
    #region Properties

    public ColorType Color { get; private set; }
    public KnitObject Object { get; private set; }
    public IReadOnlyList<IKnitGimmick> Gimmicks => _gimmicks;

    public Belt Belt
    {
        get => _belt;
        set
        {
            if (_belt == value) return;
            _belt = value;
            if (_belt != null)
            {
                State = KnitState.OnBelt;
            }
        }
    }

    public BeltLane Lane { get; private set; } = BeltLane.None;
    public Bundle Bundle { get; private set; }
    
    // TODO:: 개편시 DockSlot과 AddSpaceSlot과 BoxSlot을 하나로 묶던지 해야할 듯

    public DockSlot DockSlot
    {
        get => _dockSlot;
        set
        {
            if (_dockSlot == value) return;
            if (_dockSlot != null && _dockSlot.Knit == this)
            {
                _dockSlot.Knit = null;
                _dockSlot = null;
            }

            _dockSlot = value;
            if (_dockSlot != null)
            {
                _dockSlot.Knit = this;
            }
        }
    }

    public AddSpaceSlot AddSpaceSlot
    {
        get => _addSpaceSlot;
        set
        {
            if (_addSpaceSlot == value) return;
            if (_addSpaceSlot != null && _addSpaceSlot.Knit == this)
            {
                _addSpaceSlot.Knit = null;
                _addSpaceSlot = null;
            }

            _addSpaceSlot = value;
            if (_addSpaceSlot != null)
            {
                _addSpaceSlot.Knit = this;
            }
        }
    }

    public BoxSlot BoxSlot
    {
        get => _boxSlot;
        set
        {
            if (_boxSlot == value) return;
            if (_boxSlot != null && _boxSlot.Knit == this)
            {
                _boxSlot.Knit = null;
                _boxSlot = null;
            }

            _boxSlot = value;
            if (_boxSlot != null)
            {
                _boxSlot.Knit = this;
            }
        }
    }

    public KnitState State
    {
        get => _state;
        set
        {
            if (_state == value) return;
            
            if (_state == KnitState.OnBelt || value == KnitState.MoveToAddSpaceSlot)
            {
                Main.Board.RemoveKnitsOnBelt(this);
                Object.SetAnimation();
            }

            _state = value;
            if (_state == KnitState.MoveToSsagae) Main.Board.AddKnitsOnBelt(this);
            OnChangedState?.Invoke(value);
        }
    }

    public MoveMachine MoveMachine { get; private set; }

    #endregion

    #region Fields

    private Belt _belt;
    private DockSlot _dockSlot;
    private AddSpaceSlot _addSpaceSlot;
    private BoxSlot _boxSlot;
    private KnitState _state;
    private readonly List<IKnitGimmick> _gimmicks = new();
    
    private event Action<KnitState> OnChangedState;

    #endregion

    #region Constructor

    public Knit(Bundle bundle, KnitData data)
    {
        Bundle = bundle;
        Color = data.Color;
        SetGimmick(data.Gimmicks);
    }

    #endregion

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<KnitObject>();
        Object.Set(this);
    }

    public void Destroy()
    {
        Object?.Destroy();
        Object = null;
    }

    public void SetBundle(Bundle bundle) => Bundle = bundle;

    #region Gimmick
    
    private void SetGimmick(List<KnitGimmickData> gimmicks)
    {
        if (_gimmicks == null) return;
        foreach (KnitGimmickData gimmick in gimmicks)
        {
            switch (gimmick.Type)
            {
                case KnitGimmickType.Hidden:
                    _gimmicks.Add(new KnitHidden(this));
                    break;
                case KnitGimmickType.Rope:
                    _gimmicks.Add(new KnitGroup(this, gimmick.Count));
                    break;
            }
        }
    }

    public void RemoveGimmick(IKnitGimmick gimmick)
    {
        if (_gimmicks.Remove(gimmick)) gimmick.OnRemoved();
    }

    public T GetGimmick<T>() where T : class, IKnitGimmick
    {
        return _gimmicks.OfType<T>().FirstOrDefault();
    }

    public bool TryGet<T>(out T gimmick) where T : class, IKnitGimmick
    {
        gimmick = GetGimmick<T>();
        return gimmick != null;
    }

    public bool Has(KnitGimmickType type)
    {
        return _gimmicks.Any(g => g.Type == type);
    }

    #endregion

    #region InBundle

    public void Spawn()
    {
        GenerateObject();
        Object.Spawn();
    }

    public void SpawnSlot()
    {
        GenerateObject();
        Object.SpawnSlot();
    }

    public void Pull()
    {
        Object.Pull();
    }

    public void RemoveKnit()
    {
        Bundle.RemoveKnit(this);
    }

    #endregion

    #region Move

    public void MoveToSsagae()
    {
        ResetSlot();
        State = KnitState.MoveToSsagae;
        Object.MoveToSsagae();
    }

    public void MoveToBelt(BeltLane lane, Belt belt = null)
    {
        belt ??= Main.Board.BeltBoard.FirstBelt;
        State = KnitState.MoveToBelt;
        Object.MoveToBelt(belt, lane);
        Lane = lane;
    }

    public void MoveToDock()
    {
        DockSlot slot = Main.Board.Dock.FreeUpDockSlot(this);
        // 더 이상 이동할 슬롯이 없다면 게임 끝 처리.
        if (slot == null)
        {
            GameScene.GameState = GameState.Failed;
            return;
        }

        MoveToDockSlot(slot);
    }

    public void MoveToDockSlot(DockSlot slot)
    {
        DockSlot = slot;
        State = KnitState.MoveToDockSlot;
        ResetSlot(KnitSlot.DockSlot);
        Object.MoveToDockSlot();
    }

    public void MoveToAddSpace()
    {
        AddSpaceSlot slot = Main.Board.Dock.FreeUpAddSpaceSlot(this);

        if (slot == null)
        {
            slot = Main.Board.Dock.AddSlotSpace();
        }
        
        MoveToAddSpaceSlot(slot);
    }

    public void MoveToAddSpaceSlot(AddSpaceSlot slot)
    {
        AddSpaceSlot = slot;
        DockSlot = null;
        BoxSlot = null;
        State = KnitState.MoveToAddSpaceSlot;
        ResetSlot(KnitSlot.AddSpaceSlot);
        Object.MoveToAddSpaceSlot();
    }

    public void MoveToBox(BoxSlot slot)
    {
        BoxSlot = slot;
        State = KnitState.MoveToBox;
        ResetSlot(KnitSlot.BoxSlot);
        Object.MoveToBox();
    }

    public void MoveToBoxData(BoxSlot slot)
    {
        BoxSlot = slot;
        State = KnitState.OnBox;
    }

    public void MoveToMoveMachine(MoveMachine moveMachine)
    {
        MoveMachine = moveMachine;
        State = KnitState.OnMoveMachine;
        Object.MoveToMoveMachine();
    }

    #endregion

    #region Event

    private void ResetSlot(KnitSlot curSlot = KnitSlot.None)
    {
        if(curSlot != KnitSlot.BoxSlot) BoxSlot = null;
        if(curSlot != KnitSlot.DockSlot) DockSlot = null;
        if(curSlot != KnitSlot.AddSpaceSlot) AddSpaceSlot = null;
    }

    #endregion

    private enum KnitSlot
    {
        None = -1,
        BoxSlot,
        DockSlot,
        AddSpaceSlot,
    }
}

public enum KnitState
{
    None = 0,
    InBundle = 1 << 0,      // 번들 안에 존재하여 오브젝트가 없는 상태.
    Waiting = 1 << 1,       // 번들에 존재하며 오브젝트가 생성되어 대기중인 상태.
    MoveToSsagae = 1 << 2,  // 투입구로 이동하는 상태.
    InSsagae = 1 << 3,      // 투입구 내부에 있는 상태.
    MoveToBelt = 1 << 4,    // 벨트로 이동하고 있는 상태.
    OnBelt = 1 << 5,        // 벨트 위에 있는 상태.
    MoveToDockSlot = 1 << 6,    // Dock 슬롯으로 이동하는 상태.
    OnDockSlot = 1 << 7,        // Dock 슬롯에 있는 상태.
    MoveToAddSpaceSlot = 1 << 8,    // AddSpace 슬롯으로 이동하는 상태.
    OnAddSpaceSlot = 1 << 9,        // AddSpace 슬롯에 있는 상태.
    MoveToBox = 1 << 10,        // 박스로 이동하고 있는 상태.
    OnBox = 1 << 11,            // 박스 안에 있는 상태.
    OnMoveMachine = 1 << 12,    // 무빙머신에 있는 상태.
}