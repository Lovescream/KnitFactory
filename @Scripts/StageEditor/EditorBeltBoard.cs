using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorBeltBoard {

    #region Properties

    public Vector2Int Size { get; private set; }
    public Vector2Int Min { get; private set; }
    public Vector2Int Max { get; private set; }
    public Vector2 Center { get; private set; }
    
    public EditorBeltCursor BeltCursor { get; private set; }
    public EditorBoxQueueCursor BoxQueueCursor { get; private set; }
    public EditorSelectCursor SelectCursor { get; private set; }
    public EditorMoveMachineCursor MoveMachineCursor { get; private set; }
    public EditorKnitSsagaeCursor KnitSsagaeCursor { get; private set; }
    public EditorKnitKeeperCursor KnitKeeperCursor { get; private set; }

    public GameObject Object { get; private set; }

    public IReadOnlyDictionary<ColorType, int> RequiredKnitCounts => _requiredKnitCounts;
    
    #endregion
    
    #region Fields

    private readonly Dictionary<Vector2Int, EditorBelt> _belts = new();
    private readonly Dictionary<Vector2Int, EditorBoxQueue> _boxQueues = new();
    private readonly Dictionary<EditorBoxQueue, Vector2Int[]> _nanaQ = new();
    private readonly Dictionary<Vector2Int, EditorMoveMachine> _moveMachines = new();
    private readonly Dictionary<Vector2Int, EditorKnitKeeper> _knitKeepers = new();
    private readonly Dictionary<Vector2Int, EditorKnitSsagae> _knitSsagae = new();
    private Dictionary<ColorType, int> _requiredKnitCounts = new();

    private UI_EditorScene _sceneUI;

    #endregion

    #region Constructor

    public EditorBeltBoard() {
        _sceneUI = (Main.Scene.Current as EditorScene)?.SceneUI;
        
        Object = new("NANABOARD");
        SetBeltCursor();

        StageDataKey stageDataKey = EditorScene.CurrentStage;
        if (stageDataKey == null) return;
        if (stageDataKey.Belts != null)
            foreach (BeltData beltData in stageDataKey.Belts) NewBelt(beltData);
        if (stageDataKey.BoxQueues != null)
            foreach (BoxQueueData queueData in stageDataKey.BoxQueues) NewBoxQueue(queueData);
        if (stageDataKey.MoveMachines != null)
            foreach (MoveMachineData moveMachineData in stageDataKey.MoveMachines) NewMoveMachine(moveMachineData);
        if (stageDataKey.KnitKeepers != null)
            foreach (KnitKeeperData knitKeeperData in stageDataKey.KnitKeepers) NewKnitKeeper(knitKeeperData);
        if(stageDataKey.KnitSsagaes != null)
            foreach(KnitSsagaeData knitSsagaeData in stageDataKey.KnitSsagaes) NewKnitSsagae(knitSsagaeData);

        RecalculateCounts();
    }

    #endregion

    #region Show / Hide

    public void Show() {
        Object.SetActive(true);
        SetSelectCursor();
        RecalculateBoard();
        SetCamera();
    }

    public void Hide() {
        Object.SetActive(false);
        ResetCursor();
    }

    #endregion

    #region Validate

    public bool IsEmpty(Vector2Int index) {
        HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>(
            _belts.Keys
                .Concat(_boxQueues.Keys)
                .Concat(_nanaQ.Values.SelectMany(v => v))
                .Concat(_moveMachines.Keys)
                .Concat(_knitKeepers.Keys));
        if (hashSet.Contains(index)) return false;
        return true;
    }

    #endregion

    #region Cursor

    public void ResetBeltCursor()
    {
        if (BeltCursor != null) {
            Main.Object.Destroy(BeltCursor);
            BeltCursor = null;
        }
    }

    public void ResetBoxQueueCursor()
    {
        if (BoxQueueCursor != null) {
            Main.Object.Destroy(BoxQueueCursor);
            BoxQueueCursor = null;
        }
    }

    public void ResetMoveMachineCursor()
    {
        if (MoveMachineCursor != null) {
            Main.Object.Destroy(MoveMachineCursor);
            MoveMachineCursor = null;
        }
    }

    public void ResetKnitKeeperCursor()
    {
        if (KnitKeeperCursor != null) {
            Main.Object.Destroy(KnitKeeperCursor);
            KnitKeeperCursor = null;
        }
    }

    public void ResetKnitSsagaeCursor()
    {
        if (KnitSsagaeCursor != null) {
            Main.Object.Destroy(KnitSsagaeCursor);
            KnitSsagaeCursor = null;
        }
    }

    public void ResetSelectCursor()
    {
        if (SelectCursor != null) {
            Main.Object.Destroy(SelectCursor);
            SelectCursor = null;
        }
    }

    public void ResetOtherObject(int resetEnum)
    {
        if((resetEnum & (int)ResetObjectType.Select) == 0) ResetSelectCursor();
        if((resetEnum & (int)ResetObjectType.Belt) == 0) ResetBeltCursor();
        if((resetEnum & (int)ResetObjectType.BoxQueue) == 0) ResetBoxQueueCursor();
        if((resetEnum & (int)ResetObjectType.MoveMachine) == 0) ResetMoveMachineCursor();
        if((resetEnum & (int)ResetObjectType.KnitKeeper) == 0) ResetKnitKeeperCursor();
        if((resetEnum & (int)ResetObjectType.KnitSsagae) == 0) ResetKnitSsagaeCursor();
    }

    public void SetBeltCursor() {
        if (BeltCursor != null) return;
        ResetOtherObject((int)ResetObjectType.Belt);
        BeltCursor = Main.Object.Instantiate<EditorBeltCursor>(parent: Object.transform);
        BeltCursor.Set(this);
        _sceneUI.SetDescription(true, typeof(EditorBeltCursor));
    }

    public void SetBoxQueueCursor() {
        if (BoxQueueCursor != null) return;
        ResetOtherObject((int)ResetObjectType.BoxQueue);
        BoxQueueCursor = Main.Object.Instantiate<EditorBoxQueueCursor>(parent: Object.transform);
        BoxQueueCursor.Set(this);
        _sceneUI.SetDescription(true, typeof(EditorBoxQueueCursor));
    }

    public void SetSelectCursor() {
        if (SelectCursor != null) return;
        ResetOtherObject((int)ResetObjectType.Select);
        SelectCursor = Main.Object.Instantiate<EditorSelectCursor>(parent: Object.transform);
        SelectCursor.Set(this);
        _sceneUI.SetDescription(true, typeof(EditorSelectCursor));
    }

    public void SetMoveMachineCursor()
    {
        if (MoveMachineCursor != null) return;
        ResetOtherObject((int)ResetObjectType.MoveMachine);
        MoveMachineCursor = Main.Object.Instantiate<EditorMoveMachineCursor>(parent: Object.transform);
        MoveMachineCursor.Set(this);
        _sceneUI.SetDescription(true, typeof(EditorMoveMachineCursor));
    }

    public void SetKnitSsagaeCursor()
    {
        if (KnitSsagaeCursor != null) return;
        ResetOtherObject((int)ResetObjectType.KnitSsagae);
        KnitSsagaeCursor = Main.Object.Instantiate<EditorKnitSsagaeCursor>(parent: Object.transform);
        KnitSsagaeCursor.Set(this);
        _sceneUI.SetDescription(true, typeof(EditorKnitSsagaeCursor));
    }

    public void SetKnitKeeperCursor()
    {
        if (KnitKeeperCursor != null) return;
        ResetOtherObject((int)ResetObjectType.KnitKeeper);
        KnitKeeperCursor = Main.Object.Instantiate<EditorKnitKeeperCursor>(parent: Object.transform);
        KnitKeeperCursor.Set(this);
        _sceneUI.SetDescription(true, typeof(EditorKnitKeeperCursor));
    }

    private void ResetCursor() {
        ResetOtherObject(0);
        _sceneUI.SetDescription(false);
    }
    
    #endregion

    #region Belt

    public bool CanPlaceBelt() {
        return IsEmpty(BeltCursor.CursorPosition);
    }
    
    public void NewBelt(BeltData data = null) {
        data ??= new BeltData {
            Index = BeltCursor.CursorPosition,
            StartDirection = BeltCursor.StartDirection,
            EndDirection = BeltCursor.EndDirection,
        };
        
        if (!IsEmpty(data.Index)) return;

        EditorBelt newBelt = Main.Object.Instantiate<EditorBelt>(parent: Object.transform);
        newBelt.Set(this, data);
        //newBelt.Set(this, index, BeltCursor.StartDirection, BeltCursor.EndDirection);
        _belts[data.Index] = newBelt;
        
        RecalculateBoard();
        SetCamera();
    }

    public void RemoveBelt() {
        Vector2Int index = BeltCursor.CursorPosition;
        if (!_belts.TryGetValue(index, out EditorBelt belt)) return;
        
        Main.Object.Destroy(belt);
        _belts.Remove(index);
        
        RecalculateBoard();
        SetCamera();
    }

    public EditorBelt GetBelt(Vector2Int index) {
        return _belts.GetValueOrDefault(index);
    }

    // 근처 상하좌우의 벨트 중 index를 향하는 벨트를 가져오는 함수
    public EditorBelt GetNearEndBelt(Vector2Int index)
    {
        // 검사 순서: 왼쪽, 오른쪽, 아래, 위
        Vector2Int[] offsets = { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
        Direction[] checkDirections = { Direction.Right, Direction.Left, Direction.Top, Direction.Bottom };

        for (int i = 0; i < offsets.Length; i++) {
            EditorBelt neighbor = GetBelt(index + offsets[i]);
            if (IsEndDirection(neighbor, checkDirections[i])) return neighbor;
        }

        return null;
    }

    // 근처 상하좌우의 벨트 중 index를 향하는 벨트를 가져오는 함수
    public EditorBelt GetNearStartBelt(Vector2Int index)
    {
        // 검사 순서: 왼쪽, 오른쪽, 아래, 위
        Vector2Int[] offsets = { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
        Direction[] checkDirections = { Direction.Right, Direction.Left, Direction.Top, Direction.Bottom };

        for (int i = 0; i < offsets.Length; i++) {
            EditorBelt neighbor = GetBelt(index + offsets[i]);
            if (IsStartDirection(neighbor, checkDirections[i])) return neighbor;
        }

        return null;
    }

    public Orientation GetBeltEndOrientation(Vector2Int index, out PortType portType)
    {
        Orientation result = Orientation.None;
        portType = PortType.Input;

        EditorBelt belt = GetNearEndBelt(index);
        if (belt == null) return result;
        if (belt.EndDirection == Direction.Right || belt.EndDirection == Direction.Left)
        {
            result = Orientation.Horizontal;
        }
        else result = Orientation.Vertical;
        return result;
    }

    public Orientation GetBeltStartOrientation(Vector2Int index, out PortType portType)
    {
        Orientation result = Orientation.None;
        portType = PortType.Output;

        EditorBelt belt = GetNearStartBelt(index);
        if (belt == null) return result;
        if (belt.EndDirection == Direction.Right || belt.EndDirection == Direction.Left)
        {
            result = Orientation.Horizontal;
        }
        else result = Orientation.Vertical;
        return result;
    }

    public bool IsEndDirection(EditorBelt belt, Direction checkDirection)
    {
        if (belt == null) return false;
        if (belt.EndDirection != checkDirection) return false;
        return true;
    }

    public bool IsStartDirection(EditorBelt belt, Direction checkDirection)
    {
        if (belt == null) return false;
        if (belt.StartDirection != checkDirection) return false;
        return true;
    }

    #endregion

    #region BoxQueue

    public bool CanPlaceBoxQueue(Vector2Int index, Direction direction) {
        Vector2Int[] indexes = new Vector2Int[3];
        Vector2Int offset = direction.GetOpposite().GetIndex();
        indexes[0] = index;
        indexes[1] = indexes[0] + offset;
        indexes[2] = indexes[1] + offset;
        foreach (Vector2Int i in indexes)
            if (!IsEmpty(i))
                return false;
        return _belts.ContainsKey(indexes[0] + direction.GetIndex());
    }
    
    public void NewBoxQueue(BoxQueueData data = null) {
        data ??= new BoxQueueData {
            Index = BoxQueueCursor.CursorPosition,
            Direction = BoxQueueCursor.Direction,
            Boxes = new()
        };
        if (!CanPlaceBoxQueue(data.Index, data.Direction)) return;
        
        EditorBoxQueue newQueue = Main.Object.Instantiate<EditorBoxQueue>(parent: Object.transform);
        newQueue.Set(this, data);
        _boxQueues[data.Index] = newQueue;
        
        Vector2Int offset = data.Direction.GetOpposite().GetIndex();
        _nanaQ.Add(newQueue, new[] { data.Index + offset, data.Index});
        
        RecalculateBoard();
        SetCamera();
    }

    public void RemoveBoxQueue(EditorBoxQueue queue = null) {
        if (queue == null) {
            Vector2Int index = BoxQueueCursor.CursorPosition;
            if (!_boxQueues.TryGetValue(index, out queue)) return;
        }
        
        Main.Object.Destroy(queue);
        _nanaQ.Remove(queue);
        _boxQueues.Remove(queue.Index);

        RecalculateBoard();
        SetCamera();
    }

    public void RecalculateCounts() {
        _requiredKnitCounts = _boxQueues.Values.Where(q => q.Data != null && q.Data.Boxes != null)
            .SelectMany(q => q.Data.Boxes)
            .GroupBy(b => b.Color)
            .ToDictionary(g => g.Key, g => g.Count() * 6);
    }

    #endregion
    
    #region MoveMachine

    public void NewMoveMachine(MoveMachineData data = null)
    {
        data ??= new MoveMachineData {
            Index = MoveMachineCursor.CursorPosition,
            ConnectIndex = MoveMachineCursor.ConnectIndex,
            Orientation = MoveMachineCursor.Orientation,
        };
        if (!CanPlaceMoveMachine(data.Index, out Orientation orientation, out PortType portType)) return;
        data.Orientation = orientation;
        data.PortType = portType;
        
        EditorMoveMachine newMoveMachine = Main.Object.Instantiate<EditorMoveMachine>(parent: Object.transform);
        newMoveMachine.Set(this, data);
        _moveMachines[data.Index] = newMoveMachine;
        
        RecalculateBoard();
        SetCamera();
    }

    public bool CanPlaceMoveMachine(Vector2Int index, out Orientation orientation, out PortType portType)
    {
        orientation = Orientation.None;
        portType = PortType.None;
        if (!IsEmpty(index)) return false;
        
        orientation = GetBeltEndOrientation(index, out portType);
        if(orientation == Orientation.None) orientation = GetBeltStartOrientation(index, out portType);
        return orientation != Orientation.None;
    }

    public void RemoveMoveMachine()
    {
        Vector2Int index = MoveMachineCursor.CursorPosition;
        if (!_moveMachines.TryGetValue(index, out EditorMoveMachine moveMachine)) return;
        RemoveMoveMachine(moveMachine);
    }

    public void RemoveMoveMachine(EditorMoveMachine moveMachine)
    {
        Main.Object.Destroy(moveMachine);
        _moveMachines.Remove(moveMachine.Index);
        
        RecalculateBoard();
        SetCamera();
    }
    
    #endregion
    
    #region KnitKeeper

    public void NewKnitKeeper(KnitKeeperData data = null)
    {
        data ??= new KnitKeeperData {
            Index = KnitKeeperCursor.CursorPosition,
            Orientation = KnitKeeperCursor.Orientation,
        };
        if (!CanPlaceKnitKeeper(data.Index, out Orientation orientation)) return;
        data.Orientation = orientation;
        
        EditorKnitKeeper newKnitKeeper = Main.Object.Instantiate<EditorKnitKeeper>(parent: Object.transform);
        newKnitKeeper.Set(this, data);
        _knitKeepers[data.Index] = newKnitKeeper;
        
        RecalculateBoard();
        SetCamera();
    }

    public bool CanPlaceKnitKeeper(Vector2Int index, out Orientation orientation)
    {
        orientation = Orientation.None;
        if (!IsEmpty(index)) return false;
        
        orientation = GetBeltEndOrientation(index, out PortType portType);
        if (orientation == Orientation.None) return false;
        return true;
    }

    public void RemoveKnitKeeper()
    {
        Vector2Int index = KnitKeeperCursor.CursorPosition;
        if (!_knitKeepers.TryGetValue(index, out EditorKnitKeeper knitKeeper)) return;
        
        Main.Object.Destroy(knitKeeper);
        _knitKeepers.Remove(index);
        
        RecalculateBoard();
        SetCamera();
    }
    
    #endregion
    
    #region KnitSsagae

    public void NewKnitSsagae(KnitSsagaeData data = null)
    {
        data ??= new KnitSsagaeData() {
            Index = KnitSsagaeCursor.CursorPosition,
        };
        if (!CanPlaceKnitSsagae(data.Index, out Orientation orientation)) return;
        
        EditorKnitSsagae newKnitSsagae = Main.Object.Instantiate<EditorKnitSsagae>(parent: Object.transform);
        newKnitSsagae.Set(this, data);
        _knitSsagae[data.Index] = newKnitSsagae;
        
        RecalculateBoard();
        SetCamera();
    }

    public bool CanPlaceKnitSsagae(Vector2Int index, out Orientation orientation)
    {
        orientation = Orientation.None;
        if (!IsEmpty(index)) return false;
        
        orientation = GetBeltStartOrientation(index, out PortType portType);
        if (orientation == Orientation.None) return false;
        return true;
    }

    public void RemoveKnitSsagae()
    {
        Vector2Int index = KnitSsagaeCursor.CursorPosition;
        if (!_knitSsagae.TryGetValue(index, out EditorKnitSsagae knitKeeper)) return;
        
        Main.Object.Destroy(knitKeeper);
        _knitSsagae.Remove(index);
        
        RecalculateBoard();
        SetCamera();
    }
    
    #endregion

    #region Select

    public void Select() {
        if (SelectCursor == null) return;
        if (_boxQueues.TryGetValue(SelectCursor.CursorPosition, out EditorBoxQueue queue))
        {
            Main.UI.OpenPanel<EUI_Panel_BoxQueueProperties>().Set(queue);
            ResetCursor();
        }
        else if (_moveMachines.TryGetValue(SelectCursor.CursorPosition, out EditorMoveMachine moveMachine))
        {
            Main.UI.OpenPanel<EUI_Panel_MoveMachineProperties>().Set(moveMachine);
            ResetCursor();
        }
    }

    #endregion
    
    #region Map / Camera

    private void RecalculateBoard() {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        
        HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>(
            _belts.Keys.Concat(_boxQueues.Keys).Concat(_nanaQ.Values.SelectMany(v => v)));

        foreach (Vector2Int index in hashSet) {
            if (minX > index.x) minX = index.x;
            if (maxX < index.x) maxX = index.x;
            if (minY > index.y) minY = index.y;
            if (maxY < index.y) maxY = index.y;
        }
        
        // foreach (Vector2Int index in _belts.Keys) {
        //     if (minX > index.x) minX = index.x;
        //     if (maxX < index.x) maxX = index.x;
        //     if (minY > index.y) minY = index.y;
        //     if (maxY < index.y) maxY = index.y;
        // }
        //
        // foreach (Vector2Int index in _boxQueues.Keys) {
        //     if (minX > index.x) minX = index.x;
        //     if (maxX < index.x) maxX = index.x;
        //     if (minY > index.y) minY = index.y;
        //     if (maxY < index.y) maxY = index.y;
        // }

        Min = new(minX, minY);
        Max = new(maxX, maxY);
        Size = new(maxX - minX + 1, maxY - minY + 1);
        Center = (Min + Max) * Vector2.one * 0.5f;
    }

    private void SetCamera() {
        Camera camera = Camera.main;
        if (camera == null) return;
        float reverseAspect = Screen.height / (float)Screen.width;

        camera.orthographicSize = Mathf.Max((Size.x + 4) * 0.5f * reverseAspect, (Size.y + 4) * 0.5f);
        camera.transform.position = new(Center.x, Center.y, -10);
    }

    #endregion

    #region Data
    
    public List<BeltData> GetBeltData() {
        List<BeltData> list = _belts.Values.Select(b => b.GetData()).ToList();
        foreach (BeltData data in list) data.Index -= Min;
        return list;
    }

    public List<BoxQueueData> GetBoxQueueData() {
        List<BoxQueueData> list = _boxQueues.Values.Select(b => b.Data).ToList();
        foreach (BoxQueueData data in list) data.Index -= Min;
        return list;
    }

    public List<MoveMachineData> GetMoveMachineData()
    {
        List<MoveMachineData> list = _moveMachines.Values.Select(b => b.GetData()).ToList();
        foreach (MoveMachineData data in list) data.Index -= Min;
        return list;
    }

    public List<KnitKeeperData> GetKnitKeeperData()
    {
        List<KnitKeeperData> list = _knitKeepers.Values.Select(b => b.GetData()).ToList();
        foreach (KnitKeeperData data in list) data.Index -= Min;
        return list;
    }

    public List<KnitSsagaeData> GetKnitSsagaeData()
    {
        List<KnitSsagaeData> list = _knitSsagae.Values.Select(b => b.GetData()).ToList();
        foreach (KnitSsagaeData data in list) data.Index -= Min;
        return list;
    }

    #endregion

}

[Flags]
public enum ResetObjectType
{
    Select = 1 << 0,
    Belt = 1 << 1,
    BoxQueue = 1 << 2,
    MoveMachine = 1 << 3,
    KnitKeeper = 1 << 4,
    KnitSsagae = 1 << 5,
}