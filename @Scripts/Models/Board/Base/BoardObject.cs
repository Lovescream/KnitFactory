using UnityEngine;

public class BoardObject : Entity {

    private Board _board;
    public Transform TRKnits { get; private set; }
    public Transform TRSlots { get; private set; }

    public override bool Initialize()
    {
        if(!base.Initialize()) return false;

        TRKnits = gameObject.FindChild<Transform>("Knits");
        TRSlots = gameObject.FindChild<Transform>("Slots");
        
        return true;
    }

    public void Set(Board board) {
        Initialize();
        _board = board;

        this.transform.name = $"FuckDuckCatNyan";
        this.transform.SetParent(null);
        this.transform.position = _board.Center;
    }

}