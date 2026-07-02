using UnityEngine;

public class EditorBoxQueue : Entity {

    #region Propertis
    
    public EditorBeltBoard Board { get; protected set; }

    public BoxQueueData Data { get; protected set; }
    public Vector2Int Index => Data.Index;

    public Direction Direction {
        get => Data.Direction;
        protected set => Data.Direction = value;
    }
    
    #endregion

    #region Fields
    
    private readonly GameObject[] _views = new GameObject[4];
    
    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        Transform nana = this.gameObject.FindChild<Transform>("NANA");
        for (int i = 0; i < 4; i++) _views[i] = nana.GetChild(i).gameObject;
        
        return true;
    }

    // public void Set(EditorBeltBoard board, Vector2Int index, Direction direction) {
    //     Initialize();
    //     Board = board;
    //     Data = new BoxQueueData {
    //         Index = index,
    //         Direction = direction,
    //         Boxes = new()
    //     };
    //     this.transform.name = $"Queue[{index.x}, {index.y}] ({direction})";
    //     this.transform.position = new(Index.x, Index.y);
    //     SetView();
    // }

    public void Set(EditorBeltBoard board, BoxQueueData data) {
        Initialize();
        Board = board;
        Data = data;
        this.transform.name = $"Queue[{Index.x}, {Index.y}] ({Direction})";
        this.transform.position = new(Index.x, Index.y);
        SetView();
    }
    
    protected void SetView(bool show = true) {
        foreach (GameObject view in _views) view.SetActive(false);
        if (!show) return;
        _views[(int)Data.Direction].SetActive(true);
    }

    #endregion

}