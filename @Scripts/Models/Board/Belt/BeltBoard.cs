using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Board;
using UnityEngine;

public class BeltBoard {

    #region Consts.

    public readonly Vector2 BeltSize = Vector2.one;
    private readonly Vector2Int MinSize = new Vector2Int(4, 4);

    #endregion

    #region Properties

    public Board Board { get; }
    public Vector2Int Size { get; private set; }
    public Vector2Int Min { get; }
    public Vector2Int Max { get; }
    public Vector2 Center { get; }
    public BeltBoardObject Object { get; private set; }
    public KnitSsagae Ssagae { get; private set; }
    public Belt FirstBelt { get; private set; }
    public IReadOnlyList<BoxQueue> BoxesQueue => _boxQueues.Select(x => x.Value).ToList();
    public IReadOnlyDictionary<int, List<Box>> BoxGroupGimmick => _boxGroupGimmick;
    public IReadOnlyDictionary<Vector2Int, Belt>  Belts => _belts;
    public IReadOnlyDictionary<int, MoveMachine> InputMoveMachines => _inputMoveMachines;
    public IReadOnlyDictionary<int, MoveMachine> OutputMoveMachines => _outputMoveMachines;
    public IReadOnlyList<Box> BoxesAll => BoxesQueue.SelectMany(queue => queue.BoxesAll).ToList();
    public Dictionary<int, BoxObject> BoxGroupObject => _boxGroupObjects;

    #endregion

    #region Fields

    private readonly Dictionary<Vector2Int, Belt> _belts = new();
    private readonly Dictionary<Vector2Int, BoxQueue> _boxQueues = new();
    private readonly Dictionary<Vector2Int, KnitSsagae> _knitSsagae = new();
    private readonly Dictionary<Vector2Int, MoveMachine> _moveMachines = new();
    private readonly Dictionary<Vector2Int, KnitKeeper> _knitKeeper = new();
    private readonly Dictionary<int, MoveMachine> _inputMoveMachines = new();
    private readonly Dictionary<int, MoveMachine> _outputMoveMachines = new();
    private readonly Dictionary<int, List<Box>> _boxGroupGimmick = new();
    private readonly Dictionary<int, BoxObject> _boxGroupObjects = new();
    
    #endregion

    #region Indexer

    public Belt this[int x, int y] => _belts.GetValueOrDefault(new(x, y));
    public Belt this[Vector2Int index] => _belts.GetValueOrDefault(index);

    #endregion

    #region Constructor

    public BeltBoard(Board board, StageDataKey stageDataKey) {
        Board = board;
        
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        
        // #1. stageData를 기반으로 각 데이터를 생성.
        foreach (BeltData beltData in stageDataKey.Belts) {
            Vector2Int index = beltData.Index;
            _belts[index] = new(this, beltData);
            if (minX > index.x) minX = index.x;
            if (maxX < index.x) maxX = index.x;
            if (minY > index.y) minY = index.y;
            if (maxY < index.y) maxY = index.y;
        }
        foreach (BoxQueueData boxQueueData in stageDataKey.BoxQueues) {
            Vector2Int index = boxQueueData.Index;
            BoxQueue boxQueue = new(this, boxQueueData);
            _boxQueues[index] = boxQueue;
            if (minX > index.x) minX = index.x;
            if (maxX < index.x) maxX = index.x;
            if (minY > index.y) minY = index.y;
            if (maxY < index.y) maxY = index.y;
        }
        foreach (KnitSsagaeData knitSsagaeData in stageDataKey.KnitSsagaes) {
            Vector2Int index = knitSsagaeData.Index;
            _knitSsagae[index] = new(this, knitSsagaeData);
            if (minX > index.x) minX = index.x;
            if (maxX < index.x) maxX = index.x;
            if (minY > index.y) minY = index.y;
            if (maxY < index.y) maxY = index.y;
        }
        foreach (KnitKeeperData knitKeeperData in stageDataKey.KnitKeepers) {
            Vector2Int index = knitKeeperData.Index;
            _knitKeeper[index] = new(this, knitKeeperData);
            if (minX > index.x) minX = index.x;
            if (maxX < index.x) maxX = index.x;
            if (minY > index.y) minY = index.y;
            if (maxY < index.y) maxY = index.y;
        }
        foreach (MoveMachineData moveMachineData in stageDataKey.MoveMachines) {
            Vector2Int index = moveMachineData.Index;
            MoveMachine moveMachine = new (this, moveMachineData);
            _moveMachines[index] = moveMachine;
            if (moveMachineData.PortType == PortType.Input)
                _inputMoveMachines[moveMachineData.ConnectIndex] = moveMachine;
            else
                _outputMoveMachines[moveMachineData.ConnectIndex] = moveMachine;
            
            if (minX > index.x) minX = index.x;
            if (maxX < index.x) maxX = index.x;
            if (minY > index.y) minY = index.y;
            if (maxY < index.y) maxY = index.y;
        }

        // #2. 크기 및 위치 설정.
        Min = new(minX, minY);
        Max = new(maxX, maxY);
        Size = new(Mathf.Max(MinSize.x, maxX - minX + 1), Mathf.Max(MinSize.y, maxY - minY + 1));
        
        Center = (Max + Min) * BeltSize * 0.5f;
        Ssagae = _knitSsagae.FirstOrDefault().Value;
        
        SetConnects();
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<BeltBoardObject>();
        Object.Set(this);
        foreach (Belt belt in _belts.Values) belt.GenerateObject();
        foreach (BoxQueue queue in _boxQueues.Values) queue.GenerateObject();
        foreach (KnitSsagae knitSsagae in _knitSsagae.Values) knitSsagae.GenerateObject();
        foreach (KnitKeeper knitKeeper in _knitKeeper.Values) knitKeeper.GenerateObject();
        foreach (MoveMachine moveMachine in _moveMachines.Values) moveMachine.GenerateObject();
        
        foreach (BoxQueue queue in _boxQueues.Values) queue.Pull();
        foreach (BoxQueue queue in _boxQueues.Values) queue.Spawn();
    }

