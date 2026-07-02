using UnityEngine;
using UnityEngine.InputSystem;

public class EditorBoxQueueCursor : EditorBoxQueue {

    #region Properties

    public Vector2Int CursorPosition { get; private set; }

    #endregion
    
    #region Fields

    private EditorController _controller;

    #endregion

    #region MonoBehaviours

    void OnDisable() {
        if (_controller != null) {
            _controller.OnActionSelectStarted -= OnLeftClick;
            _controller.OnActionRightClickStarted -= OnRightClick;
            _controller.OnActionPositionPerformed -= OnMovePosition;
        }
    }

    void Update() {
        if (Keyboard.current.rKey.wasPressedThisFrame) {
            Direction++;
            if ((int)Direction >= 4) Direction = Direction.Top;
            SetView();
        }
    }

    #endregion

    #region Initialize / Set
    
    public void Set(EditorBeltBoard board) {
        Initialize();
        Board = board;
        Data = new BoxQueueData {
            Index = Vector2Int.zero,
            Direction = Direction.Top,
            Boxes = new()
        };
        SetView();
        _controller = (Main.Scene.Current as EditorScene)?.InputController;
        if (_controller == null) return;
        _controller.OnActionSelectStarted += OnLeftClick;
        _controller.OnActionRightClickStarted += OnRightClick;
        _controller.OnActionPositionPerformed += OnMovePosition;
    }

    #endregion
    
    #region Events

    private void OnLeftClick() {
        Board.NewBoxQueue();
    }

    private void OnRightClick() {
        Board.RemoveBoxQueue();
    }
    
    private void OnMovePosition(Vector2 position) {
        int x = Mathf.FloorToInt(position.x + 0.5f);
        int y = Mathf.FloorToInt(position.y + 0.5f);
        CursorPosition = new(x, y);

        SetView(Board.CanPlaceBoxQueue(CursorPosition, Direction));
        this.transform.position = new(x, y);
    }

    #endregion

}