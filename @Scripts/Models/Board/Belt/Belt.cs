using System.Collections.Generic;
using UnityEngine;

public class Belt {

    #region Const.

    private const float LaneOffset = 0.18f;

    #endregion
    
    #region Properties

    public BeltBoard Board { get; }
    public Vector2Int Index { get; }
    public Vector2 Position { get; }
    public Direction StartDirection { get; }
    public Direction EndDirection { get; }
    public Vector2 StartPosition { get; }
    public Vector2 EndPosition { get; }

    public Belt PrevBelt { get; private set; }
    public Belt NextBelt { get; private set; }
    public MoveMachine MoveMachine { get; set; }

    public BeltObject Object { get; private set; }

    #endregion
    
    #region Fields

    private readonly List<BoxQueue> _boxQueues = new();

    #endregion

    #region Constructor

    public Belt(BeltBoard board, BeltData data) {
        Board = board;
        Index = data.Index;
        Position = new(Index.x * Board.BeltSize.x, Index.y * Board.BeltSize.y);
        StartDirection = data.StartDirection;
        EndDirection = data.EndDirection;
        StartPosition = Position + StartDirection.GetIndex() * Board.BeltSize * 0.5f;
        EndPosition = Position + EndDirection.GetIndex() * Board.BeltSize * 0.5f;
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<BeltObject>();
        Object.Set(this);
    }

    public BeltData GetData() {
        return new() {
            Index = Index,
            StartDirection = StartDirection,
            EndDirection = EndDirection,
        };
    }

    public override string ToString() {
        return $"Belt[{Index}]";
    }

    public float GetRotationZArrow()
    {
        if (StartDirection == Direction.Left)
        {
            return EndDirection switch
            {
                Direction.Top => 45,
                Direction.Right => 90,
                Direction.Bottom => 45,
                _ => 0,
            };
        }
        else if (StartDirection == Direction.Top)
        {
            return EndDirection switch
            {
                Direction.Right => 45,
                Direction.Bottom => 90,
                Direction.Left => 45,
                _ => 0,
            };
        }
        else if (StartDirection == Direction.Right)
        {
            return EndDirection switch
            {
                Direction.Bottom => 45,
                Direction.Left => 90,
                Direction.Top => 45,
                _ => 0,
            };
        }
        else if (StartDirection == Direction.Bottom)
        {
            return EndDirection switch
            {
                Direction.Left => 45,
                Direction.Top => 90,
                Direction.Right => 45,
                _ => 0,
            };
        }
        else
        {
            Debug.LogError($"not Fount StartDirection: {StartDirection}, EndDirection: {EndDirection}");
            return 0;
        }
    }

    public Vector3 GetPositionArrow()
    {
        return Vector3.back;
    }

    #endregion

    #region Validate / Get

    private bool IsStraight() {
        return StartDirection.GetIndex() + EndDirection.GetIndex() == Vector2Int.zero;
    }

    #endregion
    
    #region BoxQueue

    public void AddBoxQueue(BoxQueue queue) {
        _boxQueues.Add(queue);
    }

    public bool EnterBox(Knit knit) {
        foreach (BoxQueue queue in _boxQueues) {
            if (queue.EnterBox(knit)) return true;
        }

        return false;
    }
    
    #endregion

    #region Belts

    public void SetConnect() {
        PrevBelt = Board[Index + StartDirection.GetIndex()];
        NextBelt = Board[Index + EndDirection.GetIndex()];
    }

    #endregion

    #region Position

    public Vector2 GetPosition(float ratio, BeltLane lane) {
        Vector2 center = GetPosition(ratio);
        Vector2 tangent = GetTangent(ratio);
        Vector2 normal = new(-tangent.y, tangent.x);
        int sign = lane == BeltLane.Inner ? 1 : -1;
        return center + normal * sign * LaneOffset;
    }
    
    // 중앙선 위치.
    private Vector2 GetPosition(float ratio) {
        // #0. ratio 보정.
        ratio = Mathf.Clamp01(ratio);
        Vector2 p0 = StartPosition;
        Vector2 p2 = EndPosition;
        
        // #1-1. 직선 벨트인 경우.
        if (IsStraight()) return Vector2.Lerp(p0, p2, ratio);
        
        // #1-2. 코너 벨트인 경우: 부드럽게 꺾기.
        return QuadBezier(p0, Position, p2, ratio);
    }

    // 중앙선 법선 벡터.
    private Vector2 GetTangent(float ratio) {
        // #0. ratio 보정.
        ratio = Mathf.Clamp01(ratio);
        Vector2 p0 = StartPosition;
        Vector2 p2 = EndPosition;
        
        // #1-1. 직선 벨트인 경우.
        if (IsStraight()) {
            Vector2 dir = p2 - p0;
            return dir.sqrMagnitude > 1e-8f ? dir.normalized : Vector2.right;
        }
        
        // #1-2. 코너 벨트인 경우: Bezier 미분.
        Vector2 diff = 2f * (1f - ratio) * (Position - p0) + 2f * ratio * (p2 - Position);
        if (diff.sqrMagnitude < 1e-8f) diff = p2 - p0;
        return diff.normalized;
    }

    private static Vector2 QuadBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
        float u = 1f - t;
        return (u * u) * p0 + (2f * u * t) * p1 + (t * t) * p2;
    }

    #endregion
    
}

public enum BeltLane { None = -1, Inner = 0, Outer = 1 }