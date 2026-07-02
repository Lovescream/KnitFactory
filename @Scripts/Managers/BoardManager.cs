using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : ContentManager {
    
    #region Properties

    public int MaxKnitsOnBelt { get; private set; }
    public Board Current { get; private set; }
    public BeltBoard BeltBoard => Current.BeltBoard;
    public Dock Dock => (Main.Scene.Current as GameScene)?.SceneUI.Dock;
    public BundleBoard Bundle => (Main.Scene.Current as GameScene)?.SceneUI.Bundle;
    public IReadOnlyList<Knit> KnitsOnBelt => _knitsOnBelt;
    // 벨트 위에 올릴 수 있는 최대 털실 개수를 반환
    public int CanKnitsOnBeltCount => MaxKnitsOnBelt - KnitsOnBelt.Count;

    #endregion

    #region Fields

    private readonly List<Knit> _knitsOnBelt = new();

    public event Action OnChangedKnitsOnBelt;

    #endregion

    #region Generate

    public void GenerateBoard(StageDataKey dataKey, UI_GameScene sceneUI) {
        Current = new(dataKey, sceneUI);
        MaxKnitsOnBelt = dataKey.MaxKnitsOnBelt;
    }

    public void GenerateBoardObject() {
        Current?.GenerateObject();
    }

    public override void Clear() {
        base.Clear();
        Current = null;
        _knitsOnBelt.Clear();
        OnChangedKnitsOnBelt = null;
    }

    #endregion

    #region Knits

    public void AddKnitsOnBelt(Knit knit) {
        _knitsOnBelt.Add(knit);
        OnChangedKnitsOnBelt?.Invoke();
    }

    public void RemoveKnitsOnBelt(Knit knit) {
        _knitsOnBelt.Remove(knit);
        knit.Belt = null;
        OnChangedKnitsOnBelt?.Invoke();
    }

    public void RemoveKnitsAll()
    {
        _knitsOnBelt.Clear();
        OnChangedKnitsOnBelt?.Invoke();
    }

    public void AddKnitsCountOnBelt(int addCount)
    {
        MaxKnitsOnBelt += addCount;
        Main.Board.OnChangedKnitsOnBelt?.Invoke();
    }

    #endregion

    public void CheckClear() {
        if (BeltBoard.IsAllClear()) {
            GameScene.GameState = GameState.Success;
        }
    }

}