using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EUI_Panel_Confirm : UI_Panel {

    #region Fields

    private bool _saved;

    private TMP_InputField _fieldIndex;
    private TMP_Dropdown _dropdownDifficulty;
    private TMP_InputField _fieldMaxKnitsOnBelt;
    private TMP_InputField _fieldMaxKnitsOnDock;
    private UI_Button _btnSave;
    private UI_Button _btnClear;
    private UI_Button _btnPlayTest;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _fieldIndex = this.gameObject.FindChild<TMP_InputField>("fieldIndex");
        _dropdownDifficulty = this.gameObject.FindChild<TMP_Dropdown>("dropdownDifficulty");
        _fieldMaxKnitsOnBelt = this.gameObject.FindChild<TMP_InputField>("fieldMaxKnitsOnBelt");
        _fieldMaxKnitsOnDock = this.gameObject.FindChild<TMP_InputField>("fieldMaxKnitsOnDock");
        _btnSave = this.gameObject.FindChild<UI_Button>("btnSave").SetEvent(OnButtonSave);
        _btnClear = this.gameObject.FindChild<UI_Button>("btnClear").SetEvent(OnButtonClearEditor);
        _btnPlayTest = this.gameObject.FindChild<UI_Button>("btnPlayTest").SetEvent(OnButtonPlayTest);
        
        return true;
    }

    public void Set() {
        Initialize();

        EditorScene.EditorState = EditorState.ConfirmInfo;
        SetStageInfo();

        (Main.Scene.Current as EditorScene)?.SceneUI.SetPageButtons(true, false);
    }

    private void SetStageInfo() {
        StageDataKey dataKey = EditorScene.CurrentStage;
        
        List<string> options = new();
        _dropdownDifficulty.ClearOptions();
        for (int i = 0; i < System.Enum.GetNames(typeof(Difficulty)).Length; i++) {
            Difficulty difficulty = (Difficulty)i;
            options.Add($"{difficulty}");
        }
        _dropdownDifficulty.AddOptions(options);
        
        _fieldIndex.text = $"{dataKey.Index}";
        _fieldMaxKnitsOnBelt.text = $"{dataKey.MaxKnitsOnBelt}";
        _fieldMaxKnitsOnDock.text = $"{dataKey.MaxKnitsOnDock}";
        _dropdownDifficulty.value = (int)dataKey.Difficulty;

        _btnSave.gameObject.SetActive(true);
        _btnClear.gameObject.SetActive(_saved);
        _btnPlayTest.gameObject.SetActive(true);
    }

    private void SaveData()
    {
        StageDataKey dataKey = EditorScene.CurrentStage;
        dataKey.Index = int.Parse(_fieldIndex.text);
        dataKey.MaxKnitsOnBelt = int.Parse(_fieldMaxKnitsOnBelt.text);
        dataKey.MaxKnitsOnDock = int.Parse(_fieldMaxKnitsOnDock.text);
        dataKey.Difficulty = (Difficulty)_dropdownDifficulty.value;
    }

    #endregion

    #region Events

    private void OnButtonSave() {
        SaveData();
        EditorUtilities.Save(b => {
            if (b)
            {
                _saved = true;
                _btnClear.gameObject.SetActive(true);
                _btnPlayTest.gameObject.SetActive(true);
            }
        });
    }

    private void OnButtonClearEditor() {
        Main.Scene.Reload();
    }

    private void OnButtonPlayTest() {
        Main.IsEditorMode = true;
        Main.Data.EditorStageDataKey = EditorUtilities.GetStageData();
        Main.Scene.Load("GameScene");
    }

    #endregion

}