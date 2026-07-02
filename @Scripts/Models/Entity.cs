using UnityEngine;

public class Entity : MonoBehaviour, IPoolable {
    
    public string PoolKey { get; set; }

    private bool _isInitialized;

    void Start() {
        Initialize();
    }

    public virtual bool Initialize() {
        if (_isInitialized) return false;
        _isInitialized = true;
        
        return true;
    }

    public virtual void OnRelease() { }


}