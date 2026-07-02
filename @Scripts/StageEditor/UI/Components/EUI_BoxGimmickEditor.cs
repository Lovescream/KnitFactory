using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EUI_BoxGimmickEditor : UI_Image
{
    #region Properties

    public BoxData Data { get; private set; }

    #endregion
    
    #region Fields

    // Collections.
    private readonly Dictionary<BoxGimmickData, EUI_BoxGimmickElement> _elements = new();
    
    // Components.
    private Transform _elementParent;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _elementParent = this.gameObject.FindChild<Transform>("Content");
        this.gameObject.FindChild<UI_Button>("btnAdd").SetEvent(OnButtonAdd);
        
        return true;
    }

    public void Set(BoxData data) {
        Initialize();
        Data = data;
        
        _elementParent.DestroyAllChildren();
        _elements.Clear();

        foreach (BoxGimmickData gimmickData in Data.Gimmicks) {
            EUI_BoxGimmickElement element = Main.UI.CreateComponent<EUI_BoxGimmickElement>(_elementParent);
            element.Set(this, gimmickData);
            _elements[gimmickData] = element;
        }
    }

    #endregion

    #region Events

    private void OnButtonAdd() {
        NewElement(BoxGimmickType.None);
    }
    
    #endregion

    public List<BoxGimmickData> GetGimmickDataList() {
        return _elements.Keys.ToList();
    }

    private void NewElement(BoxGimmickType type) {
        BoxGimmickData newData = new() { Type = type };
        Data.Gimmicks.Add(newData);
        EUI_BoxGimmickElement element = Main.UI.CreateComponent<EUI_BoxGimmickElement>(_elementParent);
        element.Set(this, newData);
        _elements[newData] = element;
    }

    public void Delete(BoxGimmickData data) {
        Data.Gimmicks.Remove(data);
        Main.UI.Destroy(_elements[data]);
        _elements.Remove(data);
    }
}
