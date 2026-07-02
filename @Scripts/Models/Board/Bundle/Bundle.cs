using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;

public class Bundle
{
    #region Const.

    private const int MaxWaiting = 16;
    private const float KnitWidthGap = 0.15f;
    
    private const float RightEndKnitYGap = 0.01f;

    #endregion

    #region Properties

    public BundleState State { get; set; }
    public BundleBoard Board { get; private set; }
    public UI_BundleObject Object { get; private set; }
    public IReadOnlyList<Knit> OriginKnits => _originalKnits;
    public IReadOnlyList<Knit> InKnit => _inKnits;
    public IReadOnlyList<Knit> WaitingKnits => _waitingKnits;
    public IReadOnlyDictionary<int, Knit> KnitsGroup => _knitsGroup;
    public IReadOnlyDictionary<int, KnitArray> KnitArrays => _knitArrays;
    public float StartY => Object.TargetStartPos.position.y;
    private float EndY => Object.TargetEndPos.position.y;
    private float StartX => Object.TargetStartPos.position.x;
    private float GapY => (StartY - EndY) / MaxWaiting;

    #endregion

    #region Fields
    
    private readonly List<Knit> _originalKnits = new();
    private readonly List<Knit> _inKnits = new();
    private readonly List<Knit> _waitingKnits = new();
    private readonly Dictionary<int, Knit> _knitsGroup = new();
    private readonly Dictionary<int, KnitArray> _knitArrays = new();

    #endregion

    #region Constructor

