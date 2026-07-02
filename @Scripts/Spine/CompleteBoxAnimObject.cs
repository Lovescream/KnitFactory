using System.Linq;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class CompleteBoxAnimObject : Entity
{
    #region Consts

    private readonly string[] AnimKeys = { "F_Jump_01", "F_Jump_02", "F_Jump_03" };
    
    private const float CatScale = 0.7f;

    #endregion

    #region Properties
    public CompleteBoxAnim CompleteBoxAnim { get; private set; }

    #endregion

    #region Fields

    private static int _animIndex = 0;

    #endregion

    // TODO: 애니메이션 실행 개편 필요.
    
    private SkeletonAnimation _skeletonAnim;

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;
        if (!TryGetComponent(out _skeletonAnim))
        {
            Debug.LogError($"SkeletonAnimation not found");
            return false;
        }

        gameObject.SetActive(true);
        return true;
    }

    public void Set(CompleteBoxAnim completeBoxAnim)
    {
        Initialize();
        CompleteBoxAnim = completeBoxAnim;

        this.transform.SetParent(completeBoxAnim.Box.Object.transform);
        this.transform.localPosition = Vector3.up * -0.2f;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one * CatScale;
        _skeletonAnim.skeleton.SetColor(new Color(1, 1, 1, 0));
    }

    public void Destroy()
    {
        Main.Object.Destroy(this);
    }

    public void StartAnimation()
    {
        Sequence seq = DOTween.Sequence();

        // 덮개를 덮음.
        seq.Append(CompleteBoxAnim.Box.Object.LidCloseSequence());

        // 박스 안에 있는 모든 Knit을 파괴
        seq.AppendCallback(CompleteBoxAnim.Box.DestroyAllKnits);
        // 덮개를 흔듦.
        seq.Append(CompleteBoxAnim.Box.Object.LidShakeSequence(0.3f));

        // 박스 안에 고양이를 생성
        seq.JoinCallback(() =>
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(0.2f);
                seq.AppendCallback(() =>
                {
                    _skeletonAnim.skeleton.SetColor(new Color(1, 1, 1, 1));
                    SetCat(CompleteBoxAnim.Box.Color);
                });
                // 연기 애니메이션 실행
                seq.AppendInterval(0.2f);
                seq.AppendCallback(CompleteBoxAnim.Box.PlayEffect);
                seq.AppendInterval(0.2f);
                seq.Append(_skeletonAnim.transform.DOLocalMoveY(0.1f, 0.15f));
                seq.Join(transform.DOScale(Vector3.one * 0.9f, 0.15f));
            }
        );

        // 덮개를 열음.
        seq.Append(CompleteBoxAnim.Box.Object.LidOpenSequence(0.5f));

        // 고양이 마스크 사이즈 업
        seq.Join(CompleteBoxAnim.Box.Object.AddSizeCatMaskSequence(0.45f));

        seq.AppendInterval(0.45f);

        // 모든 오브젝트가 좁아지며 없어지는 애니메이션 실행
        seq.Append(CompleteBoxAnim.Box.Object.BoxDestroyAnimSequence());
        seq.OnComplete(() => CompleteBoxAnim.Box.Destroy());
    }

    private void SetCat(ColorType colorType)
    {
        Skeleton skeleton = _skeletonAnim.skeleton;
        string setSkinColor = colorType.ToString();

        _animIndex++;
        if (_animIndex >= AnimKeys.Length) _animIndex = 0;

        skeleton.SetSkin(setSkinColor);
        skeleton.SetToSetupPose();
        _skeletonAnim.AnimationState.Apply(skeleton);
        _skeletonAnim.AnimationState.SetAnimation(0, AnimKeys[_animIndex], false);
    }
}