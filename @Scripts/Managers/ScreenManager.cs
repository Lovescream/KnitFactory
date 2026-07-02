using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenManager : CoreManager
{
    #region Const.
    public const float ReferenceCameraSize = 5.625f;
    public const float ReferenceScreenAspect = 9 / 16f;
    public const float ScaleRatio = 0.9f;

    #endregion

    #region Properties

    public Vector2 ReferenceResolution => new(1080f, 1920f);
    private float NormalizedAspect => Aspect / ReferenceScreenAspect;

    // 오브젝트 스케일 비율 보정(1에 가깝게 보정.)
    private float ComRatio => (1f - NormalizedAspect) * 0.9f;

    // 해상도 비율에 따른 오브젝트 최종 스케일 비율 값
    public float FinalScaleRatio => (NormalizedAspect + ComRatio) * ScaleRatio;
    public Vector2 CameraSize { get; private set; }
    public Vector3 CameraPosition { get; private set; }
    public float Aspect { get; private set; }
    public float ReverseAspect { get; private set; }
    private MainCamera mainCamera;
    public event Action<Camera> OnChangeCamera;

    public Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                Camera _cam = Camera.main;
                if (_cam == null)
                {
                    mainCamera = new();
                    mainCamera.GenerateObject();
                    _cam = mainCamera.Object.Camera;
                }

                Camera = _cam;
            }

            return _camera;
        }
        set
        {
            _camera = value;
            OnChangeCamera?.Invoke(value);
        }
    }

    // private SpriteRenderer _background;

    #endregion

    #region Fields

    private Camera _camera;

    #endregion

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        Application.targetFrameRate = 60;
        // TODO:: 나중에 구조 개선 필요
        mainCamera = new();
        Camera = Camera.main;
        GameEvents.OnGameReady += () => SetActiveRayCast2D(false);

        return true;
    }

    public void SetActiveRayCast2D(bool isActive)
    {
        Camera.GetComponent<Physics2DRaycaster>().enabled = isActive;
    }

    public Vector3 GetScreenPos(Vector3 worldPos) => Camera.WorldToScreenPoint(worldPos);

    public override void Clear()
    {
        base.Clear();
        _camera = null;
    }

    public void ResetCamera()
    {
        Camera.orthographicSize = 3;
        Camera.transform.position = new Vector3(0, 0, -10);
    }

    public void SetGameCameraPos()
    {
        Initialize();
        
        UI_GameScene sceneUI = Main.Scene.SceneUI as UI_GameScene;
        BeltBoard board = Main.Board.Current.BeltBoard;
        
        // 카메라 위치 계산
        float cameraY = sceneUI.BoardPixelCenterY - board.Center.y;
        CameraPosition = new Vector3(board.Center.x, -cameraY, -10);
        Camera.transform.position = CameraPosition;
    }

    public void SetGameCameraSize()
    {
        Initialize();

        // #1. 보드, UI 위치 및 사이즈 캐싱
        UI_GameScene sceneUI = Main.Scene.SceneUI as UI_GameScene;
        BeltBoard board = Main.Board.Current.BeltBoard;
        float width = board.Size.x + 1f;
        float height = board.Size.y + 1f;
        
        // #2. 화면 비율 계산
        Aspect = Screen.width / (float)Screen.height;
        ReverseAspect = Screen.height / (float)Screen.width;

        // #3. 카메라 사이즈 계산
        float sizeByHeight = (height / sceneUI.BoardPixelHeight) * (Screen.height / 2f);
        float sizeByWidth = (width / sceneUI.BoardPixelWidth) * (Screen.width / 2f);
        sizeByWidth /= Aspect;
        
        float finalSize = Mathf.Max(sizeByWidth, sizeByHeight);
        Camera.orthographicSize = finalSize;
    }
}