    public Bundle(BundleBoard board, BundleData data)
    {
        Board = board;
        if (data == null || data.Knits == null || data.Knits.Count == 0) return;

        foreach (KnitData knitData in data.Knits)
        {
            Knit knit = new(this, knitData);
            _inKnits.Add(knit);
            _originalKnits.Add(knit);
            knit.State = KnitState.InBundle;
        }

        State = BundleState.Idle;
    }

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<UI_BundleObject>();
        Object.Set(this);
    }

    public void SetKnitArrays()
    {
        _knitArrays.Clear();
        List<Knit> knits = GetAllKnits();
        List<Knit> newKnits = new();
        if (knits == null || knits.Count == 0) return;
        int nowIndex = 0;
        ColorType nowColor = knits[0].Color;
        for (int i = 0; i < knits.Count; i++)
        {
            if (nowColor != knits[i].Color)
            {
                _knitArrays[nowIndex] = new(this, newKnits, nowIndex);
                nowIndex++;
                newKnits = new();
                nowColor = knits[i].Color;
            }
            newKnits.Add(knits[i]);
        }
        _knitArrays[nowIndex] = new(this, newKnits, nowIndex);
    }


    public void AddGroupKnit(int groupCount, Knit knit) => _knitsGroup.Add(groupCount, knit);

    #endregion

    #region Knits

    public void ResetList()
    {
        _waitingKnits.Clear();
        _inKnits.Clear();
    }

    public void AddInKnit(Knit knit) => _inKnits.Add(knit);
    public void AddWaitKnit(Knit knit) => _waitingKnits.Add(knit);
    public void RemoveInKnit(Knit knit) => _inKnits.Remove(knit);
    public void RemoveWaitKnit(Knit knit) => _waitingKnits.Remove(knit);
    public bool ContainInKnit(Knit knit) => _inKnits.Contains(knit);
    public bool ContainWaitKnit(Knit knit) => _waitingKnits.Contains(knit);
    public bool IsFirstKnit(Knit knit) => _waitingKnits.Count > 0 && _waitingKnits[0] == knit;
    
    // 번들에서 소환 가능한 모든 털실 소환.
    public void SpawnKnitAll() => Object?.SpawnKnitAll();
    
    // 번들 클릭 활성화 여부
    public void SetActiveBundleClick(bool active) => Object?.SetActiveBundleClick(active);

    // Knit를 꺼내고 오브젝트를 생성한다. (꺼낼 수 없다면 null 리턴)
    public Knit SpawnKnitOnce()
    {
        // #1. 꺼낼 수 있는 Knit가 없거나 이미 꺼낼 수 있을 만큼 모두 꺼냈다면 꺼내지 않음.
        if (_inKnits.Count == 0 || _waitingKnits.Count >= MaxWaiting) return null;

        // #2. 하나를 꺼낸다.
        Knit knit = _inKnits[0];
        _inKnits.Remove(knit);
        _waitingKnits.Add(knit);

        // #3. 오브젝트 생성.
        knit.Spawn();
        return knit;
    }

    public void SetDataInKnitsForKnitArrays()
    {
        foreach (KnitArray knitArray in _knitArrays.Values)
        {
            foreach (Knit knit in knitArray.Knits)
            {
                knit.SetBundle(this);
                AddInKnit(knit);
            }
        }
        _knitArrays.Clear();
    }

    public void DestroyAllKnitObjects()
    {
        List<Knit> knits = GetAllKnits();
        foreach (Knit knit in knits)
        {
            knit.Destroy();
        }
    }

    // 대기중인 Knit의 Position 계산.
    public Vector2 GetPosition(Knit knit)
    {
        if (!_waitingKnits.Contains(knit))
        {
            Debug.LogError($"obj id Not found: {knit}");
            return Vector2.zero;
        }
        int index = _waitingKnits.IndexOf(knit);
        float x, y;
        if (index % 2 == 0)
        {
            x = StartX - KnitWidthGap * Main.Screen.Camera.ApplyRatio();
            y = EndY + GapY * index;
        }
        else
        {
            x = StartX + KnitWidthGap * Main.Screen.Camera.ApplyRatio();
            y = EndY + RightEndKnitYGap + GapY * index;
        }

        return new(x, y);
    }

    // 대기중인 모든 Knit들을 당긴다.
    public void Pull()
    {
        foreach (Knit knit in _waitingKnits) knit.Pull();
    }

    // 다음 나갈 수 있는 Knit의 개수 반환: 색상이 같은 Knit의 묶음.
    public int GetNextKnitCount()
    {
        if (_waitingKnits.Count == 0) return 0;
        ColorType color = _waitingKnits[0].Color;
        int count = 0;
        foreach (Knit knit in _waitingKnits)
        {
            if (knit.Color == color) count++;
            else break;
        }

        return count;
    }


    public List<Knit> DequeueKnits()
    {
        // #1. 빼야 할 Knit의 개수 계산: 0이라면 null 리턴.
        int nextCount = GetNextKnitCount();
        if (nextCount == 0) return null;

        // #2. 벨트 위에 올라갈 수 있는 최대 수를 넘어선다면 null 리턴.
        if (Main.Board.CanKnitsOnBeltCount < nextCount) return null;

        // #3. 빼
        List<Knit> list = new();
        for (int i = 0; i < nextCount; i++)
        {
            list.Add(_waitingKnits[0]);
            _waitingKnits.RemoveAt(0);
        }

        return list;
    }

    public List<Knit> TryGetHiddenKnits()
    {
        List<Knit> hiddenKnits = new();
        if (_waitingKnits.Count == 0 || !_waitingKnits[0].Has(KnitGimmickType.Hidden)) return hiddenKnits;
        ColorType color = _waitingKnits[0].Color;

        List<Knit> allKnits = GetAllKnits();

        for (int i = 0; i < allKnits.Count; i++)
        {
            if (color != allKnits[i].Color) break;
            if (!allKnits[i].Has(KnitGimmickType.Hidden)) break;
            hiddenKnits.Add(allKnits[i]);
        }

        return hiddenKnits;
    }

    //대기열에 있는 모든 Knit를 호출 가능한 순서대로 반환
    public List<Knit> GetAllKnits()
    {
        List<Knit> allKnits = new();
        allKnits.AddRange(_waitingKnits);
        allKnits.AddRange(_inKnits);
        return allKnits;
    }

    public Dictionary<int, Knit> GetKnitForColorIndex(ColorType color)
    {
        Dictionary<int, Knit> resultDict = new();
        List<Knit> allKnits = GetAllKnits();
        for (int i = 0; i < allKnits.Count; i++)
        {
            if (allKnits[i].Color == color) resultDict.Add(i, allKnits[i]);
        }

        return resultDict;
    }

    public void RemoveKnit(Knit knit)
    {
        if (_waitingKnits.Contains(knit))
        {
            knit.Destroy();
            _waitingKnits.Remove(knit);
        }
        else if (_inKnits.Contains(knit))
        {
            _inKnits.Remove(knit);
        }
        else
        {
            Debug.LogError($"not found knit : {knit}");
            return;
        }
    }

    public void PullAllKnits()
    {
        foreach (var knit in _waitingKnits)
        {
            knit.Pull();
        }
        
        SpawnKnitAll();
    }

    public List<Knit> GetLastKnitGroup()
    {
        List<Knit> knits = new();
        if (_waitingKnits.Count <= 0) return knits;
        ColorType color = _waitingKnits[0].Color;
        for (int i = 0; i < _waitingKnits.Count; i++)
        {
            if (color == _waitingKnits[i].Color)
            {
                // Debug.LogError($"add Knit : {waitingKnits[i]}, Color: {color}");
                knits.Add(_waitingKnits[i]);
            }
            else break;
        }

        return knits;
    }

    #endregion
}

