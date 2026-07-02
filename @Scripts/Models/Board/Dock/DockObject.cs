using UnityEngine;

public class DockObject : Entity {
    // TODO: 해당 스크립트는 없애던가 되돌려야함.

    #region Properties

    public Dock Dock { get; private set; }

    #endregion
    
    #region Initialize / Set

    public void Set(Dock dock) {
        Initialize();
        Dock = dock;
    }

    #endregion
    
}