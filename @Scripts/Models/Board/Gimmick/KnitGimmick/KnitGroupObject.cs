using DG.Tweening;
using UnityEngine;

public class KnitGroupObject : KnitGimmickObject<KnitGroup>
{
    private const float FadeOutDuration = 0.2f;

    private Sequence _seq;
    private SpriteRenderer _studSprinter;
    private SpriteRenderer _ropeSprinter;
    
    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _studSprinter = gameObject.FindChild<SpriteRenderer>("Stud_F");
        _ropeSprinter = gameObject.FindChild<SpriteRenderer>("Stud_Rope");
        
        return true;
    }

    public override void Set(KnitGroup gimmick)
    {
        base.Set(gimmick);
        transform.localPosition = Vector3.zero;
    }

    public void Destroy()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.OnComplete(() => Main.Object.Destroy(this));
    }
}
