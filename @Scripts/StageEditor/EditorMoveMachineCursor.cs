using UnityEngine;
using UnityEngine.InputSystem;

public class EditorMoveMachineCursor : EditorMoveMachine {

    #region Properties

    public EditorBeltBoard Board { get; private set; }
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
    
    #endregion

    #region Initialize / Set
    
    public void Set(EditorBeltBoard board) {
        Initialize();
        Board = board;
        Data = new MoveMachineData()
        {
            Index = Vector2Int.zero,
            Orientation = Orientation.None,
        };
        _controller = (Main.Scene.Current as EditorScene)?.InputController;
        if (_controller == null) return;
        _controller.OnActionSelectStarted += OnLeftClick;
        _controller.OnActionRightClickStarted += OnRightClick;
        _controller.OnActionPositionPerformed += OnMovePosition;
    }

    #endregion

    #region Events

    private void OnLeftClick() {
        Board.NewMoveMachine();
    }

    private void OnRightClick() {
        Board.RemoveMoveMachine();
    }
    
    private void OnMovePosition(Vector2 position) {
        int x = Mathf.FloorToInt(position.x + 0.5f);
        int y = Mathf.FloorToInt(position.y + 0.5f);
        CursorPosition = new(x, y);

        if (Board.CanPlaceMoveMachine(new Vector2Int(x, y), out Orientation orientation, out PortType portType))
        {
            Orientation = orientation;
            PortType = portType;
            SetSprite();
            this.transform.position = new(x, y);
        }
    }

    #endregion
    
}