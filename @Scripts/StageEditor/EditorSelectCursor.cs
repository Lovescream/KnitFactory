using UnityEngine;
using UnityEngine.InputSystem;

public class EditorSelectCursor : Entity {

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
            _controller.OnActionSelectStarted -= Fuwawa;
            _controller.OnActionRightClickStarted -= Mococo;
            _controller.OnActionPositionPerformed -= Kanade;
        }
    }
    
    #endregion

    #region Initialize / Set
    
    public void Set(EditorBeltBoard board) {
        Initialize();
        Board = board;
        _controller = (Main.Scene.Current as EditorScene)?.InputController;
        if (_controller == null) return;
        _controller.OnActionSelectStarted += Fuwawa;
        _controller.OnActionRightClickStarted += Mococo;
        _controller.OnActionPositionPerformed += Kanade;
    }

    #endregion

    #region Events

    private void Fuwawa() {
        Board.Select();
    }

    private void Mococo() {
        
    }
    
    private void Kanade(Vector2 position) {
        int x = Mathf.FloorToInt(position.x + 0.5f);
        int y = Mathf.FloorToInt(position.y + 0.5f);
        CursorPosition = new(x, y);
    }

    #endregion
    
}