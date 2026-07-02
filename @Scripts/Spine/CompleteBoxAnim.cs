using UnityEngine;

public class CompleteBoxAnim
{
    #region properties

    public CompleteBoxAnimObject Object { get; private set; }
    public Box Box { get; private set; }

    #endregion

    #region Constructor

    public CompleteBoxAnim(Box box)
    {
        Box = box;
    }

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<CompleteBoxAnimObject>();
        Object.Set(this);
    }
    
    public void StartAnimation() => Object?.StartAnimation();
    public void Destroy() => Object?.Destroy();

    #endregion
}
