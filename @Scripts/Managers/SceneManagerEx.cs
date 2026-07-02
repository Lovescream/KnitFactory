using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneManagerEx : CoreManager {

    public SceneBase Current { get; set; }
    public UI_Scene SceneUI { get; set; }

    public void Load(string sceneName) {
        Main.Clear();
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchAsync(string sceneName, bool showLoading = true) {
        if (showLoading)
        {
            Main.Loading.Show(LoadingType.Transition, () => 
                Main.StartCoroutine(SwitchSceneAsync(sceneName)));
        }
        else
        {
            Main.StartCoroutine(SwitchSceneAsync(sceneName));
        }
    }
    
    public void Reload() {
        Main.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadSceneAsync(string sceneName, Action<float> onProgress = null) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f) {
            onProgress?.Invoke(operation.progress / 0.9f);
            yield return null;
        }
        onProgress?.Invoke(1f);

        void OnLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name != sceneName) return;
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnLoaded;
        }
        SceneManager.sceneLoaded += OnLoaded;
        
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;
    }

    private IEnumerator SwitchSceneAsync(string sceneName, Action<float> onProgress = null) {
        // #2. 클리어.
        Main.Clear();
        
        // #3. 새 씬 로드.
        yield return LoadSceneAsync(sceneName, onProgress);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        
        // #4. 이전 씬 언로드.
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName) continue;
            yield return SceneManager.UnloadSceneAsync(scene);
        }
        
        // #5. 로딩 숨기기.
        Main.Loading.Hide();
    }

}

public abstract class SceneBase : MonoBehaviour {
    
    private bool _initialized;

    protected virtual void Start() { 
        //Initialize();
        StartCoroutine(WaitActiveThenInitialize());
    }

    protected virtual void Update() {
        
    }

    private IEnumerator WaitActiveThenInitialize() {
        Scene scene = this.gameObject.scene;
        yield return new WaitUntil(() => SceneManager.GetActiveScene() == scene);
        Initialize();
    }

    protected virtual bool Initialize() {
        if (_initialized) return false;
        _initialized = true;

        Main.Scene.Current = this;
            
        if (FindFirstObjectByType<EventSystem>() == null) {
            GameObject prefab = Main.Resource.Get<GameObject>("EventSystem");
            if (prefab == null) {
                Debug.LogError($"[Scene] Initialize(): Failed to load EventSystem prefab.");
                return false;
            }
            Instantiate(prefab).name = "EventSystem";
        }
        
        Main.Data.PrefsSync();

        return true;
    }
}