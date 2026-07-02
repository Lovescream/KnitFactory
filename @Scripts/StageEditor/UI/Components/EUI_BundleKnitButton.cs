using UnityEngine;

public class EUI_BundleKnitButton : UI_Button {

    #region Fields

    private ColorType _color;

    private EUI_Panel_BundlePanel _panel;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        this.SetEvent(OnButton);
        
        return true;
    }

    public void Set(EUI_Panel_BundlePanel panel, ColorType color) {
        Initialize();
        _panel = panel;
        _color = color;

        this.Sprite = Main.Resource.Get<Sprite>($"Knit_{_color}");
        this.transform.localScale = Vector3.one;
    }

    #endregion
    
    #region Events

    private void OnButton() {
        _panel.NewKnit(_color);
    }

    #endregion
    
}