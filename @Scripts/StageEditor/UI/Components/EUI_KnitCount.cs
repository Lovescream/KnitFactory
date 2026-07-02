using UnityEngine;

public class EUI_KnitCount : UI {

    #region Const.

    private readonly Color GoodColor = new Color(75 / 255f, 200 / 255f, 100 / 255f, 1);
    private readonly Color FuckColor = new Color(200 / 255f, 100 / 255f, 75 / 255f, 1); 

    #endregion
    
    #region Properties

    public ColorType Color { get; private set; }

    #endregion
    
    #region Fields

    private UI_Image _imgKnit;
    private UI_Text _txtCount;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _imgKnit = this.gameObject.FindChild<UI_Image>("imgKnit");
        _txtCount = this.gameObject.FindChild<UI_Text>("txtCount");
        
        return true;
    }

    public void Set(ColorType color, int current, int total) {
        Initialize();
        Color = color;
        _imgKnit.Sprite = Main.Resource.Get<Sprite>($"Knit_{Color}");
        SetCount(current, total);
        this.transform.localScale = Vector3.one;
    }

    public void SetCount(int current, int total) {
        _txtCount.SetColor(current == total ? GoodColor : FuckColor);
        _txtCount.Text = $"{current}/{total}";
    }

    #endregion

}