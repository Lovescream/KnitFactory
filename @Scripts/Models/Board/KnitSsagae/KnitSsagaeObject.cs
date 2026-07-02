using System;
using UnityEngine;

public class KnitSsagaeObject : Entity {

    #region Const.

    public const float PoopInterval = 0.08f;

    #endregion
    
    #region Properties

    public KnitSsagae Ssagae { get; private set; }

    public float Timer {
        get => _timer;
        set {
            _timer = value;
            if (_timer <= 0) {
                Ssagae.Poop();
                _timer = PoopInterval;
            }
        }
    }

    #endregion

    #region Fields

    private float _timer;

    private UI_Text _txtCount;
    private FillAmountSprite _fillSprite;

    #endregion

    #region MonoBehaviours

    void Update() {
        if (Ssagae.HasKnit) Timer -= Time.deltaTime;
    }

    void OnDisable() {
        Main.Board.OnChangedKnitsOnBelt -= OnChangedKnitsOnBelt;
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _txtCount = this.gameObject.FindChild<UI_Text>("Text_Count");
        _fillSprite = this.gameObject.FindChild<FillAmountSprite>("Sprite_Gauge_Front");
        
        return true;
    }

    public void Set(KnitSsagae ssagae) {
        Initialize();
        Ssagae = ssagae;
        this.transform.SetParent(Ssagae.Board.Object.transform);
        this.transform.position = Ssagae.Position;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        Main.Board.OnChangedKnitsOnBelt -= OnChangedKnitsOnBelt;
        Main.Board.OnChangedKnitsOnBelt += OnChangedKnitsOnBelt;
        OnChangedKnitsOnBelt();
    }

    #endregion

    #region Events

    private void OnChangedKnitsOnBelt() {
        _txtCount.Text = $"{Main.Board.KnitsOnBelt.Count:D2}/{Main.Board.MaxKnitsOnBelt:D2}";
        float amountValue = (float)Main.Board.KnitsOnBelt.Count / Main.Board.MaxKnitsOnBelt;
        _fillSprite.SetAmount(amountValue);
    }

    #endregion
    
}