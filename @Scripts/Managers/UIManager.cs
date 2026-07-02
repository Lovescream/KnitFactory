using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : ContentManager {

    #region Const

    private static readonly int INIT_ORDER_PANEL = 10;
    private static readonly int INIT_ORDER_POPUP = 100;

    #endregion

    #region Properties

    public bool UIOpened => _panels.Count > 0 || _popups.Count > 0;
    
    public Transform Root {
        get {
            if (_root == null) {
                GameObject obj = GameObject.Find("@UI_Root");
                if (obj == null) obj = new GameObject("@UI_Root");
                _root = obj.transform;
            }

            return _root;
        }
    }

    public Canvas OverlayCanvas
    {
        get
        {
            if (_overlayCanvas == null)
            {
                _overlayCanvas = GameObject.Find("UI_Overlay").GetComponent<Canvas>();
            }
            return _overlayCanvas;
        }
    }

    #endregion

    #region Fields

    private int _panelOrder = INIT_ORDER_PANEL;
    private int _popupOrder = INIT_ORDER_POPUP;

    private readonly List<UI_Panel> _panels = new();
    private readonly List<UI_Popup> _popups = new();

    private Transform _root;
    private Canvas _overlayCanvas;

    #endregion

    #region Generals

    public override void Clear() {
        base.Clear();
        CloseAllPanels();
        CloseAllPopups();
    }

    public T Instantiate<T>(bool pooling = true) where T : UI {
        GameObject prefab = Main.Resource.Get<GameObject>(typeof(T).Name);
        if (prefab == null) {
            Debug.LogError($"[UIManager] Instantiate<{typeof(T).Name}>(): Failed to load prefab.");
            return null;
        }

        T ui;
        if (pooling) ui = Main.Pool.Pop(prefab).GetOrAddComponent<T>();
        else {
            GameObject obj = GameObject.Instantiate(prefab, Root);
            obj.name = prefab.name;
            ui = obj.GetOrAddComponent<T>();
        }

        return ui;
    }

    public void Destroy(UI obj) {
        if (obj == null || obj.gameObject == null || !obj.gameObject.activeSelf) return;
        obj.OnRelease();
        if (Main.Pool.Push(obj.gameObject)) return;
        Object.Destroy(obj.gameObject);
    }

    #endregion

    #region SceneUI

    public T OpenSceneUI<T>() where T : UI_Scene {
        return Instantiate<T>(false);
    }

    #endregion

    #region Canvas

    public T OpenCanvas<T>() where T : UI_Canvas {
        return Instantiate<T>(false);
    }

    #endregion

    #region PanelUI

    public T GetPanel<T>() where T : UI_Panel {
        foreach (UI_Panel panel in _panels) if (panel is T p) return p;
        return null;
    }

    public T OpenPanel<T>() where T : UI_Panel {
        foreach (UI_Panel openedPanel in _panels) {
            if (openedPanel is not T thisPanel) continue;
            if (thisPanel.AllowDuplicate) break;
            thisPanel.SetPanelToFront();
            return thisPanel;
        }
        UI_Panel panel = Instantiate<T>();
        if (panel == null) return null;
        _panels.Add(panel);
        panel.SetOrder(_panelOrder++);
        GameEvents.OnGamePause?.Invoke();
        return panel as T;
    }
    
    public void ClosePanel(UI_Panel panel) {
        if (_panels.Count <= 0) return;
        bool isLatest = _panels[^1] == panel;
        _panels.Remove(panel);
        Destroy(panel);
        if (isLatest) _panelOrder--;
        else ReorderAllPanels();
        if (!UIOpened) GameEvents.OnGameResume?.Invoke();
    }
    
    public void ClosePanels<T>() where T : UI_Panel {
        _panels.Where(x => x is T).ToList().ForEach(ClosePanel);
    }
    
    public void CloseAllPanels() {
        if (_panels.Count == 0) return;
        for (int i = _panels.Count - 1; i >= 0; i--) {
            if (_panels[i] == null) continue;
            Destroy(_panels[i]);
        }

        _panels.Clear();
        _panelOrder = INIT_ORDER_PANEL;
    }

    public void ReorderAllPanels() {
        _panelOrder = INIT_ORDER_PANEL;
        _panels.ForEach(x=>x.SetOrder(_panelOrder++));
    }

    public void SetPanelToFront(UI_Panel panel) {
        if (!_panels.Remove(panel)) return;
        _panels.Add(panel);
        ReorderAllPanels();
    }

    public UI_Panel GetLatestPanel() => _panels.Count == 0 ? null : _panels[^1];

    #endregion

    #region PopopUI

    public T GetPopup<T>() where T : UI_Popup {
        foreach (UI_Popup popup in _popups) if (popup is T p) return p;
        UI_Popup newPopup = Instantiate<T>();
        if (newPopup == null)
        {
            Debug.LogError($"popup is null T = {typeof(T).Name}");
            return null;
        }
        _popups.Add(newPopup);
        newPopup.SetOrder(_popupOrder++);
        return newPopup as T;
    }

    public T OpenPopup<T>() where T : UI_Popup {
        foreach (UI_Popup openedPopup in _popups) {
            if (openedPopup is not T thisPopup) continue;
            if (thisPopup.AllowDuplicate) break;
            thisPopup.SetPopupToFront();
            return thisPopup;
        }
        UI_Popup popup = Instantiate<T>();
        if (popup == null) return null;
        _popups.Add(popup);
        popup.SetOrder(_popupOrder++);
        GameEvents.OnGamePause?.Invoke();
        return popup as T;
    }

    public T UsePopup<T>() where T : UI_Popup
    {
        foreach (UI_Popup popup in _popups)if (popup is T p) return p;
        return null;
    }

    public void ClosePopup(UI_Popup popup) {
        if (_popups.Count <= 0) return;
        bool isLatest = _popups[^1] == popup;
        _popups.Remove(popup);
        Destroy(popup);
        if (isLatest) _popupOrder--;
        else ReorderAllPanels();
        if (!UIOpened) GameEvents.OnGameResume?.Invoke();
    }

    public void ClosePopups<T>() where T : UI_Popup {
        _popups.Where(x=>x is T).ToList().ForEach(ClosePopup);
    }

    public void CloseAllPopups() {
        if (_popups.Count == 0) return;
        for (int i = _popups.Count - 1; i >= 0; i--) {
            if (_popups[i] == null) continue;
            Destroy(_popups[i]);
        }

        _popups.Clear();
        _popupOrder = INIT_ORDER_POPUP;
    }

    public void ReorderAllPopups() {
        _popupOrder = INIT_ORDER_POPUP;
        _popups.ForEach(x=>x.SetOrder(_popupOrder++));
    }

    public void CloseAllUI()
    {
        CloseAllPopups();
        CloseAllPanels();
    }

    public void SetPopupToFront(UI_Popup popup) {
        if (!_popups.Remove(popup)) return;
        _popups.Add(popup);
        ReorderAllPopups();
    }
    
    public UI_Popup GetLatestPopup() => _popups.Count == 0 ? null : _popups[^1];

    #endregion

    #region Components

    public T CreateComponent<T>(Transform parent = null, bool pooling = true) where T : UI {
        T item = Instantiate<T>(pooling);
        item.transform.SetParent(parent);
        return item;
    }

    #endregion

}