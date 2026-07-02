using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class EUI_BoxQueueElement : UI_Image, IPointerClickHandler {

    #region Properties

    public BoxData Data { get; private set; }
    public EUI_BoxQueueEditor BoxQueueEditor { get; private set; }

    #endregion

    #region Fields

    #endregion
    
    #region MonoBehaviours

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right) OnRightClick();
    }

    #endregion

    #region Initialize / Set

    public void Set(EUI_BoxQueueEditor editor, BoxData data) {
        Initialize();
        BoxQueueEditor = editor;
        Data = data;
        this.Sprite = Main.Resource.Get<Sprite>($"Box_Vertical_{Data.Color}_B");
        this.transform.localScale = Vector3.one;
    }

    public void ChangeAngle(ColorType color)
    {
        Data.Color = color;
        Sprite = Main.Resource.Get<Sprite>($"Box_Vertical_{Data.Color}_B");
    }

    public void SetGimmick(List<BoxGimmickData> gimmickDataList)
    {
        Data.Gimmicks = gimmickDataList;
    }

    #endregion

    #region Events

    private void OnLeftClick()
    {
        Main.UI.OpenPanel<EUI_Panel_BoxProperties>().Set(this);
    }

    private void OnRightClick() {
        BoxQueueEditor.DeleteElement(Data);
    }

    #endregion
    
}