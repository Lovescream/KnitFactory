using DG.Tweening;
using UnityEngine;

public class BoxGroupObject : BoxGimmickObject<BoxGroup>
{
    private const float FadeOutDuration = 0.2f;

    private Sequence _seq;
    private SpriteRenderer _renderer;
    
    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _renderer = gameObject.FindChild<SpriteRenderer>("Renderer");
        
        return true;
    }

    //짝을 가지는 두 박스가 서로 같은 기믹을 가지고 있게 만들고 나중에 생성된 것을 베이스로 실제 설정 값을 가지도록 함.
    public override void Set(BoxGroup gimmick)
    {
        base.Set(gimmick);
        if (Box.Queue.Board.BoxGroupObject.TryGetValue(Gimmick.Count, out BoxObject boxObject))
        {
            gameObject.SetActive(true);
            SetPosition(boxObject);
            SetSprite(false);
        } 
        else
        {
            Box.Queue.Board.BoxGroupObject[Gimmick.Count] = Box.Object;
            gameObject.SetActive(false);
        }
    }

    public void Destroy()
    {
        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.OnComplete(() => Main.Object.Destroy(this));
    }

    private void SetPosition(BoxObject obj)
    {
        Vector3 localPos = (obj.Box.Queue.Object.transform.position - Box.Queue.Object.transform.position) * 0.5f;
        if (Box.Orientation == Orientation.Horizontal) localPos.y -= 0.02f;
        transform.localPosition = localPos;
    }
    
    // 이제 해야 할 일이 리소스 위치 값을 자동으로 찾아줘야하는 기믹을 넣어야해서 상자가 가로인지 세로인지 확인부터 해야함.
    private void SetSprite(bool setLight)
    {
        string lightStr = setLight ? "On" : "Off";
        string dirStr = Box.Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical";
        Sprite sprite = Main.Resource.Get<Sprite>($"Box_Group_{dirStr}_{lightStr}");
        _renderer.sprite = sprite;
    }
}
