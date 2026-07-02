using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class BoxObject : Entity, IPointerClickHandler
{
    #region Const

    private const float CatMaskVerPosX = 0;
    private const float CatMaskVerPosY = 0.08f;
    private const float CatMaskVerScaleX = 0.6f;
    private const float CatMaskVerScaleY = 0.8f;
    
    private const float CatMaskHorPosX = 0;
    private const float CatMaskHorPosY = 0.1f;
    private const float CatMaskHorScaleX = 1.065f;
    private const float CatMaskHorScaleY = 0.448f;

    private const float CatMaskAddSize = 0.5f;

    #endregion
    
    
    #region Properties

    public Box Box { get; private set; }
    public SpriteMask SpriteMaskBack => _spriteMaskBack;
    public SpriteRenderer BackSpriter => _backSpriter;
    public SpriteRenderer FrontSpriter => _frontSpriter;
    private BoxAnimationData boxAnimData => Main.So.AnimationData.boxAnimation;
    private float BoxDestroyDuration => boxAnimData.boxDestroyDuration;
    private float BoxLidFadeInDuration => boxAnimData.boxLidFadeInDuration;
    private float BoxLidCloseDownDuration => boxAnimData.boxLidCloseDownDuration;
    private float BoxLidCloseUpDuration => boxAnimData.boxLidCloseUpDuration;
    private float BoxLidStartY => boxAnimData.boxLidStartY;
    private float BoxLidDownY => boxAnimData.boxLidDownY;
    private float BoxLidUpY => boxAnimData.boxLidUpY;
    private float BoxLidShakeDuration => boxAnimData.boxLidShakeDuration;
    private float BoxLidShakeStrength => boxAnimData.boxLidShakeStrength;
    private int BoxLidShakeCount => boxAnimData.boxLidShakeCount;
    private float BoxLidShakeRandomness => boxAnimData.boxLidShakeRandomness;
    private bool BoxLidShakeFadeout => boxAnimData.boxLidShakeFadeout;
    private float BoxLidOpenStrength => boxAnimData.boxLidOpenStrength;
    private float BoxLidOpenCorrectionFactor => boxAnimData.boxLidOpenCorrectionFactor;
    private float BoxLidOpenPositionY => boxAnimData.boxLidOpenPositionY;

    public int BackLayer
    {
        get => _backSpriter.sortingOrder;
        set
        {
            _backSpriter.sortingOrder = value;
            OnChangeBackLayer?.Invoke(value);
        }
    }
    public int FrontLayer
    {
        get => _frontSpriter.sortingOrder;
        set
        {
            _frontSpriter.sortingOrder = value;
            OnChangeFrontLayer?.Invoke(value);
        }
    }

    #endregion

    #region Fields

    private Sequence _sequence;

    private SpriteRenderer _backSpriter;
    private SpriteRenderer _frontSpriter;
    private SpriteRenderer _lidSprinter;

    private SpriteMask _spriteMaskBack;
    private SpriteMask _spriteMaskCat;
    private BoxCollider2D _boxCollider;

    #endregion

    #region Event

    public event Action<int> OnChangeBackLayer;
    public event Action<int> OnChangeFrontLayer;

    #endregion

    #region MonoBehaviours

    void OnDisable()
    {
        _sequence?.Kill();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameScene.InGameState != InGameState.Hammer) return;
        GameEvents.OnHammerItem?.Invoke(Box);
        Main.Screen.SetActiveRayCast2D(false);
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _backSpriter = gameObject.FindChild<SpriteRenderer>("Back");
        _boxCollider = gameObject.FindChild<BoxCollider2D>("Back");
        _spriteMaskBack = gameObject.FindChild<SpriteMask>("Back");
        _spriteMaskCat = gameObject.FindChild<SpriteMask>("CatMask");
        _frontSpriter = gameObject.FindChild<SpriteRenderer>("Front");
        _lidSprinter = gameObject.FindChild<SpriteRenderer>("Lid");

        return true;
    }

    public void Set(Box box)
    {
        Initialize();
        Box = box;

        transform.SetParent(Box.Queue.Object.transform);
        
        ResetSprite();
        ResetPosition();
        ResetRotation();
        ResetColor();
        ResetScale();
        ResetNextLayer();
        ResizeCatMask();

        SetGimmicks();
    }


    public override void OnRelease()
    {
        base.OnRelease();
        foreach (BoxSlot slot in Box.Slots) slot.Object.Destroy();
    }

    #endregion

    #region InQueue

    public void Spawn()
    {
        this.transform.SetParent(Box.Queue.Object.Next.transform, false);
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOScale(1, 0.25f)).SetEase(Ease.InOutBounce)
            .OnComplete(() => 
            { 
                Box.State = BoxState.Idle; 
                ResizeColliderToSprite();
                
            });
    }

    public void Pull()
    {
        this.transform.SetParent(Box.Queue.Object.Current.transform);
        ResetCurrentLayer();
        if (Box.TryGetGimmick(out BoxHidden boxHidden)) Box.RemoveGimmick(boxHidden);

        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector2.zero, 0.25f))
            .Join(this.transform.DOScale(1, 0.25f))
            .OnComplete(() =>
            {
                Box.State = BoxState.Idle;
                _spriteMaskBack.maskSource = SpriteMask.MaskSource.SupportedRenderers;
                Box.OnCompletedMove();
            });
    }

    #endregion


    public void Destroy(Action completeAction = null)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOScale(0, BoxDestroyDuration)).SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                Main.Object.Destroy(this);
                completeAction?.Invoke();
            });
    }

    #region AnimationSequence

    public Sequence KnitMergeSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.25f);
        foreach (BoxSlot slot in Box.Slots)
        {
            seq.Join(slot.Object.transform.DOLocalMove(Vector3.zero, 0.4f)).SetEase(Ease.InSine);
        }
        seq.AppendInterval(0.1f);
        foreach (BoxSlot slot in Box.Slots)
        {
            seq.Join(slot.Object.transform.DOScale(1.2f, 0.3f)).SetEase(Ease.InSine);
        }
        
        return seq;
    }
    
    public Sequence LidCloseSequence()
    {
        Sequence seq = DOTween.Sequence();
        //뚜겅 투명도 1로 만듦
        seq.Append(_lidSprinter.DOFade(1, BoxLidFadeInDuration));
        //공을 박스 가운데로 몰아넣음
        seq.JoinCallback(() => KnitMergeSequence().Play());
        //뚜겅 내림
        seq.Append(_lidSprinter.transform.DOLocalMoveY(0.07f, BoxLidCloseDownDuration));
        //뚜껑 올림
        seq.Append(_lidSprinter.transform.DOLocalMoveY(0.1f, BoxLidCloseUpDuration));

        return seq;
    }

    public Sequence LidShakeSequence(float shakeDuration)
    {
        Sequence seq = DOTween.Sequence();
        //뚜껑 흔듦
        seq.Append(transform.DOShakePosition
        (
            duration: shakeDuration,
            strength: BoxLidShakeStrength,
            vibrato: BoxLidShakeCount,
            randomness: BoxLidShakeRandomness,
            fadeOut: BoxLidShakeFadeout
        ));
        seq.AppendInterval(0.1f);
        return seq;
    }

    public Sequence LidOpenSequence(float openDuration)
    {
        Vector2 targetPos = GetTargetPos();
        float targetRotationZ = Utilities.GetTargetRotationZ(targetPos);
        
        Sequence seq = DOTween.Sequence();
        
        //살짝 올림
        seq.Append(_lidSprinter.transform.DOMoveY(BoxLidOpenPositionY, openDuration));
        //옆으로 밀침
        seq.Join(_lidSprinter.transform.DOLocalMove(targetPos, openDuration));
        seq.Join(_lidSprinter.transform
            .DOLocalRotate(new Vector3(0, 0, targetRotationZ), openDuration)
            .SetEase(Ease.OutSine));
        seq.Join(_lidSprinter.DOFade(0, openDuration * 0.8f).SetDelay(openDuration * 0.2f));

        return seq;

        Vector2 GetTargetPos()
        {
            Vector2 cameraPos = Main.Screen?.Camera?.transform.position ?? Vector2.zero;
            Vector2 boxPos = this.transform.position;
        
            Vector2 animPos = boxPos - cameraPos;

            int randX, randY;

            do
            {
                randX = Random.Range(-1, 2);
                randY = Random.Range(-1, 2);
            } while (randX == 0 && randY == 0);
        
            Vector2 resultPos = new Vector2(randX, randY).normalized * BoxLidOpenStrength;
            if (Box.Orientation == Orientation.Horizontal)
            {
                resultPos.x *= BoxLidOpenCorrectionFactor;
                if (animPos.y > 0)
                    resultPos.y = -Mathf.Abs(resultPos.y);
                else
                    resultPos.y = Mathf.Abs(resultPos.y);
            }
            else
            {
                resultPos.y *= BoxLidOpenCorrectionFactor;
                if (animPos.x > 0)
                    resultPos.x = -Mathf.Abs(resultPos.x);
                else
                    resultPos.x = Mathf.Abs(resultPos.x);
            }
            return resultPos;
        }
    }

    public Sequence BoxDestroyAnimSequence()
    {
        Sequence seq = DOTween.Sequence();
        
        seq.Join(Box.Object.transform.DOScale(0, 0.45f)).SetEase(Ease.OutQuad);
        
        return seq;
    }

    #endregion

    #region Reset

    private void ResizeColliderToSprite()
    {
        Vector2 spriteSize = _backSpriter.bounds.size;
        _boxCollider.size = spriteSize;
        _boxCollider.offset = Vector2.zero;
    }

    private void ResizeCatMask()
    {
        Vector3 setPos = Vector3.zero;
        Vector3 setScale = Vector3.one;
        if (Box.Orientation == Orientation.Horizontal)
        {
            setPos.x = CatMaskHorPosX;
            setPos.y = CatMaskHorPosY;
            setScale.x = CatMaskHorScaleX;
            setScale.y = CatMaskHorScaleY;
        }
        else
        {
            setPos.x = CatMaskVerPosX;
            setPos.y = CatMaskVerPosY;
            setScale.x = CatMaskVerScaleX;
            setScale.y = CatMaskVerScaleY;
        }
        _spriteMaskCat.transform.localPosition = setPos;
        _spriteMaskCat.transform.localScale = setScale;
    }

    private void ResetNextLayer()
    {
        BackLayer = Box.Queue.Direction == Direction.Top ? 20 : 10;
        FrontLayer = Box.Queue.Direction == Direction.Bottom ? 5 : 15;
        _lidSprinter.sortingOrder = 100;
    }

    private void ResetCurrentLayer()
    {
        BackLayer = 12;
        FrontLayer = 17;
    }

    private void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
        _lidSprinter.transform.localPosition = Vector3.up * BoxLidStartY;
    }

    private void ResetRotation()
    {
        _lidSprinter.transform.rotation = Quaternion.identity;
        transform.localRotation = Quaternion.identity;
    }

    private void ResetSprite()
    {
        _backSpriter.sprite = Main.Resource.Get<Sprite>($"Box_{Box.Orientation}_{Box.Color}_B");
        _frontSpriter.sprite = Main.Resource.Get<Sprite>($"Box_{Box.Orientation}_{Box.Color}_F");
        _lidSprinter.sprite = Main.Resource.Get<Sprite>($"Box_{Box.Orientation}_{Box.Color}_Lid");
    }

    private void ResetColor()
    {
        _lidSprinter.color = new Color(1, 1, 1, 0);
        _frontSpriter.color = new Color(1, 1, 1, 1);
        _backSpriter.color = new Color(1, 1, 1, 1);
    }

    private void ResetScale()
    {
        transform.localScale = Vector3.zero;
        _lidSprinter.transform.localScale = Vector3.one;
        _frontSpriter.transform.localScale = Vector3.one;
        _backSpriter.transform.localScale = Vector3.one;
    }

    private void SetGimmicks()
    {
        foreach (IBoxGimmick gimmick in Box.Gimmicks)
        {
            gimmick.GenerateNewObject();
        }
    }

    #endregion

    public Sequence AddSizeCatMaskSequence(float duration)
    {
        Sequence seq = DOTween.Sequence();
        
        seq.Join(_spriteMaskCat.transform.DOLocalMoveY(CatMaskAddSize, duration)).SetRelative(true);
        seq.Join(_spriteMaskCat.transform.DOScaleX(CatMaskAddSize * 2, duration)).SetRelative(true);
        seq.Join(_spriteMaskCat.transform.DOScaleY( CatMaskAddSize * 2, duration)).SetRelative(true);

        return seq;
    }
}