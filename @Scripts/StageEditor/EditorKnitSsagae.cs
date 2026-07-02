using UnityEngine;

public class EditorKnitSsagae : Entity
{
    #region Properties

    public EditorBeltBoard Board { get; protected set; }
    public KnitSsagaeData Data { get; protected set; }

    public Vector2Int Index
    {
        get => Data.Index;
        set => Data.Index = value;
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;
        
        return true;
    }

    public void Set(EditorBeltBoard board, KnitSsagaeData data)
    {
        Initialize();
        Board = board;
        Data = data;
        this.transform.name = $"KnitKeeper[{Index.x}, {Index.y}]";
        this.transform.position = new(Index.x, Index.y);
    }

    #endregion

    public KnitSsagaeData GetData()
    {
        return new KnitSsagaeData
        {
            Index = Data.Index,
        };
    }
}
