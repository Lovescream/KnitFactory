using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public class NewObjectEx {

    #region Properties

    private const string PATH = "Assets/@Resources/Prefabs/UIBase/UIBase.asset";
    private static Prefabs Prefabs => AssetDatabase.LoadAssetAtPath<Prefabs>(PATH);

    #endregion

    #region Generals

    private static void Instantiate(Func<Prefabs, GameObject> selector) {
        if (Prefabs == null) {
            Debug.LogWarning($"Prefabs not found at path {PATH}");
            return;
        }

        Object instance = PrefabUtility.InstantiatePrefab(selector(Prefabs), Selection.activeTransform);

        Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
        Selection.activeObject = instance;
    }

    #endregion

    #region Editor Menu

    [MenuItem("GameObject/>>>UI/Base/Image", false, 1)]
    private static void CreateNewImage() => Instantiate(Prefabs => Prefabs.Image);


    [MenuItem("GameObject/>>>UI/Base/Text", false, 1)]
    private static void CreateNewText() => Instantiate(Prefabs => Prefabs.Text);


    [MenuItem("GameObject/>>>UI/Base/Button", false, 1)]
    private static void CreateNewButton() => Instantiate(Prefabs => Prefabs.Button);
    
    
    [MenuItem("GameObject/>>>UI/Base/Toggle", false, 1)]
    private static void CreateNewToggle() => Instantiate(Prefabs => Prefabs.Toggle);

    #endregion

    #region Check Validate

    [MenuItem("GameObject/>>>UI/Base/Image", true)]
    [MenuItem("GameObject/>>>UI/Base/Text", true)]
    [MenuItem("GameObject/>>>UI/Base/Button", true)]
    [MenuItem("GameObject/>>>UI/Base/Toggle", true)]
    private static bool CreateNewUIComponentValidate() => Selection.activeGameObject && Selection.activeGameObject.GetComponentInParent<Canvas>();

    #endregion

}