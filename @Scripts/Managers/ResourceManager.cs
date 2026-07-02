using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceManager : CoreManager {

    private Dictionary<Type, Dictionary<string, Object>> _resources = new();

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        Load<Sprite>(BlossomPath.RESOURCES_SPRITES);
        Load<GameObject>(BlossomPath.RESOURCES_PREFABS);
        Load<AudioClip>(BlossomPath.RESOURCES_AUDIO);
        Load<FontsSo>(BlossomPath.RESOURCES_FONTS);
        
        return true;
    }

    private void Load<T>(string path, Func<T, string> keyFinder = null) where T : Object {
        keyFinder ??= x => x.name;
        _resources[typeof(T)] = Resources.LoadAll<T>(path).ToDictionary(keyFinder, x => (Object)x);
    }

    public bool IsExist<T>(string key) where T : Object {
        if (!_resources.TryGetValue(typeof(T), out Dictionary<string, Object> dictionary)) return false;
        return dictionary.ContainsKey(key);
    }
    
    public T Get<T>(string key) where T : Object {
        if (!_resources.TryGetValue(typeof(T), out Dictionary<string, Object> dictionary)) {
            Debug.LogError($"[ResourceManager] Get<{typeof(T)}>({key}): Failed to get resource. The Resource of type {typeof(T)} does not exist.");
            return null;
        }
        if (!dictionary.TryGetValue(key, out Object resource)) {
            Debug.LogError($"[ResourceManager] Get<{typeof(T)}>({key}): Failed to get resource. The Resource with the key {key} does not exist.");
            return null;
        }
        return resource as T;
    }
    
    public List<T> GetAll<T>() where T : Object {
        if (!_resources.TryGetValue(typeof(T), out Dictionary<string, Object> dictionary)) {
            Debug.LogError($"[ResourceManager] GetAll<{typeof(T)}>(): Failed to get resource. The Resource of type {typeof(T)} does not exist.");
            return null;
        }
        return dictionary.Values.Select(x => x as T).ToList();
    }
    
}