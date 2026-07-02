using UnityEngine;

public static class Extensions {
    
    #region Generals

    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component => Utilities.GetOrAddComponent<T>(obj);
    public static T FindChild<T>(this GameObject obj, string name = null) where T : Component => Utilities.FindChild<T>(obj, name);
    public static T FindChildDirect<T>(this GameObject obj, string name = null) where T : Component => Utilities.FindChildDirect<T>(obj, name);
    public static GameObject FindChild(this GameObject obj, string name = null) => Utilities.FindChild(obj, name);
    public static GameObject FindChildDirect(this GameObject obj, string name = null) => Utilities.FindChildDirect(obj, name);

    #endregion
    
    #region UI

    public static void SetPanelToFront(this UI_Panel panel) => Main.UI.SetPanelToFront(panel);
    public static void SetPopupToFront(this UI_Popup popup) => Main.UI.SetPopupToFront(popup);

    #endregion

    #region Vector3
    public static Vector3 With(
        this Vector3 v,
        float? x = null,
        float? y = null,
        float? z = null)
    {
        return new Vector3(
            x ?? v.x,
            y ?? v.y,
            z ?? v.z
        );
    }

    #endregion

}