using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UI_BundleObject : Entity
{
    #region Properties

    public Bundle Bundle { get; private set; }
    public Transform TargetStartPos { get; private set; }
    public Transform TargetEndPos { get; private set; }

    #endregion

    #region Fields

    private Coroutine _coroutine;
    private Transform _queueEnter;
    private UI_Button _btnBundleClick;
    private GameScene _scene;

    #endregion

    #region MonoBehaviours

    void OnDisable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _scene.OnInGameProcessing -= SetTrueBundleClick;
        _scene.OnInGameStop -= SetFalseBundleClick;
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _queueEnter = gameObject.FindChild<Transform>("Bundle_Top");
        TargetStartPos = gameObject.FindChild<Transform>("TargetStart");
        TargetEndPos = gameObject.FindChild<Transform>("TargetEnd");
        _btnBundleClick = gameObject.GetComponent<UI_Button>().SetEvent(OutKnits);
        _scene = Main.Scene.Current as GameScene;
        
        _scene.OnInGameProcessing += SetTrueBundleClick;
        _scene.OnInGameStop += SetFalseBundleClick;

        return true;
    }

    public void Set(Bundle bundle)
    {
        Initialize();
        Bundle = bundle;

        this.transform.SetParent(Bundle.Board.Object.transform, false);
        this.transform.localPosition = new(Bundle.Board.GetXPosition(Bundle), 0);
    }

    public Transform GetQueueEnter()
    {
        Initialize();
        return _queueEnter;
    }

    #endregion

    #region Knits

    public void SpawnKnitAll()
    {
        if (Bundle.State != BundleState.Idle) return;
        Bundle.State = BundleState.Spawning;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CoSpawnKnits());
    }

    private IEnumerator CoSpawnKnits()
    {
        Knit knit = Bundle.SpawnKnitOnce();
        while (knit != null)
        {
            yield return new WaitForSeconds(0.05f);
            knit = Bundle.SpawnKnitOnce();
        }

        Bundle.State = BundleState.Idle;
    }
    
    private void SetTrueBundleClick() => SetActiveBundleClick(true);
    private void SetFalseBundleClick() => SetActiveBundleClick(false);

    public void SetActiveBundleClick(bool active)
    {
        _btnBundleClick.SetActive(active, false);
    }

    public void OutKnits()
    {
        if (Bundle.State != BundleState.Idle) return;
        if (Bundle.WaitingKnits.Count <= 0) return;
        Knit firstKnit = Bundle.WaitingKnits[0];
        if (firstKnit.Has(KnitGimmickType.Rope)) return;
        
        List<Knit> knits = Bundle.DequeueKnits();
        if (knits == null) return;
        Bundle.State = BundleState.BAUBAU;
        if (_coroutine != null) StopCoroutine(_coroutine);
        
        List<Knit> hiddenKnits = Bundle.TryGetHiddenKnits();
        foreach (Knit knit in hiddenKnits)
        {
            if (knit.TryGet(out KnitHidden knitHidden)) knitHidden.OnRemoved();
        }

        _coroutine = StartCoroutine(CoOutKnits(knits));
    }

    private IEnumerator CoOutKnits(List<Knit> knits)
    {
        for (int i = 0; i < knits.Count; i++)
        {
            knits[i].MoveToSsagae();
            yield return new WaitForSeconds(0.05f);
        }

        Bundle.Pull();
        if (Bundle.WaitingKnits.Count > 0 && Bundle.WaitingKnits[0].TryGet(out KnitGroup group))
        {
            group.TryRemoveRope(group.Count);
        }

        Bundle.State = BundleState.Idle;
        SpawnKnitAll();
    }

    #endregion
}