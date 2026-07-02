using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EUI_Knit : UI_Image, IPointerClickHandler {

    #region Properties

    public KnitData Data { get; private set; }
    public EUI_Bundle Bundle { get; private set; }
    public ColorType Color => Data.Color;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        
        
        return true;
    }

    public void Set(EUI_Bundle bundle, KnitData data) {
        Initialize();
        Bundle = bundle;
        Data = data;
        this.Sprite = Main.Resource.Get<Sprite>($"Knit_{Color}");
        this.transform.localScale = Vector3.one;
    }

    #endregion

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right) OnRightClick();
    }

    public void ChangeColor(ColorType color)
    {
        this.Sprite = Main.Resource.Get<Sprite>($"Knit_{color}");
        Data.Color = color;
    }

    public void SetGimmick(List<KnitGimmickData> gimmicks)
    {
        Data.Gimmicks = gimmicks;
    }

    private void OnLeftClick()
    {
        List<EUI_Knit> knits = Bundle.GetGroupKnits(this);
        Main.UI.OpenPanel<EUI_Panel_KnitProperties>().Set(Bundle, knits);
    }

    private void OnRightClick()
    {
        List<EUI_Knit> knits = new(){this};
        Main.UI.OpenPanel<EUI_Panel_KnitProperties>().Set(Bundle, knits);
    }
}