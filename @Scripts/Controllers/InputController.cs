using UnityEngine;
using UnityEngine.InputSystem;

public class InputController {

    #region Properties

    public static bool AllowInput { get; set; } = true;
    public static bool UsingItem { get; set; } = false;

    // public static BlockController Current { get; private set; }
    // public static BlockObject CurrentObject { get; private set; }

    #endregion
    
    #region Fields

    private readonly InputAction _actionSelect;
    private readonly InputAction _actionPosition;
    
    private readonly Camera _camera;

    #endregion
    
    #region Initialize

    public InputController() {
        _camera = Main.Screen.Camera;
        _actionSelect = InputSystem.actions.FindAction("Select");
        _actionPosition = InputSystem.actions.FindAction("Position");
        _actionSelect.started += OnSelected;
        _actionSelect.canceled += OnCanceled;
        _actionPosition.performed += OnPositionChanged;
        GameEvents.OnGamePause += OnGamePause;
        GameEvents.OnGameResume += OnGameResume;
    }

    public void OnDestroy() {
        _actionSelect.started -= OnSelected;
        _actionSelect.canceled -= OnCanceled;
        _actionPosition.performed -= OnPositionChanged;
        GameEvents.OnGamePause -= OnGamePause;
        GameEvents.OnGameResume -= OnGameResume;
    }

    #endregion

    #region Events

    private void OnSelected(InputAction.CallbackContext context) 
    {
        if (!AllowInput || UsingItem) return;

        Vector3 worldPosition = GetWorldPosition();
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);
        if (hits.Length <= 0) return;

        foreach (RaycastHit2D hit in hits) {
            if (hit.collider == null) continue;

            if (hit.collider.TryGetComponent(out KnitObject knitObject)) {
                knitObject.OnSelected();
            }
        }
    }

    private void OnCanceled(InputAction.CallbackContext context) {
    }

    private void OnPositionChanged(InputAction.CallbackContext context) {
        if (!AllowInput || UsingItem) return;
    }

    private void OnGamePause() => AllowInput = false;
    private void OnGameResume() => AllowInput = true;
    
    #endregion
    
    public Vector3 GetWorldPosition() {
        Vector3 screenPosition = _actionPosition.ReadValue<Vector2>();
        return _camera.ScreenToWorldPoint(screenPosition.SetZ(_camera.transform.position.z));
    }
    
}