    // 그룹 기믹에 해당 박스를 추가
    public void AddGroupGimmick(int count, Box box)
    {
        if(!_boxGroupGimmick.ContainsKey(count)) _boxGroupGimmick[count] = new ();
        _boxGroupGimmick[count].Add(box);
    }

    #endregion

    public bool IsAllClear() {
        return _boxQueues.Values.All(b => b.IsEmpty);
    }
    
    private void SetConnects() {
        foreach (Belt belt in _belts.Values) belt.SetConnect();
    }

    //CurrentBox에 SpriteDim 처리를 해줌.
    public void SetMaskCurrentBox()
    {
        foreach (BoxQueue queue in _boxQueues.Values)
        {
            MaskData maskData = queue.CurrentBox?.GetMaskData();
            if (maskData == null) continue;
            Board.SpriteDim.Object.AddSpriteMask(maskData);
        }
    }
    
    // 근처 상하좌우의 벨트 중 index를 향하는 벨트를 가져오는 함수
    public Belt GetNearEndBelt(Vector2Int index)
    {
        // 검사 순서: 왼쪽, 오른쪽, 아래, 위
        Vector2Int[] offsets = { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
        Direction[] checkDirections = { Direction.Right, Direction.Left, Direction.Top, Direction.Bottom };

        for (int i = 0; i < offsets.Length; i++) {
            Belt neighbor = GetBelt(index + offsets[i]);
            if (IsEndDirection(neighbor, checkDirections[i])) return neighbor;
        }

        return null;
    }

    // 근처 상하좌우의 벨트 중 index를 향하는 벨트를 가져오는 함수
    public Belt GetNearStartBelt(Vector2Int index)
    {
        // 검사 순서: 왼쪽, 오른쪽, 아래, 위
        Vector2Int[] offsets = { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
        Direction[] checkDirections = { Direction.Right, Direction.Left, Direction.Top, Direction.Bottom };

        for (int i = 0; i < offsets.Length; i++) {
            Belt neighbor = GetBelt(index + offsets[i]);
            if (IsStartDirection(neighbor, checkDirections[i])) return neighbor;
        }

        return null;
    }

    public Belt GetBelt(Vector2Int index) {
        return Belts.GetValueOrDefault(index);
    }

    private bool IsStartDirection(Belt belt, Direction checkDirection)
    {
        if (belt == null) return false;
        if (belt.StartDirection != checkDirection) return false;
        return true;
    }

    public bool IsEndDirection(Belt belt, Direction checkDirection)
    {
        if (belt == null) return false;
        if (belt.EndDirection != checkDirection) return false;
        return true;
    }
}