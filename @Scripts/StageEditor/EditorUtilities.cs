using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleFileBrowser;
using UnityEngine;
using Random = System.Random;

public static class EditorUtilities {

    #region Save / Load

    public static void Save(Action<bool> cbResult = null) {
        if (!CanSave()) return;

        StageDataKey stageDataKey = GetStageData();

        string json = JsonConvert.SerializeObject(stageDataKey, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new IgnoreVector2IntPropsResolver()
        });
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        OpenSaveDialogue(bytes, cbResult);
    }

    public static void Load(Action<StageDataKey> cbOnLoaded) {
        OpenLoadDialogue(cbOnLoaded, null);
    }

    #endregion

    #region Validate

    private static bool CanSave() {
        StageDataKey stageDataKey = EditorScene.CurrentStage;
        if (stageDataKey == null) return false;
        if (stageDataKey.Index <= 0) return false;
        if (stageDataKey.MaxKnitsOnBelt <= 0) return false;
        
        return true;
    }

    #endregion

    #region Parse

    public static StageDataKey GetStageData() {
        EditorScene scene = Main.Scene.Current as EditorScene;
        if (scene == null) return null;
        scene.ApplyBeltBoard();
        return EditorScene.CurrentStage;
    }

    #endregion

    #region Dialogue

    private static void OpenSaveDialogue(byte[] jsonBytes, Action<bool> cbResult) {
        FileBrowser.ShowSaveDialog(paths => {
                if (paths.Length > 0) {
                    string selectedPath = paths[0];
                    if (!selectedPath.EndsWith(".json")) selectedPath += ".json";
                    try {
                        FileBrowserHelpers.WriteBytesToFile(selectedPath, jsonBytes);
                        cbResult?.Invoke(true);
                    }
                    catch (Exception e) {
                        Debug.LogError($"Failed to save stage data: {e}");
                        cbResult?.Invoke(false);
                    }
                }
                else {
                    cbResult?.Invoke(false);
                }
            },
            () => {
                cbResult?.Invoke(false);
            },
            FileBrowser.PickMode.Files, false, null, $"{EditorScene.CurrentStage.Index}.json",
            "쿠크다스", "Save");
    }

    private static void OpenLoadDialogue(Action<StageDataKey> cbOnLoaded, Action<bool> cbResult) {
        FileBrowser.ShowLoadDialog(paths => {
                if (paths.Length > 0) {
                    try {
                        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(paths[0]);
                        string json = Encoding.UTF8.GetString(bytes);
                        StageDataKey stageDataKey = JsonConvert.DeserializeObject<StageDataKey>(json, new JsonSerializerSettings {
                            NullValueHandling = NullValueHandling.Ignore,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                        cbOnLoaded?.Invoke(stageDataKey);
                        cbResult?.Invoke(true);
                    }
                    catch (Exception e) {
                        Debug.LogError($"Failed to load stage data: {e}");
                        cbResult?.Invoke(false);
                    }
                }
                else {
                    cbResult?.Invoke(false);
                }
            },
            () => {
                cbResult?.Invoke(false);
            },
            FileBrowser.PickMode.Files, false, null, null, "초코칩쿠키");
    }

    private class IgnoreVector2IntPropsResolver : DefaultContractResolver {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            if (type == typeof(Vector2Int)) {
                props = props.Where(p => p.PropertyName is not ("magnitude" or "sqrMagnitude")).ToList();
            }

            return props;
        }
    }

    #endregion
    
}