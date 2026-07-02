using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainInitializer : MonoBehaviour {
    #region Fields
    private UI_LoadingCanvas _canvasLoading;

    #endregion
    
    private void Awake()
    {
        Main _ = Main.Instance;
        _canvasLoading = FindFirstObjectByType<UI_LoadingCanvas>();
        _canvasLoading.Set();
        DontDestroyOnLoad(gameObject);
    }
}