public enum BundleState
{
    None = -1,
    Idle = 0,
    Spawning,
    BAUBAU,
}

public class KnitArray
{
    public Bundle Bundle { get; private set; }
    public List<Knit> Knits { get; private set; }
    private int BundleIndex { get; set; }

    private KnitArray PrevKnitArray
    {
        get
        {
            if (Bundle?.KnitArrays == null) return null;
            Bundle.KnitArrays.TryGetValue(BundleIndex - 1, out KnitArray prev);
            return prev;
        }
    }

    private KnitArray NextKnitArray
    {
        get
        {
            if (Bundle?.KnitArrays == null) return null;
            Bundle.KnitArrays.TryGetValue(BundleIndex + 1, out KnitArray next);
            return next;
        }
    }


    public KnitArray(Bundle bundle, List<Knit> knits, int bundleIndex)
    {
        this.Bundle = bundle;
        this.Knits = knits;
        this.BundleIndex = bundleIndex;
    }

    public bool TrySwapKnitArray(KnitArray newArray)
    {
        if (!CanSwapKnitArray(newArray)) return false;
        (this.Knits, newArray.Knits) = (newArray.Knits, this.Knits);
        return true;
    }

    private bool CanSwapKnitArray(KnitArray newArray)
    {
        // 갯수 동일한지 확인
        if (GetCount() != newArray.GetCount()) return false;

        // 컬러가 다른지 확인
        if (GetColorType() == newArray.GetColorType()) return false;

        // 타입들을 합치고 int로 변환 후 겹치는 컬러가 있는지 확인
        int checkColorTypes = (int)GetColorType() | (int)newArray.GetColorType();
        int nearColorTypes = GetIntNearColorTypes() | newArray.GetIntNearColorTypes();
        if ((checkColorTypes & nearColorTypes) != 0) return false;
        return true;
    }

    public ColorType GetColorType()
    {
        if (Knits == null || Knits.Count == 0) return ColorType.None;
        return Knits[0]?.Color ?? ColorType.None;
    }

    private int GetIntNearColorTypes()
    {
        int prevColorTypeToInt = (int)(PrevKnitArray?.GetColorType() ?? ColorType.None);
        int nextColorTypeToInt = (int)(NextKnitArray?.GetColorType() ?? ColorType.None);
        return prevColorTypeToInt | nextColorTypeToInt;
    }

    public int GetCount() => Knits.Count;
}