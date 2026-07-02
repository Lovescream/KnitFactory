using TMPro;
using UnityEngine;

public class EUI_InputField : UI
{
    #region Properties

    public string Text {
        get => _inputField.text;
        set {
            Initialize();
            _inputField.text = value;
        }
    }

    #endregion
    
    #region Fields

    private UI_Text _txtLabel;
    private TMP_InputField _inputField;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _txtLabel = this.gameObject.FindChild<UI_Text>("Txt_Label");
        _inputField = this.gameObject.FindChild<TMP_InputField>("Input_Field");

        return true;
    }

    public void Set(string label, string current) {
        Initialize();
        
        _txtLabel.Text = $"{label}";
        _inputField.text = current;
        if (_inputField.placeholder is TextMeshProUGUI t) t.text = $"{label}";
    }

    public void SetValue(string current) {
        _inputField.text = current;
    }

    #endregion

}
