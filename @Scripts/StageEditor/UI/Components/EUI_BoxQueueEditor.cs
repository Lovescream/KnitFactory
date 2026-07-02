using System.Collections.Generic;
using UnityEngine;

public class EUI_BoxQueueEditor : UI_Image {

    #region Properties

    public BoxQueueData Data { get; private set; }

    #endregion

    #region Fields

    private readonly List<EUI_BoxQueueButton> _buttons = new();
    private readonly Dictionary<BoxData, EUI_BoxQueueElement> _elements = new();
    private Transform _buttonParent;
    private Transform _elementParent;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _buttonParent = this.gameObject.FindChild<Transform>("BoxButtons");
        _elementParent = this.gameObject.FindChild<Transform>("Content");
        
        return true;
    }

    public void Set(BoxQueueData data) {
        Initialize();
        Data = data;

        _buttonParent.DestroyAllChildren();
        _buttons.Clear();
        _elementParent.DestroyAllChildren();
        _elements.Clear();

        foreach (ColorType type in System.Enum.GetValues(typeof(ColorType)))
        {
            if (type == ColorType.None) continue;
            EUI_BoxQueueButton button = Main.UI.CreateComponent<EUI_BoxQueueButton>(parent: _buttonParent);
            button.Set(this, type);
            _buttons.Add(button);
        }

        foreach (BoxData boxData in Data.Boxes) {
            EUI_BoxQueueElement element = Main.UI.CreateComponent<EUI_BoxQueueElement>(parent: _elementParent);
            element.Set(this, boxData);
            _elements[boxData] = element;
        }
    }

    #endregion

    public void NewElement(ColorType color) {
        BoxData newData = new BoxData { Color = color };
        Data.Boxes.Add(newData);
        EUI_BoxQueueElement element = Main.UI.CreateComponent<EUI_BoxQueueElement>(parent: _elementParent);
        element.Set(this, newData);
        _elements[newData] = element;
    }

    public void DeleteElement(BoxData data) {
        Data.Boxes.Remove(data);
        Main.UI.Destroy(_elements[data]);
        _elements.Remove(data);
    }

}