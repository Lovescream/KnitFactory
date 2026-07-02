using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BundleBoard
{
    #region Const.

    private readonly Vector2 BundleSize = new(1, 2.5f);
    private const float Spacing = 0.4f;
    private const float SizeY = 2f;

    #endregion

    #region Properties

    public UI_GameScene UI_Scene { get; }
    public UI_BundleBoardObject Object { get; private set; }
    // 생성된 모든 Knit데이터를 반환
    public IReadOnlyList<Knit> OriginKnits => Bundles.SelectMany(x => x.OriginKnits).ToList();
    public List<Knit> GetSameTypesKnits(int knitStateTypes) => 
        OriginKnits.Where(knit => ((int)knit.State & knitStateTypes) != 0).ToList();
    public IReadOnlyList<Bundle> Bundles => _bundles;
    public IReadOnlyDictionary<int, List<Knit>> KnitsGroupGimmick => _knitsGroupGimmick;

    #endregion

    #region Fields

    private readonly List<Bundle> _bundles = new();
    private readonly Dictionary<int, List<Knit>> _knitsGroupGimmick = new();

    #endregion

    #region Indexer

    public Bundle this[int index] => (index >= _bundles.Count || index < 0) ? null : _bundles[index];
    public Bundle LeftBundle(Bundle bundle) => this[_bundles.IndexOf(bundle) - 1];
    public Bundle RightBundle(Bundle bundle) => this[_bundles.IndexOf(bundle) + 1];

    #endregion

    #region Constructor

    public BundleBoard(UI_GameScene uiScene, StageDataKey stageDataKey)
    {
        UI_Scene = uiScene;

        // #1. 데이터가 존재하는 번들 생성.
        foreach (BundleData bundleData in stageDataKey.Bundles)
        {
            _bundles.Add(new(this, bundleData));
        }
    }

    public void GenerateObject(Transform parent)
    {
        Object = Main.Object.Instantiate<UI_BundleBoardObject>();
        Object.Set(this, parent);
        foreach (Bundle bundle in _bundles) bundle.GenerateObject();
    }

    // 그룹 기믹에 해당 털실을 추가
    public void AddGroupGimmick(int count, Knit knit)
    {
        if (!_knitsGroupGimmick.ContainsKey(count)) _knitsGroupGimmick[count] = new();
        _knitsGroupGimmick[count].Add(knit);
    }

    public List<KnitArray> GroupAllBundlesKnits()
    {
        List<KnitArray> results = new();
        foreach (Bundle bundle in _bundles)
        {
            foreach (KnitArray array in bundle.KnitArrays.Values)
            {
                results.Add(array);
            }
        }

        return results;
    }

    public void DestroyAllKnitObjects()
    {
        foreach (Bundle bundle in _bundles) bundle.DestroyAllKnitObjects();
    }

    public void SpawnAllKnitObjects()
    {
        foreach (Bundle bundle in _bundles) bundle.SpawnKnitAll();
    }

    public void SetKnitArray()
    {
        foreach (Bundle bundle in _bundles) bundle.SetKnitArrays();
    }

    public void SetInKnitData()
    {
        foreach (Bundle bundle in _bundles) bundle.SetDataInKnitsForKnitArrays();
    }

    public void ResetKnitList()
    {
        foreach (Bundle bundle in _bundles) bundle.ResetList();
    }

    public void SetActiveBundleClick(bool active) 
    {
        foreach (Bundle bundle in _bundles) bundle.SetActiveBundleClick(active);
    }

    public List<List<Knit>> GetLastKnitGroup()
    {
        List<List<Knit>> allKnitGroups = new();
        foreach (Bundle bundle in _bundles)
        {
            allKnitGroups.Add(bundle.GetLastKnitGroup());
        }

        return allKnitGroups;
    }

    #endregion

    public float GetXPosition(Bundle bundle)
    {
        int index = _bundles.IndexOf(bundle);
        return (BundleSize.x + Spacing) * (index - (_bundles.Count - 1) / 2f);
    }
}