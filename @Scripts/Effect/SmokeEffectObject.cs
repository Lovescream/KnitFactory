using UnityEngine;

public class SmokeEffectObject : Entity
{
    #region Const

    private const float LongValue = 1.2f;
    private const float ShortValue = 0.75f;
    private const float NarrowWidthValue = 0.1f;
    private const float StartPosY = 0.1f;

    #endregion
    
    #region Properties

    public SmokeEffect SmokeEffect { get; private set; }

    #endregion
    
    #region Fields
    
    private ParticleSystem _particleSystem;
    
    #endregion
    public override bool Initialize()
    {
        if (!base.Initialize()) return false;
        if (!TryGetComponent(out _particleSystem))
        {
            Debug.LogError($"not fount ParticleSystem");
        }
        return true;
    }

    public void Set(SmokeEffect effect)
    {
        Initialize();
        _particleSystem.Stop();
        SmokeEffect = effect;
        this.transform.SetParent(SmokeEffect.Box.Object.transform);
        this.transform.localPosition = Vector3.up * StartPosY;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        Orientation orientation = effect.Box.Orientation;
        Vector3 scale = Vector3.one;
        if (orientation == Orientation.Horizontal)
        {
            scale.x *= LongValue - NarrowWidthValue;
            scale.y *= ShortValue;
        }
        else
        {
            scale.x *= ShortValue - NarrowWidthValue;
            scale.y *= LongValue;
        }
        var shape = _particleSystem.shape;
        shape.scale = scale;
    }

    public void PlayEffect()
    {
        Initialize();
        _particleSystem.Play();
    }

    public void Destroy()
    {
        _particleSystem.Stop();
        Main.Object.Destroy(this);
    }
}