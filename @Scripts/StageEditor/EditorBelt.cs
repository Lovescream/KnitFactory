using UnityEngine;

public class EditorBelt : Entity {

    #region Properties

    public EditorBeltBoard Board { get; protected set; }
    public BeltData Data { get; protected set; }

    public Vector2Int Index {
        get => Data.Index;
        set => Data.Index = value;
    }

    public Direction StartDirection {
        get => Data.StartDirection;
        protected set => Data.StartDirection = value;
    }

    public Direction EndDirection {
        get => Data.EndDirection;
        protected set => Data.EndDirection = value;
    }
    
    #endregion
    
    #region Fields
    
    protected SpriteRenderer _spriter;
    protected SpriteRenderer _arrowSpriter;

    #endregion
    
    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _spriter = this.GetComponent<SpriteRenderer>();
        _arrowSpriter = this.gameObject.FindChild<SpriteRenderer>("Arrow");
        
        return true;
    }

    // public void Set(EditorBeltBoard board, Vector2Int index, Direction start, Direction end) {
    //     Initialize();
    //     Board = board;
    //     Index = index;
    //     StartDirection = start;
    //     EndDirection = end;
    //     this.transform.name = $"Belt[{index.x}, {index.y}]";
    //     this.transform.position = new(Index.x, Index.y);
    //     SetSprite();
    // }
    
    public void Set(EditorBeltBoard board, BeltData data) {
        Initialize();
        Board = board;
        Data = data;
        this.transform.name = $"Belt[{Index.x}, {Index.y}]";
        this.transform.position = new(Index.x, Index.y);
        SetSprite();
    }

    protected void SetSprite() {
        bool isCurve = StartDirection.GetOpposite() != EndDirection;
        bool isCW = StartDirection.GetClockwise() == EndDirection;
        _spriter.sprite = Main.Resource.Get<Sprite>($"Belt_{(isCurve ? "Curve" : "Straight")}");
        int r = 4 - (int)StartDirection;
        if (isCW) r -= 1;
        else if (!isCurve) r -= 3;
        _spriter.transform.localRotation = Quaternion.Euler(0, 0, r * 90);
        _arrowSpriter.transform.localPosition = isCurve ? new(-0.1f, 0.15f) : Vector2.zero;
        r = 90;
        if (isCurve) r += isCW ? -135 : 45;
        _arrowSpriter.transform.localRotation = Quaternion.Euler(0, 0, r);
    }

    #endregion

    public BeltData GetData() {
        return new BeltData {
            Index = Index,
            StartDirection = StartDirection,
            EndDirection = EndDirection,
        };
    }
}