using System;
using UnityEngine;

public class KnitKeeperObject : Entity {
    
    #region Properties

    public KnitKeeper KnitKeeper { get; private set; }

    #endregion

    #region Fields

    private SpriteRenderer _sprinter;

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;
        
        _sprinter = gameObject.FindChild<SpriteRenderer>("sprite_KnitKeeper");
        
        return true;
    }

    public void Set(KnitKeeper knitKeeper) {
        Initialize();
        KnitKeeper = knitKeeper;
        transform.SetParent(KnitKeeper.Board.Object.transform);
        this.transform.position = knitKeeper.Position;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        SetSprite();
    }

    private void SetSprite()
    {
        Sprite sprite = Main.Resource.Get<Sprite>($"Knit_Keeper_{KnitKeeper.Data.Orientation}");
        _sprinter.sprite = sprite;
    }

    #endregion
    
}