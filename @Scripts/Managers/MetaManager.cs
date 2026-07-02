using UnityEngine;

public class MetaManager : ContentManager
{
    public RewordMeta RewordMeta { get; private set; }

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        RewordMeta = new RewordMeta();
        
        return true;
    }
}
