using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EUI_Dropdown : UI
{
    #region Properties

    public Type Type { get; private set; }

    public Enum Value {
        get {
            int index = _dropdown.value;
            if (index >= 0 && index < _values.Count) return _values[index];
            return null;
        }
    }
    
    #endregion
    
    #region Fields
    
    // Collections.
    private readonly List<Enum> _values = new();
    
    // Components.
    private UI_Text _txtLabel;
    private TMP_Dropdown _dropdown;
    
    // Events.
    public event Action<Enum> OnValueChanged;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _txtLabel = this.gameObject.FindChild<UI_Text>("Txt_Label");
        _dropdown = this.gameObject.FindChild<TMP_Dropdown>("Dropdown");

        return true;
    }

    public void Set(string label, Type enumType, Enum current, bool includeNone = false) {
        Initialize();
        Type = enumType;
        _txtLabel.Text = $"{label}";
        SetDropdown(current, includeNone);
    }

    public void SetValue(Enum current) {
        int selectedIndex = _values.IndexOf(current);
        _dropdown.value = selectedIndex >= 0 ? selectedIndex : 0;
    }

    private void SetDropdown(Enum current, bool includeNone = false) {
        _dropdown.ClearOptions();
        List<string> options = new();

        foreach (object value in Enum.GetValues(Type)) {
            int index = Convert.ToInt32(value);
            if (index == -1 && !includeNone) continue;

            if (index == -1) {
                _values.Insert(0, (Enum)value);
                options.Insert(0, $"{value}");
            }
            else {
                _values.Add((Enum)value);
                options.Add($"{value}");
            }
        }
        _dropdown.AddOptions(options);
        
        int selectedIndex = _values.IndexOf(current);
        _dropdown.value = selectedIndex >= 0 ? selectedIndex : 0;
        
        _dropdown.onValueChanged.RemoveAllListeners();
        _dropdown.onValueChanged.AddListener(i => {
            if (i >= 0 && i < _values.Count) OnValueChanged?.Invoke(_values[i]);
        });
    }

    #endregion
}
