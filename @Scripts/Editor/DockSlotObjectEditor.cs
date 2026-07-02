using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DockSlotObject))]
public class DockSlotObjectEditor : Editor {

    public override void OnInspectorGUI() {
        DockSlotObject slotObject = (DockSlotObject)target;
        DockSlot slot = slotObject.Slot;
        
        // 기본 Inspector 그리기.
        DrawDefaultInspector();
        
        // 구분선 추가.
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("====Debug Information====", EditorStyles.boldLabel);
        
        if (slot.Knit == null) EditorGUILayout.LabelField("슬?롯!이비@었어^요!!!!!!!!!!!!!!!!!!");
        else if (slot.Knit.Object == null) EditorGUILayout.LabelField("뭐야이건");
        else {
            EditorGUILayout.ObjectField("Slot", slot.Knit.Object, typeof(GameObject), true);
        }
        
        if (EditorApplication.isPlaying) Repaint();
    }
    
}