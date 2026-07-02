using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EditorController {

    #region Fields

    private readonly InputAction _actionSelect;
    private readonly InputAction _actionPosition;
    private readonly InputAction _actionRightClick;
    
    private readonly Camera _camera;

    public event Action OnActionSelectStarted;
    public event Action OnActionRightClickStarted;
    public event Action<Vector2> OnActionPositionPerformed;
    
    #endregion
    
    #region Initialize

    public EditorController() {
        _camera = Main.Screen.Camera;
        _actionSelect = InputSystem.actions.FindAction("Select");
        _actionPosition = InputSystem.actions.FindAction("Position");
        _actionRightClick = InputSystem.actions.FindAction("RightClick");
        _actionSelect.started += OnSelected;
        _actionRightClick.started += OnRightClicked;
        _actionSelect.canceled += OnCanceled;
        _actionPosition.performed += OnPositionChanged;
    }

    public void OnDestroy() {
        _actionSelect.started -= OnSelected;
        _actionRightClick.started -= OnRightClicked;
        _actionSelect.canceled -= OnCanceled;
        _actionPosition.performed -= OnPositionChanged;
    }

    #endregion

    #region Events

    private void OnSelected(InputAction.CallbackContext context) {
        OnActionSelectStarted?.Invoke();
        Main.StartCoroutine(CoSelect());
    }

    private IEnumerator CoSelect() {
        yield return null;
        if (EventSystem.current.IsPointerOverGameObject()) yield break;

        // Vector3 worldPosition = GetWorldPosition();
        // if (EditorScene.EditorState == EditorState.EditBelts) {
        //     
        // }
    }

    private void OnRightClicked(InputAction.CallbackContext context) {
        OnActionRightClickStarted?.Invoke();
        Main.StartCoroutine(CoRight());
    }

    private IEnumerator CoRight() {
        yield return null;
        if (EventSystem.current.IsPointerOverGameObject()) yield break;

        // Vector3 worldPosition = GetWorldPosition();
        // if (EditorScene.EditorState == EditorState.EditBelts) {
        //     
        // }
    }
    
    private void OnCanceled(InputAction.CallbackContext context) { }

    private void OnPositionChanged(InputAction.CallbackContext context) {
        OnActionPositionPerformed?.Invoke(GetWorldPosition());
    }

    #endregion
    
    private Vector3 GetWorldPosition() {
        Vector3 screenPosition = _actionPosition.ReadValue<Vector2>();
        return _camera.ScreenToWorldPoint(screenPosition.SetZ(_camera.transform.position.z));
    }
    
}