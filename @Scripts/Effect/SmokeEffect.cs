using UnityEngine;

public class SmokeEffect
{
    public SmokeEffectObject Object { get; private set; }
    public Box Box { get; private set; }

    public SmokeEffect(Box box)
    {
        Box = box;
    }

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<SmokeEffectObject>();
        Object.Set(this);
    }
    
    public void PlayEffect() => Object?.PlayEffect();
    public void Destroy() => Object?.Destroy();
}
