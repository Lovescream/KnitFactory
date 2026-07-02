using System.Collections.Generic;
using UnityEngine;

public class KnitSsagae {

    #region Properties

    public BeltBoard Board { get; }
    public Vector2 Position { get; }

    public bool HasKnit => _queue.Count > 0;

    public KnitSsagaeObject Object { get; private set; }
    private Belt StartBelt { get; }

    #endregion
    
    #region Fields
    private BeltLane _nextLane;
    
    private readonly Queue<Knit> _queue = new();

    #endregion

    #region Constructor

    public KnitSsagae(BeltBoard board, KnitSsagaeData stageData) {
        Board = board;
        Position = stageData.Index;
        StartBelt = Board.GetNearStartBelt(stageData.Index);
        _nextLane = BeltLane.Inner;
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<KnitSsagaeObject>();
        Object.Set(this);
    }
    
    #endregion

    public void Enqueue(Knit knit) {
        _queue.Enqueue(knit);
    }

    public void Poop() {
        Knit knit = _queue.Dequeue();
        if (knit == null) return;
        knit.MoveToBelt(_nextLane, StartBelt);
        _nextLane = _nextLane == BeltLane.Inner ? BeltLane.Outer : BeltLane.Inner;
    }

    public void RemoveAllKnits() => _queue.Clear();
}