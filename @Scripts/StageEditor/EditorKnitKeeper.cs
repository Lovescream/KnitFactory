using UnityEngine;

public class EditorKnitKeeper : Entity
{
    #region Properties

    public EditorBeltBoard Board { get; protected set; }
    public KnitKeeperData Data { get; protected set; }

    public Vector2Int Index
    {
        get => Data.Index;
        set => Data.Index = value;
    }

    public Orientation Orientation
    {
        get => Data.Orientation;
        set => Data.Orientation = value;
    }

    #endregion

    #region Fields

    protected SpriteRenderer _sprinter;

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _sprinter = this.GetComponent<SpriteRenderer>();
        
        return true;
    }

    public void Set(EditorBeltBoard board, KnitKeeperData data)
    {
        Initialize();
        Board = board;
        Data = data;
        this.transform.name = $"KnitKeeper[{Index.x}, {Index.y}]";
        this.transform.position = new(Index.x, Index.y);
        SetSprite();
    }

    protected void SetSprite()
    {
        _sprinter.sprite = Main.Resource.Get<Sprite>($"Knit_Keeper_{Orientation}");
        _sprinter.transform.localRotation = Quaternion.identity;
        _sprinter.transform.localScale = Vector3.one;
    }

    #endregion

    public KnitKeeperData GetData()
    {
        return new KnitKeeperData
        {
            Index = Data.Index,
            Orientation = Data.Orientation,
        };
    }
}
