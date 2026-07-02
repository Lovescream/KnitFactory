using UnityEngine;
using UnityEngine.InputSystem;

public class EditorBeltCursor : EditorBelt {

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
        if (Keyboard.current.eKey.wasPressedThisFrame) {
            StartDirection++;
            if (StartDirection == EndDirection) StartDirection++;
            if ((int)StartDirection >= 4) StartDirection = Direction.Top;
            if (StartDirection == EndDirection) StartDirection++;
            SetSprite();
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame) {
            EndDirection++;
            if (EndDirection == StartDirection) EndDirection++;
            if ((int)EndDirection >= 4) EndDirection = Direction.Top;
            if (EndDirection == StartDirection) EndDirection++;
            SetSprite();
        }
    }
    
    #endregion

    #region Initialize / Set
    
    public void Set(EditorBeltBoard board) {
        Initialize();
        Board = board;
        Data = new BeltData {
            Index = Vector2Int.zero,
            StartDirection = Direction.Left,
            EndDirection = Direction.Right,
        };
        SetSprite();
        _controller = (Main.Scene.Current as EditorScene)?.InputController;
        if (_controller == null) return;
        _controller.OnActionSelectStarted += OnLeftClick;
        _controller.OnActionRightClickStarted += OnRightClick;
        _controller.OnActionPositionPerformed += OnMovePosition;
    }

    #endregion

    #region Events

    private void OnLeftClick() {
        Board.NewBelt();
    }

    private void OnRightClick() {
        Board.RemoveBelt();
    }
    
    private void OnMovePosition(Vector2 position) {
        int x = Mathf.FloorToInt(position.x + 0.5f);
        int y = Mathf.FloorToInt(position.y + 0.5f);
        CursorPosition = new(x, y);

        if (!Board.CanPlaceBelt()) {
            _spriter.color = new Color(1, 1, 1, 0);
            _arrowSpriter.color = new Color(1, 1, 1, 0);
        }
        else {
            _spriter.color = new Color(1, 1, 1, 0.3f);
            _arrowSpriter.color = new Color(1, 1, 1, 1);
        }

        this.transform.position = new(x, y);
    }

    #endregion
    
}