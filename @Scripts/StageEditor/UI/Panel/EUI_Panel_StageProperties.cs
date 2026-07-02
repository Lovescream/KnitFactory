using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EUI_Panel_StageProperties : UI_Panel {

    #region Fields

    // Components.
    private EditorScene _scene;
    private TMP_InputField _fieldIndex;
    private TMP_Dropdown _dropdownDifficulty;
    private TMP_InputField _fieldMaxKnitsOnBelt;
    private TMP_InputField _fieldMaxKnitsOnDock;
    private UI_Button _btnGenerate;
    private UI_Button _btnLoad;
    private UI_Button _btnClear;

    #endregion

    #region MonoBehaviours

    protected override void Update() {
        if (Keyboard.current.tabKey.wasPressedThisFrame) {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current == _fieldIndex.gameObject) {
                _fieldMaxKnitsOnBelt.Select();
                _fieldMaxKnitsOnBelt.ActivateInputField();
            }
            else if (current == _fieldMaxKnitsOnBelt.gameObject) {
                _fieldIndex.Select();
                _fieldIndex.ActivateInputField();
            }
        }
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _fieldIndex = this.gameObject.FindChild<TMP_InputField>("fieldIndex");
        _dropdownDifficulty = this.gameObject.FindChild<TMP_Dropdown>("dropdownDifficulty");
        _fieldMaxKnitsOnBelt = this.gameObject.FindChild<TMP_InputField>("fieldMaxKnitsOnBelt");
        _fieldMaxKnitsOnDock = this.gameObject.FindChild<TMP_InputField>("fieldMaxKnitsOnDock");
        _btnGenerate = this.gameObject.FindChild<UI_Button>("btnGenerate").SetEvent(OnButtonGenerate);
        _btnLoad = this.gameObject.FindChild<UI_Button>("btnLoad").SetEvent(OnButtonLoad);
        _btnClear = this.gameObject.FindChild<UI_Button>("btnClear").SetEvent(OnButtonClear);

        return true;
    }

    public void Set(bool showGenerate) {
        Initialize();
        _scene = Main.Scene.Current as EditorScene;
        _scene.SceneUI.SetPageButtons(false, !showGenerate);
        
        SetStageProperties();
        _btnGenerate.gameObject.SetActive(showGenerate);
        _btnLoad.gameObject.SetActive(Main.Board.Current == null);
        _btnClear.gameObject.SetActive(Main.Board.Current != null);

        EditorScene.EditorState = EditorState.StageProperties;
    }

    private void SetStageProperties() {
        StageDataKey dataKey = EditorScene.CurrentStage;
        _fieldIndex.text = $"{dataKey.Index}";
        _fieldMaxKnitsOnBelt.text = $"{dataKey.MaxKnitsOnBelt}";
        _fieldMaxKnitsOnDock.text = $"{dataKey.MaxKnitsOnDock}";
        _dropdownDifficulty.ClearOptions();
        List<string> options = new();
        for (int i = 0; i < System.Enum.GetNames(typeof(Difficulty)).Length; i++) {
            Difficulty difficulty = (Difficulty)i;
            options.Add($"{difficulty}");
        }
        _dropdownDifficulty.AddOptions(options);
        _dropdownDifficulty.value = (int)dataKey.Difficulty;
    }

    #endregion

    public void ApplyStageData() {
        StageDataKey dataKey = EditorScene.CurrentStage;

        int index = dataKey.Index;
        if (!int.TryParse(_fieldIndex.text, out int newIndex))
            Debug.LogError($"Stage의 Index는 반드시 숫자여야 합니다.");
        else index = newIndex;

        Difficulty difficulty = (Difficulty)_dropdownDifficulty.value;
        
        int maxKnitsOnBelt = int.Parse(_fieldMaxKnitsOnBelt.text);
        int maxKnitsOnDock = int.Parse(_fieldMaxKnitsOnDock.text);

        dataKey.Index = index;
        dataKey.Difficulty = difficulty;
        dataKey.MaxKnitsOnBelt = maxKnitsOnBelt;
        dataKey.MaxKnitsOnDock = maxKnitsOnDock;

        this.Close();
    }

    #region Events

    private void OnButtonGenerate() {
        ApplyStageData();

        _scene.GenerateNewBeltBoard();
        
        this.Close();
    }

    private void OnButtonLoad() {
        EditorUtilities.Load(data => {
            EditorScene.CurrentStage = data;
            (Main.Scene.Current as EditorScene)?.GenerateNewBeltBoard();
            this.Close();
        });
    }

    private void OnButtonClear() {
        Main.Scene.Reload();
    }
    
    #endregion

}