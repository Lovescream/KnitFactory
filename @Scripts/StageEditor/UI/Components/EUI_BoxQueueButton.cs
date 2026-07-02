using UnityEngine;

public class EUI_BoxQueueButton : UI_Button {

    #region Fields
    
    private EUI_BoxQueueEditor _editor;
    private ColorType _color;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        this.SetEvent(OnButton);
        
        return true;
    }

    public void Set(EUI_BoxQueueEditor editor, ColorType color) {
        Initialize();
        _editor = editor;
        _color = color;
        this.Sprite = Main.Resource.Get<Sprite>($"Box_Vertical_{_color}_B");
        this.transform.localScale = Vector3.one;
    }

    #endregion

    #region Events

    private void OnButton() {
        _editor.NewElement(_color);
    }

    #endregion
    
}