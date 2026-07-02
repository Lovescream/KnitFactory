using System;
using UnityEngine;

public interface IKnitGimmick
{
    KnitGimmickType Type { get; }
    Knit Knit { get; }
    KnitGimmickObject GimmickObject { get; }
    public KnitGimmickData GetData();
    public void GenerateNewObject();
    public void OnRemoved();
}

public abstract class KnitGimmickBase : IKnitGimmick
{
    public abstract KnitGimmickType Type { get; }
    public KnitGimmickObject GimmickObject { get; set; }
    public Knit Knit { get; }

    public event Action<KnitGimmickBase> OnGimmickRemoved;

    protected KnitGimmickBase(Knit knit)
    {
        Knit = knit;
    }
    
    public abstract KnitGimmickData GetData();
    public abstract void GenerateNewObject();

    public virtual void OnRemoved()
    {
        OnGimmickRemoved?.Invoke(this);
    }
}

public interface IKnitGimmickObject{}

public abstract class KnitGimmickObject : Entity, IKnitGimmickObject
{
    public abstract void Show(bool show);
}

public abstract class KnitGimmickObject<T> : KnitGimmickObject where T : IKnitGimmick
{
    public T Gimmick { get; private set; }
    public Knit Knit => Gimmick?.Knit;

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
        transform.SetParent(Knit.Object.transform, false);
        Show(true);
    }

    public override void Show(bool show)
    {
        _view.gameObject.SetActive(show);
    }
}

public interface IKnitGimmickInfo
{
    public bool Has(KnitGimmickType type);
    public bool TryGet<T>(out T gimmick) where T : IKnitGimmick;
}