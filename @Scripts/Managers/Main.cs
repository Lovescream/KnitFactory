using System.Collections;
using System.Reflection;
using ActionFit_Plugin.IAP;
using ActionFit_Plugin.SDK;
using ActionFit_Plugin.Settings;
using JSAM;
using UnityEngine;

public class Main : MonoBehaviour {
    
    #region Singleton

    private static Main _instance;

    public static Main Instance {
        get {
            if (_instance == null) Initialize();
            return _instance;
        }
    }

    #endregion

    #region Properties

    // Core.
    public static PoolManager Pool => Instance?._pool;
    public static ResourceManager Resource => Instance?._resource;
    public static ScreenManager Screen => Instance?._screen;
    public static DataManager Data => Instance?._data;
    public static SceneManagerEx Scene => Instance?._scene;
    public static LoadingManager Loading => Instance?._loading;
    
    // Content.
    public static UIManager UI => Instance?._ui;
    public static ObjectManager Object => Instance?._object;
    public static LivesManager Lives => Instance?._lives;
    public static AdsManager Ads => Instance?._ads;
    public static IAPInitializer IAP => Instance?._iap;
    public static TimeManager Time => Instance?._time;
    public static BoardManager Board => Instance?._board;
    public static SoManager So => Instance?._so;
    public static TextManager Text => Instance?._text;
    public static MetaManager Meta => Instance?._meta;
    
    #endregion

    #region Fields

    public static bool IsEditorMode = false;
    
    // Core.
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly ScreenManager _screen = new();
    private readonly SceneManagerEx _scene = new();
    private readonly DataManager _data = new();
    private readonly LoadingManager _loading = new();
    
    // Content.
    private readonly UIManager _ui = new();
    private readonly ObjectManager _object = new();
    private readonly LivesManager _lives = new();
    private readonly AdsManager _ads = new();
    private readonly IAPInitializer _iap = new();
    private readonly TimeManager _time = new();
    private readonly BoardManager _board = new();
    private readonly SoManager _so = new();
    private readonly TextManager _text = new();
    private readonly MetaManager _meta = new();
    
    private static bool _initialized;

    #endregion

    #region Initialize

    private static void Initialize() {
        if (_instance != null || _initialized) return;
        _initialized = true;
        
        PlayerData.Initialized();

        GameObject obj = GameObject.Find("@Main");
        if (obj == null) {
            obj = new("@Main");
            obj.AddComponent<Main>();
        }

        DontDestroyOnLoad(obj);
        _instance = obj.GetComponent<Main>();
        
        // 코어 매니저 초기화
        foreach (FieldInfo fieldInfo in typeof(Main).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
            CoreManager manager = fieldInfo.GetValue(_instance) as CoreManager;
            manager?.Initialize();
        }
        
        // 컨텐츠 매니저 초기화
        foreach (FieldInfo fieldInfo in typeof(Main).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
            ContentManager manager = fieldInfo.GetValue(_instance) as ContentManager;
            manager?.Initialize();
        }

        Application.targetFrameRate = 60;
        
        //Haptic.Initialize();
            
        Setting.Initialized();
        
        // 씬에서 오디오 매니저를 찾고 없다면 새로 생성
        if (!FindFirstObjectByType<AudioManager>())
        {
            GameObject go = Resource.Get<GameObject>("AudioManagerObject");
            Instantiate(go);
        }

        _ = Loading.InitializeSDK();
    }

    #endregion

    #region MonoBehaviours

    void OnApplicationFocus(bool hasFocus) {
        //if (hasFocus) Ads?.ShowResumeInterstitial();
    }

    #endregion

    #region CoroutineHelper

    public new static Coroutine StartCoroutine(IEnumerator coroutine) => (Instance as MonoBehaviour).StartCoroutine(coroutine);
    public new static void StopCoroutine(Coroutine coroutine) => (Instance as MonoBehaviour).StopCoroutine(coroutine);

    #endregion

    public static void Clear() {
        foreach (FieldInfo fieldInfo in typeof(Main).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
            CoreManager manager = fieldInfo.GetValue(_instance) as CoreManager;
            manager?.Clear();
        }
        foreach (FieldInfo fieldInfo in typeof(Main).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
            ContentManager manager = fieldInfo.GetValue(_instance) as ContentManager;
            manager?.Clear();
        }
    }
    
}

public abstract class CoreManager {
    private bool _initialized;
    public bool IsInitialized => _initialized;

    public virtual bool Initialize() {
        if (_initialized) return false;
        _initialized = true;
        return true;
    }

    public virtual void Clear() { }
}

public abstract class ContentManager {
    private bool _initialized;
    public bool IsInitialized => _initialized;

    public virtual bool Initialize() {
        if (_initialized) return false;
        _initialized = true;
        return true;
    }

    public virtual void Clear() { }
}

public static class BlossomPath {
    public static readonly string RESOURCES_SPRITES = $"Sprites";
    public static readonly string RESOURCES_PREFABS = $"Prefabs";
    public static readonly string RESOURCES_MATERIALS = $"Materials";
    public static readonly string RESOURCES_MESHES = $"Meshes";
    public static readonly string RESOURCES_DATA = $"Data";
    public static readonly string RESOURCES_STAGEDATA = $"StageData";
    public static readonly string RESOURCES_TEXTURES = $"Textures";
    public static readonly string RESOURCES_AUDIO = $"AudioClips";
    public static readonly string RESOURCES_SO = $"Sodesuka";
    public static readonly string RESOURCES_FONTS = $"FontsSO";

    public static readonly string CSVDATA = $"CSV";
}

public static class Styles {
    public static readonly Color BUTTONCOLOR_DISABLED = new(100 / 255f, 100 / 255f, 100 / 255f, 1);
}