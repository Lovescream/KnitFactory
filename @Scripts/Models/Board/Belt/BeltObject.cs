using System;
using UnityEngine;

public class BeltObject : Entity {

    #region Properties

    public Belt Belt { get; private set; }
    public string ArrowKey { get; private set; }

    #endregion
    
    #region Fields

    private SpriteRenderer _beltSpriter;
    private SpriteRenderer _arrowSpriter;

    #endregion

    #region MonoBehaviours

    void Update() {
        // if (_arrowSpriter == null || string.IsNullOrEmpty(ArrowKey)) return;
        // int frame = (int)(Main.Time.Frame % 9);
        // _arrowSpriter.sprite = Main.Resource.Get<Sprite>($"Belt_Arrow_{ArrowKey}_{frame}");
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _beltSpriter = this.gameObject.FindChild<SpriteRenderer>("Belt");
        _arrowSpriter = this.gameObject.FindChild<SpriteRenderer>("Arrow");
        
        return true;
    }

    public void Set(Belt belt) {
        Initialize();
        Belt = belt;
        
        // #1. Transform 설정.
        this.transform.name = $"{Belt}";
        this.transform.SetParent(Belt.Board.Object.transform);
        this.transform.position = Belt.Position;
        this.transform.localScale = Vector3.one;

        Direction start = Belt.StartDirection;
        Direction end = Belt.EndDirection;
        bool isCurve = start.GetOpposite() != end;
        bool isCW = start.GetClockwise() == end;
        _beltSpriter.sprite = Main.Resource.Get<Sprite>($"Belt_{(isCurve ? "Curve" : "Straight")}");
        int r = 4 - (int)start;
        if (isCW) r -= 1;
        else if (!isCurve) r -= 3;
        _beltSpriter.transform.localRotation = Quaternion.Euler(0, 0, r * 90);
        if (isCurve) _arrowSpriter.transform.localPosition = new(-0.1f, 0.15f);
        r = 90;
        if (isCurve) r += isCW ? -135 : 45;
        _arrowSpriter.transform.localRotation = Quaternion.Euler(0, 0, r);
    }

    #endregion
}

