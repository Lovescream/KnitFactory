using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EUI_Panel_BundlePanel : UI_Panel {
    
    #region Fields
    
    private EUI_Bundle _selectedBundle;

    private readonly List<EUI_KnitCount> _knitCounts = new();
    private readonly List<EUI_BundleKnitButton> _knitButtons = new();
    private readonly List<EUI_Bundle> _bundles = new();
    private Dictionary<ColorType, int> _currentKnitCounts = new();

    private EditorScene _scene;
    private Transform _knitCountParent;
    private Transform _knitButtonParent;
    private Transform _bundleParent;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _knitCountParent = this.gameObject.FindChild<Transform>("KnitCounts");
        _knitButtonParent = this.gameObject.FindChild<Transform>("BundleKnitButtons");
        _bundleParent = this.gameObject.FindChild<Transform>("Content");
        this.gameObject.FindChild<UI_Button>("btnMoveLeft").SetEvent(OnButtonMoveLeft);
        this.gameObject.FindChild<UI_Button>("btnMoveRight").SetEvent(OnButtonMoveRight);
        this.gameObject.FindChild<UI_Button>("btnRemoveKnit").SetEvent(OnButtonRemoveKnit);
        this.gameObject.FindChild<UI_Button>("btnAdd").SetEvent(OnButtonAdd);
        this.gameObject.FindChild<UI_Button>("btnDelete").SetEvent(OnButtonDelete);
        
        return true;
    }

    public void Set() {
        Initialize();
        _scene = Main.Scene.Current as EditorScene;

        SetKnitButtons();
        SetBundles();
        ResetShowKnits(true);
        
        _scene?.SceneUI.SetPageButtons(true, true);
        EditorScene.EditorState = EditorState.BundlePanel;
        this.OnClosed += Apply;
    }
    
    public void ResetShowKnits(bool init = false){
        if (init) {
            _knitCountParent.DestroyAllChildren();
            _knitCounts.Clear();
        }

        RecalculateCounts();
        List<ColorType> riko = _currentKnitCounts.Keys.ToList();
        IReadOnlyDictionary<ColorType, int> counts = _scene.BeltBoard.RequiredKnitCounts;
        List<EUI_KnitCount> buki = new(_knitCounts);
        foreach (KeyValuePair<ColorType, int> pair in counts) {
            riko.Remove(pair.Key);
            EUI_KnitCount knitCount = _knitCounts.FirstOrDefault(c => c.Color == pair.Key);
            int current = _currentKnitCounts.GetValueOrDefault(pair.Key, 0);
            if (knitCount == null) {
                EUI_KnitCount newCount = Main.UI.CreateComponent<EUI_KnitCount>(parent: _knitCountParent);
                newCount.Set(pair.Key, current, pair.Value);
                _knitCounts.Add(newCount);
            }
            else {
                knitCount.SetCount(current, pair.Value);
                buki.Remove(knitCount);
            }
        }

        foreach (ColorType color in riko) {
            EUI_KnitCount newCount = Main.UI.CreateComponent<EUI_KnitCount>(parent: _knitCountParent);
            newCount.Set(color, _currentKnitCounts[color], 0);
            _knitCounts.Add(newCount);
        }
        
        foreach (EUI_KnitCount knitCount in buki) {
            Main.UI.Destroy(knitCount);
            _knitCounts.Remove(knitCount);
        }
    }

    private void SetKnitButtons() {
        _knitButtonParent.DestroyAllChildren();
        _knitButtons.Clear();
        foreach (ColorType type in System.Enum.GetValues(typeof(ColorType)))
        {
            if(type == ColorType.None) continue;
            EUI_BundleKnitButton button = Main.UI.CreateComponent<EUI_BundleKnitButton>(parent: _knitButtonParent);
            button.Set(this, type);
            _knitButtons.Add(button);
        }
    }

    private void SetBundles() {
        _bundleParent.DestroyAllChildren();
        _bundles.Clear();
        
        IReadOnlyList<BundleData> dataList = _scene?.BundleDataList;
        if (dataList == null) return;
        for (int i = 0; i < dataList.Count; i++) {
            EUI_Bundle bundle = Main.UI.CreateComponent<EUI_Bundle>(parent: _bundleParent);
            bundle.Set(this, dataList[i], i);
            _bundles.Add(bundle);
        }
    }

    #endregion
    
    #region Events

    private void OnButtonMoveLeft() {
        if (_selectedBundle == null) return;
        int index = _bundles.IndexOf(_selectedBundle);
        if (index <= 0) return;
        _bundles.Remove(_selectedBundle);
        _bundles.Insert(index - 1, _selectedBundle);
        for (int i = 0; i < _bundles.Count; i++) {
            EUI_Bundle bundle = _bundles[i];
            bundle.SetIndex(i, bundle == _selectedBundle);
        }
    }

    private void OnButtonMoveRight() {
        if (_selectedBundle == null) return;
        int index = _bundles.IndexOf(_selectedBundle);
        if (index >= _bundles.Count - 1) return;
        _bundles.Remove(_selectedBundle);
        _bundles.Insert(index + 1, _selectedBundle);
        for (int i = 0; i < _bundles.Count; i++) {
            EUI_Bundle bundle = _bundles[i];
            bundle.SetIndex(i, bundle == _selectedBundle);
        }
    }

    public void RemoveKnit(EUI_Bundle bundle, List<EUI_Knit> knits)
    {
        bundle.RemoveKnit(knits);
        ResetShowKnits();
    }

    private void OnButtonRemoveKnit() {
        _selectedBundle?.RemoveKnit();
        ResetShowKnits();
    }

    private void OnButtonAdd() {
        EUI_Bundle bundle = Main.UI.CreateComponent<EUI_Bundle>(parent: _bundleParent);
        bundle.Set(this, new BundleData { Knits = new() }, _bundles.Count);
        _bundles.Add(bundle);
        ResetShowKnits();
    }

    private void OnButtonDelete() {
        if (_selectedBundle == null) return;
        _bundles.Remove(_selectedBundle);
        for (int i = 0; i < _bundles.Count; i++) _bundles[i].SetIndex(i);
        Main.UI.Destroy(_selectedBundle);
        _selectedBundle = null;
        ResetShowKnits();
    }

    #endregion

    public void NewKnit(ColorType color) {
        _selectedBundle?.NewKnit(new KnitData { Color = color });
        ResetShowKnits();
    }

    public void SelectBundle(EUI_Bundle bundle) {
        _selectedBundle?.OnDeselect();
        _selectedBundle = bundle;
        _selectedBundle?.OnSelect();
    }

    public void Apply() {
        List<BundleData> dataList = _bundles.Select(b => b.Data).ToList();
        (Main.Scene.Current as EditorScene)?.ApplyBundles(dataList);
    }

    private void RecalculateCounts() {
        _currentKnitCounts = _bundles.SelectMany(b => b.Data.Knits)
            .GroupBy(k => k.Color)
            .ToDictionary(g => g.Key, g => g.Count());
    }

}