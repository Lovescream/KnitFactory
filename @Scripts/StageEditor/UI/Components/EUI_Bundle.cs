using System.Collections.Generic;
using UnityEngine;

public class EUI_Bundle : UI_Image {

    #region Properties

    public BundleData Data { get; private set; }

    #endregion
    
    #region Fields

    private EUI_Panel_BundlePanel _panel;
    
    private readonly List<EUI_Knit> _knits = new();
    
    // Components.
    private UI_Text _txtIndex;
    private Transform _bundle;
    private UI_Button _btnSelect;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _txtIndex = this.gameObject.FindChild<UI_Text>("txtIndex");
        _bundle = this.gameObject.FindChild<Transform>("Bundle");
        _btnSelect = this.gameObject.FindChild<UI_Button>("btnSelect").SetEvent(OnButtonSelect);
        
        return true;
    }

    public void Set(EUI_Panel_BundlePanel panel, BundleData data, int index) {
        Initialize();
        _panel = panel;
        Data = data;
        
        _bundle.DestroyAllChildren();
        _knits.Clear();

        foreach (KnitData knitData in Data.Knits) {
            EUI_Knit knit = Main.UI.CreateComponent<EUI_Knit>(parent: _bundle);
            knit.Set(this, knitData);
            _knits.Add(knit);
        }
        
        SetIndex(index);
        this.transform.localScale = Vector3.one;
    }

    public void SetIndex(int index, bool oto = false) {
        _txtIndex.Text = $"{index}";
        if (oto) this.transform.SetSiblingIndex(index);
    }

    public void ResetShowKnits(bool init = false) => _panel.ResetShowKnits(init);

    //해당 털실과 묶일 수 있는 모든 털실을 반환
    public List<EUI_Knit> GetGroupKnits(EUI_Knit knit)
    {
        List<EUI_Knit> result = new();
        int index = _knits.IndexOf(knit);
        int startIndex = 0;
        ColorType color = knit.Color;

        for (int i = index; i >= 0; i--)
        {
            if (_knits[i].Color == color)
            {
                startIndex = i;
            }
            else break;
        }

        for (int i = startIndex; i < _knits.Count; i++)
        {
            if (_knits[i].Color == color)
            {
                result.Add(_knits[i]);
            }
            else break;
        }
        return result;
    }

    #endregion

    #region Select

    public void OnSelect() {
        _btnSelect.Image.color = new Color(152 / 255f, 200 / 255f, 120 / 255f, 1);
    }

    public void OnDeselect() {
        _btnSelect.Image.color = new(200 / 255f, 200 / 255f, 200 / 255f, 1);
    }

    #endregion

    #region Knits

    public void NewKnit(KnitData data) {
        EUI_Knit knit = Main.UI.CreateComponent<EUI_Knit>(parent: _bundle);
        knit.Set(this, data);
        _knits.Add(knit);
        Data.Knits.Add(knit.Data);
    }

    public void RemoveKnit() {
        if (_knits.Count == 0) return;
        EUI_Knit knit = _knits[^1];
        RemoveKnit(knit);
    }

    public void RemoveKnit(EUI_Knit knit)
    {
        Main.UI.Destroy(knit);
        _knits.Remove(knit);
        Data.Knits.Remove(knit.Data);
    }

    public void RemoveKnit(List<EUI_Knit> knits)
    {
        if (knits == null) return;
        if (knits.Count == 0) return;
        foreach (EUI_Knit knit in knits)
        {
            RemoveKnit(knit);
        }
    }

    #endregion
    
    #region Events

    private void OnButtonSelect() {
        _panel.SelectBundle(this);
    }
    
    #endregion

}