using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : CoreManager {

    #region Properties

    public StageDataKey EditorStageDataKey { get; set; }

    #endregion
    
    #region Fields
    
    private Dictionary<int, TextAsset> _stageData = new();
    private Dictionary<Type, Dictionary<string, DataKey>> _data = new();
    private StageDataKey _currentStageDataKey;
    
    #endregion

    #region Initialize

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        LoadData();
        LoadStageData();
        PrefsInitialize();
        
        return true;
    }

    private void LoadData() {
        TextAsset[] dataFiles = Resources.LoadAll<TextAsset>($"{BlossomPath.RESOURCES_DATA}");
        foreach (TextAsset textAsset in dataFiles) {
            string typeName = textAsset.name.Split('-')[0];
            Type type = Type.GetType(typeName);
            if (type == null) {
                Debug.LogError($"[DataManager] Initialize(): The type({textAsset}) was not found.");
                continue;
            }

            if (!_data.ContainsKey(type)) _data[type] = new();

            IEnumerable<DataKey> dataList = typeof(JsonConvert)
                    .GetMethods()
                    .FirstOrDefault(m =>
                        m.Name == "DeserializeObject" && m.IsGenericMethod && m.GetParameters().Length == 1)
                    ?.MakeGenericMethod(typeof(List<>).MakeGenericType(type))
                    .Invoke(null, new object[] { textAsset.text })
                as IEnumerable<DataKey>;
            if (dataList == null) continue;
            
            Dictionary<string, DataKey> dataDictionary = dataList.ToDictionary(x => x.Key);
            foreach (KeyValuePair<string, DataKey> pair in dataDictionary) {
                _data[type][pair.Key] = pair.Value;
            }
        }
    }

    private StageDataKey CurrentStageDataKey()
    {
        int curLevel = PlayerData.ClearLevel.Value + 1;
        if (_currentStageDataKey != null && _currentStageDataKey.Index == curLevel)
        {
            return _currentStageDataKey;
        }
        
        _currentStageDataKey = GetStageData(curLevel) ?? GetStageData(PlayerData.ClearLevel.Value);
        return _currentStageDataKey;
    }
    
    public Difficulty CurrentDifficulty => CurrentStageDataKey().Difficulty;

    private void LoadStageData() {
        _stageData = Resources.LoadAll<TextAsset>($"{BlossomPath.RESOURCES_STAGEDATA}")
            .ToDictionary(x => int.Parse(x.name.Replace("Stage","")), x => x);
    }

    #endregion

    #region GetData
    
    public bool ContainsKey<T>(string key) where T : DataKey {
        if (!_data.TryGetValue(typeof(T), out Dictionary<string, DataKey> dictionary)) {
            Debug.LogError($"[DataManager] Get<{typeof(T)}>({key}): Failed to get data. Not found the type.");
            return false;
        }
        return dictionary.ContainsKey(key);
    }
    
    public T Get<T>(string key) where T : DataKey {
        if (!_data.TryGetValue(typeof(T), out Dictionary<string, DataKey> dictionary)) {
            Debug.LogError($"[DataManager] Get<{typeof(T)}>({key}): Failed to get data. Not found the type.");
            return null;
        }

        if (!dictionary.TryGetValue(key, out DataKey data)) {
            Debug.LogError($"[DataManager] Get<{typeof(T)}>({key}): Failed to get data. Not found the key.");
            return null;
        }

        return data as T;
    }

    public List<T> GetAll<T>() where T : DataKey {
        if (!_data.TryGetValue(typeof(T), out Dictionary<string, DataKey> dictionary)) {
            Debug.LogError($"[DataManager] GetAll<{typeof(T)}>(): Failed to get data. Not found the type.");
            return null;
        }

        return dictionary.Values.Select(x => x as T).ToList();
    }

    public StageDataKey GetStageData(int stage) {
        //if (stage > 20) return null;
        if (!_stageData.TryGetValue(stage, out TextAsset textAsset)) return null;
        try {
            JsonSerializerSettings settings = new();
            settings.Converters.Add(new Vector2IntDictionaryConverter());
            StageDataKey stageDataKey = JsonConvert.DeserializeObject<StageDataKey>(textAsset.text, settings);
            return stageDataKey;
        }
        catch (Exception e) {
            Debug.LogError($"[DataManager] GetStageData({stage}): Failed to deserialize stage data: {e.Message}");
            return null;
        }
    }
    
    #endregion

    #region Prefs

    // private LuckyPassPrefs _luckyPass;
    // private ProfilePrefs _profilePrefs;
    
    private void PrefsInitialize() {
        
        // _luckyPass = Prefs.Get<LuckyPassPrefs>();
        // _profilePrefs = Prefs.Get<ProfilePrefs>();
        //
        ItemDisplay.OnItemReceived += OnItemReceived;
    }
    
    private void OnItemReceived(CollectorItemType type, int amount) {
        switch (type) {
            // case CollectorItemType.Clock: _currency.Clock.DisplayValue += amount; break;
            // case CollectorItemType.Balloon: _currency.Balloon.DisplayValue += amount; break;
            // case CollectorItemType.Churu: _currency.Churu.DisplayValue += amount; break;
        }
    }
    
    public void PrefsSync() {
        // _currency.Clock.DisplayValueSync();
        // _currency.Balloon.DisplayValueSync();
        // _currency.Churu.DisplayValueSync();
        // _luckyPass.IsPurchased.DisplayValueSync();
        // _profilePrefs.ProfileAvatarType.DisplayValueSync();
        // _profilePrefs.ProfileFrameType.DisplayValueSync();
    }

    #endregion
}

public class DataKey {
    public string Key { get; set; }
}