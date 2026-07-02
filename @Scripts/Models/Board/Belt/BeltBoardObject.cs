using UnityEngine;

public class BeltBoardObject : Entity {

    public BeltBoard Board { get; private set; }

    public void Set(BeltBoard board) {
        Initialize();
        Board = board;

        this.transform.name = $"Belts";
        this.transform.SetParent(Board.Board.Object.transform);
        this.transform.position = Board.Center;
    }

}