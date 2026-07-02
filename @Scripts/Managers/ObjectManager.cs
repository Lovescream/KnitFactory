using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : ContentManager {

    #region Fields

    private HashSet<Entity> _entities = new();

    #endregion
    
    #region Generals

    public override void Clear() {
        base.Clear();
        DestroyAll();
    }

    public T Instantiate<T>(string prefabName = null, Transform parent = null) where T : Entity {
        if (string.IsNullOrEmpty(prefabName)) prefabName = typeof(T).Name;
        GameObject prefab = Main.Resource.Get<GameObject>(prefabName);
        GameObject obj = Main.Pool.Pop(prefab);
        obj.transform.SetParent(parent);
        
        T entity = obj.GetComponent<T>();
        _entities.Add(entity);
        return entity;
    }

    public ParticleSystem InstantiateParticle(string prefabName, Vector2 position) {
        GameObject prefab = Main.Resource.Get<GameObject>(prefabName);
        GameObject obj = Object.Instantiate(prefab);
        obj.transform.position = position;
        return obj.GetComponent<ParticleSystem>();
    }

    public void DestroyAll() {
        List<Entity> entities = new(_entities);
        foreach (Entity entity in entities) {
            Destroy(entity);
        }
    }
    
    public void Destroy<T>(T entity) where T : Entity {
        if (entity == null) return;
        _entities.Remove(entity);
        if (entity.gameObject == null || !entity.gameObject.activeSelf) return;
        entity.OnRelease();
        if (!Main.Pool.Push(entity.gameObject)) {
            Object.Destroy(entity.gameObject);
        }
    }

    #endregion
    
}