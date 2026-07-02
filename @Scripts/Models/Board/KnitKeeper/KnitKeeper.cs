using System.Collections.Generic;
using UnityEngine;

public class KnitKeeper {

    #region Properties

    public BeltBoard Board { get; }
    public Vector2 Position { get; }

    public bool HasKnit => _queue.Count > 0;

    public KnitKeeperObject Object { get; private set; }
    public KnitKeeperData Data { get; private set; }

    #endregion
    
    #region Fields

    private readonly Belt _toilet;
    private BeltLane _nextLane;
    
    private readonly Queue<Knit> _queue = new();

    #endregion

    #region Constructor

    public KnitKeeper(BeltBoard board, KnitKeeperData data) {
        Board = board;
        Data = data;
        Position = data.Index;
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<KnitKeeperObject>();
        Object.Set(this);
    }
    
    #endregion

    public void Enqueue(Knit knit) {
        _queue.Enqueue(knit);
    }
}