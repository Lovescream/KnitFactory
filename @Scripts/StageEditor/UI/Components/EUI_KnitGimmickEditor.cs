using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EUI_KnitGimmickEditor : UI_Image
{
    #region Properties

    public List<EUI_Knit> DataList { get; private set; }

    #endregion

    #region Fields

    // Collections.
    private readonly Dictionary<KnitGimmickData, EUI_KnitGimmickElement> _elements = new();

    // Components.
    private Transform _elementParent;

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _elementParent = this.gameObject.FindChild<Transform>("Content");
        this.gameObject.FindChild<UI_Button>("btnAdd").SetEvent(OnButtonAdd);

        return true;
    }

    public void Set(List<EUI_Knit> data)
    {
        Initialize();
        DataList = data;

        _elementParent.DestroyAllChildren();
        _elements.Clear();
        
        foreach (KnitGimmickData gimmickData in DataList[0].Data.Gimmicks)
        {
            EUI_KnitGimmickElement element = Main.UI.CreateComponent<EUI_KnitGimmickElement>(_elementParent);
            element.Set(this, gimmickData);
            _elements[gimmickData] = element;
        }
    }

    #endregion

    #region Events

    private void OnButtonAdd()
    {
        NewElement(KnitGimmickType.None);
    }

    #endregion

    // 원본 그대로 기믹 리스트를 받는 메서드
    public List<KnitGimmickData> GetGimmickOriginDataList()
    {
        return _elements.Keys.ToList();
    }
    
    // 원보에서 쓸대없는 기믹은 제외한 리스틀 받는 메서드
    public List<KnitGimmickData> GetGimmickRemoveDataList()
    {
        return _elements.Keys
            .Where(x => x.Type != KnitGimmickType.Rope)
            .ToList();
    }

    private void NewElement(KnitGimmickType type)
    {
        KnitGimmickData newData = new() { Type = type };
        foreach (EUI_Knit data in DataList)
        {
            data.Data.Gimmicks.Add(newData);
        }
        EUI_KnitGimmickElement element = Main.UI.CreateComponent<EUI_KnitGimmickElement>(_elementParent);
        element.Set(this, newData);
        _elements[newData] = element;
    }

    public void Delete(KnitGimmickData data)
    {
        foreach (EUI_Knit knitData in DataList)
        {
            knitData.Data.Gimmicks.Remove(data);
        }
        Main.UI.Destroy(_elements[data]);
        _elements.Remove(data);
    }
}