using UnityEngine;

public class BundleBoardObject : Entity {
    // TODO: 해당 스크립트는 없애던가 되돌려야함.
    public BundleBoard Board { get; private set; }

    public void Set(BundleBoard board) {
        Initialize();
        Board = board;

        this.transform.name = $"Bundles";
    }
}