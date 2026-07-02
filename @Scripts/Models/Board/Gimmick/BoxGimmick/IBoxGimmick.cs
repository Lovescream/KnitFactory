using System;
using UnityEngine;

public interface IBoxGimmick
{
    BoxGimmickType Type { get; }
    Box Box { get; }
    BoxGimmickObject GimmickObject { get; }
    public BoxGimmickData GetData();
    public void GenerateNewObject();
    public void OnRemoved();
}

public abstract class BoxGimmickBase : IBoxGimmick
{
    public abstract BoxGimmickType Type { get; }
    public BoxGimmickObject GimmickObject { get; set; }
    public Box Box { get; }

    public event Action<BoxGimmickBase> OnGimmickRemoved;

    protected BoxGimmickBase(Box box)
    {
        Box = box;
    }
    
    public abstract BoxGimmickData GetData();
    public abstract void GenerateNewObject();

    public virtual void OnRemoved()
    {
        OnGimmickRemoved?.Invoke(this);
    }
}

public interface IBoxGimmickObject{}

public abstract class BoxGimmickObject : Entity, IBoxGimmickObject
{
    public abstract void Show(bool show);
}

public abstract class BoxGimmickObject<T> : BoxGimmickObject where T : IBoxGimmick
{
    public T Gimmick { get; private set; }
    public Box Box => Gimmick?.Box;

    protected Transform _view;

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _view = gameObject.FindChild<Transform>("View");

        return true;
    }

    public virtual void Set(T gimmick)
    {
        Initialize();
        Gimmick = gimmick;
        transform.SetParent(Box.Object.transform, false);
        Show(true);
    }

    public override void Show(bool show)
    {
        _view.gameObject.SetActive(show);
    }
}

public interface IBoxGimmickInfo
{
    public bool Has(BoxGimmickType type);
    public bool TryGet<T>(out T gimmick) where T : IBoxGimmick;
}