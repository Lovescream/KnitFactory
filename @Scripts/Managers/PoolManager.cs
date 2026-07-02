using System.Collections.Generic;
using UnityEngine;

public class Pool {
    private readonly GameObject _prefab;
    private readonly Stack<GameObject> _pool;
    private Transform _rootroot;
    private Transform _root;

    private Transform Root {
        get {
            if (_root == null) {
                GameObject rootObject = new GameObject($"[Pool_Root] {_prefab.name}");
                _root = rootObject.transform;
                _root.SetParent(_rootroot);
            }
            return _root;
        }
    }

    public Pool(GameObject prefab, Transform rootroot = null) {
        _prefab = prefab;
        _rootroot = rootroot;
        _pool = new Stack<GameObject>();
    }

    public GameObject Pop() {
        GameObject obj;
        if (_pool.Count > 0) obj = _pool.Pop();
        else obj = OnCreate();
        OnGet(obj);
        return obj;
    }

    public void Push(GameObject obj) {
        OnRelease(obj);
        _pool.Push(obj);
    }

    public void Clear() {
        foreach (var obj in _pool)
            OnDestroy(obj);
        _pool.Clear();
    }

    #region Callback
    
    private GameObject OnCreate() {
        GameObject obj = Object.Instantiate(_prefab, Root);
        if (obj.TryGetComponent(out IPoolable poolable)) poolable.PoolKey = _prefab.name;
        obj.name = _prefab.name;
        return obj;
    }

    private void OnGet(GameObject obj) {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj) {
        obj.transform.SetParent(Root);
        obj.SetActive(false);
    }
    
    private void OnDestroy(GameObject obj) {
        Object.Destroy(obj);
    }
    
    #endregion
}

public interface IPoolable {
    public string PoolKey { get; set; }
    public void OnRelease();
}

public class PoolManager : CoreManager {
    
    private readonly Dictionary<string, Pool> _pools = new();
    private Transform _root;
    private Transform Root {
        get {
            if (_root == null) {
                GameObject rootObject = new GameObject($"[Pool_Root]");
                _root = rootObject.transform;
            }
            return _root;
        }
    }


    public GameObject Pop(GameObject prefab) {
        if (!_pools.ContainsKey(prefab.name)) 
            CreatePool(prefab);
        return _pools[prefab.name].Pop();
    }
    
    public bool Push(GameObject obj) {
        if (!obj.TryGetComponent(out IPoolable poolable) ||
            !_pools.TryGetValue(poolable.PoolKey, out Pool pool)) return false;
        
        pool.Push(obj);
        return true;
    }

    private void CreatePool(GameObject prefab) {
        Pool pool = new Pool(prefab, Root);
        _pools.Add(prefab.name, pool);
    }

    public override void Clear() {
        foreach (var pool in _pools.Values) {
            pool.Clear();
        }
        _pools.Clear();
    }
}