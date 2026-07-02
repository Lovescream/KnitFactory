using System.Collections;
using DG.Tweening;
using UnityEngine;

public class KnitHiddenObject : KnitGimmickObject<KnitHidden>
{
    private const float FadeOutDuration = 0.2f;
    private SpriteRenderer _knitSpriter;

    private Sequence _seq;
    
    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _knitSpriter = _view.gameObject.FindChild<SpriteRenderer>("Knit_Hidden");
        
        return true;
    }

    public override void Set(KnitHidden gimmick)
    {
        base.Set(gimmick);
    }

    public void Destroy()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(_knitSpriter.DOFade(0, FadeOutDuration));
        _seq.OnComplete(() => Main.Object.Destroy(this));
    }
}
