using UnityEngine;

public class UI_BundleBoardObject : Entity {

    public BundleBoard Board { get; private set; }

    public void Set(BundleBoard board, Transform parent) {
        Initialize();
        Board = board;

        this.transform.name = $"Bundles";
        this.transform.SetParent(parent, false);
    }

}