using UnityEngine;

public class EditorMoveMachine : Entity
{

    #region Properties

    public EditorBeltBoard Board { get; protected set; }
    public MoveMachineData Data { get; protected set; }

    public Vector2Int Index {
        get => Data.Index;
        set => Data.Index = value;
    }

    public int ConnectIndex
    {
        get => Data.ConnectIndex;
        set => Data.ConnectIndex = value;
    }

    public Orientation Orientation
    {
        get => Data.Orientation;
        set => Data.Orientation = value;
    }

    public PortType PortType
    {
        get => Data.PortType;
        set => Data.PortType = value;
    }
    
    #endregion
    
    #region Fields
    
    protected SpriteRenderer _spriter;

    #endregion
    
    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _spriter = this.GetComponent<SpriteRenderer>();
        
        return true;
    }
    
    public void Set(EditorBeltBoard board, MoveMachineData data) {
        Initialize();
        Board = board;
        Data = data;
        this.transform.name = $"Belt[{Index.x}, {Index.y}]";
        this.transform.position = new(Index.x, Index.y);
        SetSprite();
    }

    public void SetOrientation(Orientation orientation)
    {
        Orientation = orientation;
    }

    public void SetConnectIndex(int connectIndex)
    {
        ConnectIndex = connectIndex;
    }

    public void SetSprite() {
        _spriter.sprite = Main.Resource.Get<Sprite>($"Knit_MoveMachine_{Orientation}");
        _spriter.transform.localRotation = Quaternion.identity;
        _spriter.transform.localScale = Vector3.one;
    }

    #endregion

    public MoveMachineData GetData() {
        return new MoveMachineData {
            Index = Index,
            ConnectIndex = ConnectIndex,
            Orientation = Orientation,
            PortType = PortType,
        };
    }
}
