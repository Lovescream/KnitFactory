using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxQueue {

    #region Properties

    public BeltBoard Board { get; }
    public Vector2Int Index { get; }
    public Direction Direction { get; }
    public Belt Belt { get; }
    public Vector2 Position { get; }
    public Box CurrentBox { get; private set; }
    public Box NextBox { get; private set; }
    public bool IsEmpty => NextBox == null && 
                           _boxesRemaining.Count == 0 &&
                           (CurrentBox == null || CurrentBox.State == BoxState.Complete);
    public BoxQueueObject Object { get; private set; }
    public ColorType CurrentColor => CurrentBox?.Color ?? ColorType.None;
    public IReadOnlyList<Box> BoxesAll => _boxesAll;
    
    #endregion

    #region Fields
    
    // 데이터에 남아있는 박스들(완성되면 사라지는 데이터)
    private readonly List<Box> _boxesRemaining = new();
    // 모든 박스의 데이터들(완성되도 남아있는 데이터)
    private readonly List<Box> _boxesAll = new();

    #endregion

    #region Constructor

    public BoxQueue(BeltBoard board, BoxQueueData data) {
        Board = board;
        Index = data.Index;
        Direction = data.Direction;
        Belt = board[Index + Direction.GetIndex()];
        Belt.AddBoxQueue(this);
        Position = new(Index.x * Board.BeltSize.x, Index.y * Board.BeltSize.y);

        foreach (BoxData boxData in data.Boxes)
        {
            Box box = new(this, boxData);
            _boxesRemaining.Add(box);
            _boxesAll.Add(box);
        }
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<BoxQueueObject>($"BoxQueueObject_{Direction}");
        Object.Set(this);
        Spawn();
    }

    #endregion

    #region Pull
    
    // Next를 Current로 미는 메서드
    public void Pull() {
        // 다음 박스가 준비가 되지 않았으면 취소
        if (NextBox?.Object == null) return;
        
        // 현재 박스가 존재하는 상태라면 취소
        if (CurrentBox != null) return;
        
        // 다음 박스가 그룹 기믹을 가지고 있는지 확인
        if (NextBox.TryGetGimmick(out BoxGroup group))
        {
            // 해당 박스를 Pull 대기상태로 설정
            NextBox.State = BoxState.PullingWaiting;
            group.TryPullGroupGimmick(group.Count);
        }
        else ForcePull();
    }

    public void ForcePull()
    {
        CurrentBox = NextBox;
        SetNextBoxNull();
        CurrentBox?.Pull();
    }

    public void TrySetBoxNull(Box box)
    {
        if(CurrentBox == box) SetCurrentBoxNull();
        else if (NextBox == box) SetNextBoxNull();
    }
    public void SetNextBoxNull() => NextBox = null;
    public void SetCurrentBoxNull() => CurrentBox = null;

    public void Spawn() {
        // 다음 박스가 준비되지 않을 경우 취소
        if (NextBox != null || _boxesRemaining.Count <= 0) return;
        
        // 다음 박스 생성
        NextBox = _boxesRemaining[0];
        _boxesRemaining.RemoveAt(0);
        
        // 생성한 박스가 그룹 기믹을 가지고 있는지 확인
        if (NextBox.TryGetGimmick(out BoxGroup group))
        {
            // 그룹 기믹 상태일 경우 상자를 스폰 대기상대로 설정.
            NextBox.State = BoxState.SpawnWaiting;
            group.TrySpawnGroupGimmick(group.Count);
        }
        else NextBox.Spawn();
    }

    public void RemoveBox(Box box)
    {
        _boxesRemaining.Remove(box);
    }

    #endregion

    #region Knit

    public bool EnterBox(Knit knit) {
        Box checkBox = CurrentBox;
        if (checkBox == null) return false;
        if (CurrentBox.State == BoxState.Complete)
        {
            checkBox = NextBox;
            if (checkBox == null) return false;
            // 히든 기믹을 가진 Next박스일 경우에는 들어가지 않도록 설정.
            if (checkBox.HasGimmick(BoxGimmickType.Hidden)) return false;
        }
        return checkBox.EnterBox(knit);
    } 

    public List<Box> GetBoxList()
    {
        List<Box> boxes = new();
        if (CurrentBox != null)
        {
            boxes.Add(CurrentBox);
        }

        if (NextBox != null)
        {
            boxes.Add(NextBox);
        }
        boxes.AddRange(_boxesRemaining);
        return boxes;
    }

    #endregion

}