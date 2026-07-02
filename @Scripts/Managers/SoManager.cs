using UnityEngine;

public class SoManager : CoreManager
{
    public AnimationData AnimationData { get; private set; }
    public MetaData MetaData { get; private set; }
    public SingularData SingularData { get; private set; }
    public AdsData AdsData { get; private set; }
    
    public override bool Initialize()
    {
        if(!base.Initialize()) return false;

        AnimationData = LoadData<AnimationData>();
        MetaData = LoadData<MetaData>();
        SingularData = LoadData<SingularData>();
        AdsData = LoadData<AdsData>();
        
        return true;
    }

    private T LoadData<T>() where T : ScriptableObject
    {
        T data = Resources.Load<T>($"{BlossomPath.RESOURCES_SO}/{typeof(T)}");
        if(data == null) Debug.LogError($"Can't find {typeof(T)}");
        return data;
    }
}
