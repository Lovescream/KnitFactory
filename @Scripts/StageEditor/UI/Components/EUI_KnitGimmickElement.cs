using UnityEngine;

public class EUI_KnitGimmickElement : UI
{
    #region Properties

    public EUI_KnitGimmickEditor Editor { get; set; }
    public KnitGimmickData Data { get; private set; }

    #endregion
    
    #region Fields

    private UI_Text _txtType;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _txtType = this.gameObject.FindChild<UI_Text>("txtType");
        this.gameObject.FindChild<UI_Button>("btnEdit").SetEvent(OnButtonEdit);
        this.gameObject.FindChild<UI_Button>("btnDelete").SetEvent(OnButtonDelete);
        
        return true;
    }

    public void Set(EUI_KnitGimmickEditor editor, KnitGimmickData data) {
        Initialize();
        this.transform.localScale = Vector3.one;
        Editor = editor;
        Data = data;
        SetTypeText();
    }

    public void SetTypeText() {
        _txtType.Text = $"{Data.Type}";
    }
    
    #endregion

    #region Events

    private void OnButtonEdit() {
        Main.UI.OpenPanel<EUI_Panel_KnitGimmickProperties>().Set(this);
    }

    private void OnButtonDelete() {
        Delete();
    }

    #endregion

    public void Delete() {
        Editor.Delete(Data);
        Main.UI.Destroy(this);
    }

}
