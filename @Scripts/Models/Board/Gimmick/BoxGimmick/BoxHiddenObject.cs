using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BoxHiddenObject : BoxGimmickObject<BoxHidden>
{
    private const float FadeOutDuration = 0.2f;
    private SpriteRenderer _front;
    private SpriteRenderer _back;

    private Sequence _seq;
    
    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _front = _view.gameObject.FindChild<SpriteRenderer>("Front_Hidden");
        _back = _view.gameObject.FindChild<SpriteRenderer>("Back_Hidden");
        
        return true;
    }

    public override void Set(BoxHidden gimmick)
    {
        base.Set(gimmick);
        
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Orientation orientation = Box.Orientation;
        _front.sprite = Main.Resource.Get<Sprite>($"Box_{orientation}_Hidden_F");
        _back.sprite = Main.Resource.Get<Sprite>($"Box_{orientation}_Hidden_B");
        
        ChangeBackLayer(Box.Object.BackLayer);
        ChangeFrontLayer(Box.Object.FrontLayer);

        Box.Object.OnChangeBackLayer += ChangeBackLayer;
        Box.Object.OnChangeFrontLayer += ChangeFrontLayer;
        
        _front.color = new Color(1, 1, 1, 1);
        _back.color = new Color(1, 1, 1, 1);
    }
    
    private void ChangeFrontLayer(int layer) => _front.sortingOrder = layer + 1;
    private void ChangeBackLayer(int layer) => _back.sortingOrder = layer + 1;

    public void Destroy()
    {
        Box.Object.OnChangeBackLayer -= ChangeBackLayer;
        Box.Object.OnChangeFrontLayer -= ChangeFrontLayer;
        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(_front.DOFade(0, FadeOutDuration));
        _seq.Join(_back.DOFade(0, FadeOutDuration));
        _seq.OnComplete(() => Main.Object.Destroy(this));
    }
}
