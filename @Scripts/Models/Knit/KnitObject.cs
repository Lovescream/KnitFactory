using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class KnitObject : Entity
{
    #region Properties

    public Knit Knit { get; private set; }

    #endregion

    #region Fields

    private float _moveProgress;
    private Direction _moveDirection;

    private Tween _tween;
    private Sequence _sequence;
    private Coroutine _coMove;
    private GameScene _gameScene;

    // Components.
    private SkeletonAnimation _knitSkeletonAnimation;
    private SkeletonAnimation _tailSkeletonAnimation;

    #endregion

    #region MonoBehaviours

    void Update()
    {
        if (Knit.State == KnitState.OnBelt &&
            Knit.Belt != null &&
            GameScene.InGameState == InGameState.Processing) MoveOnBelt();
    }

    void OnDisable()
    {
        _tween?.Kill();
        _sequence?.Kill();
        if (_coMove != null) StopCoroutine(_coMove);
    }

    void OnDestroy()
    {
        if (_gameScene != null)
        {
            _gameScene.OnInGameStop -= OnInGameStop;
            _gameScene.OnInGameProcessing -= OnInGameProcessing;
        }
    }

    #endregion

    #region Initialize / Set

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;

        _knitSkeletonAnimation = this.gameObject.FindChild<SkeletonAnimation>("Knit");
        _tailSkeletonAnimation = this.gameObject.FindChild<SkeletonAnimation>("KnitTail");


        _gameScene = (Main.Scene.Current as GameScene);
        if (_gameScene != null)
        {
            _gameScene.OnInGameStop += OnInGameStop;
            _gameScene.OnInGameProcessing += OnInGameProcessing;
        }

        return true;
    }

    public void Set(Knit knit)
    {
        Initialize();
        Knit = knit;
        _moveProgress = 0;

        SetSkin();
        SetGimmicks();

        SetAnimation();
    }

    private void SetSkin()
    {
        Skeleton skeleton = _knitSkeletonAnimation.Skeleton;
        skeleton.SetSkin($"{Knit.Color}");
        skeleton.SetToSetupPose();
        _knitSkeletonAnimation.AnimationState.Apply(skeleton);
        skeleton = _tailSkeletonAnimation.Skeleton;
        skeleton.SetSkin($"{Knit.Color}");
        skeleton.SetToSetupPose();
        _tailSkeletonAnimation.AnimationState.Apply(skeleton);
    }

    private void SetGimmicks()
    {
        foreach (IKnitGimmick gimmick in Knit.Gimmicks)
        {
            gimmick.GenerateNewObject();
        }
    }

    public void SetAnimation(Direction direction = Direction.None)
    {
        if (_moveDirection == direction) return;
        _moveDirection = direction;
        if (_moveDirection == Direction.None)
        {
            _knitSkeletonAnimation.timeScale = 0;
            _tailSkeletonAnimation.gameObject.SetActive(false);
        }
        else
        {
            _knitSkeletonAnimation.timeScale = 1;
            _tailSkeletonAnimation.gameObject.SetActive(true);
            Transform t = _tailSkeletonAnimation.transform;
            switch (_moveDirection)
            {
                case Direction.Top:
                    _knitSkeletonAnimation.skeleton.ScaleX = 1;
                    t.localPosition = new(0.16f, 0f);
                    t.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.Right:
                    _knitSkeletonAnimation.skeleton.ScaleX = 1;
                    t.localPosition = new(0f, -0.15f);
                    t.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Bottom:
                    _knitSkeletonAnimation.skeleton.ScaleX = -1;
                    t.localPosition = new(-0.18f, 0f);
                    t.localRotation = Quaternion.Euler(0, 0, 270);
                    break;
                case Direction.Left:
                    _knitSkeletonAnimation.skeleton.ScaleX = -1;
                    t.localPosition = new(0f, 0.17f);
                    t.localRotation = Quaternion.Euler(0, 0, 180);
                    break;
            }
        }
    }

    #endregion

    #region Controller

    public void OnSelected()
    {
        if (GameScene.GameState == GameState.InTutorial) return;
        if (Knit.State == KnitState.OnDockSlot)
        {
            Main.Board.Dock.MoveToSsagae(Knit.DockSlot);
        }
        else if (Knit.State == KnitState.OnAddSpaceSlot)
        {
            Main.Board.Dock.MoveToSsagae(Knit.AddSpaceSlot);
        }
    }

    #endregion

    #region InBundle

    public void Spawn()
    {
        _sequence?.Kill();
        transform.DOKill();
        this.transform.SetParent(Knit.Bundle.Board.UI_Scene.Knits, false);
        this.transform.localScale = Vector3.zero;
        this.transform.position = new(Knit.Bundle.GetPosition(Knit).x, Knit.Bundle.StartY);
        _knitSkeletonAnimation.maskInteraction = SpriteMaskInteraction.None;
        _sequence = DOTween.Sequence()
            .AppendCallback(() => transform.position = new(Knit.Bundle.GetPosition(Knit).x, Knit.Bundle.StartY))
            .Append(this.transform.DOScale(Main.Screen.Camera.ApplyScale(), 0.25f).SetEase(Ease.InOutBounce))
            .OnComplete(() =>
            {
                Knit.State = KnitState.Waiting;
                Knit.Pull();
            });
    }

    public void SpawnSlot()
    {
        Transform transform = Knit?.BoxSlot?.Object?.transform;
        if (transform == null)
        {
            Destroy();
            Knit.State = KnitState.OnBox;
            Knit.BoxSlot.Box.OnCompletedMove();
            return;
        }

        _knitSkeletonAnimation.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        this.transform.SetParent(transform);
        this.transform.localScale = Vector3.zero;
        this.transform.position = Knit.BoxSlot.Position;
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOScale(1, 0.25f).SetEase(Ease.InOutBounce))
            .OnComplete(() =>
            {
                Knit.State = KnitState.OnBox;
                Knit.BoxSlot.Box.OnCompletedMove();
            });
    }

    public void Pull()
    {
        Vector2 targetPosition = Knit.Bundle.GetPosition(Knit);
        _tween?.Kill();
        _tween = this.transform.DOMove(targetPosition, 3.14f * Main.Screen.Camera.ApplyRatio())
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .SetOptions(AxisConstraint.None);
    }

    #endregion

    #region Move

    public void MoveToSsagae()
    {
        if (Knit.State != KnitState.MoveToSsagae) return;
        this.transform.SetParent(Main.Board.BeltBoard.Ssagae.Object.transform);
        _moveProgress = 0;
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector3.up * 0.5f, 0.25f))
            .Join(this.transform.DOScale(1f, 0.25f))
            .OnComplete(CoMoveToSsagae);
    }

    private void CoMoveToSsagae()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector3.zero, 0.1f))
            .OnComplete(() =>
            {
                Knit.State = KnitState.InSsagae;
                Main.Board.BeltBoard.Ssagae.Enqueue(Knit);
            });
    }

    public void MoveToBelt(Belt belt, BeltLane lane)
    {
        if (Knit.State != KnitState.MoveToBelt || belt == null) return;
        this.transform.SetParent(belt.Object.transform);
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOMove(belt.GetPosition(0, lane), 0.25f))
            .OnComplete(() =>
            {
                Knit.Belt = belt;
            });
    }

    public void MoveOnBelt()
    {
        if (Knit.State != KnitState.OnBelt || Knit.Belt == null) return;

        SetAnimation(Knit.Belt.EndDirection);

        // 이동할 거리 계산: Belt의 속력.
        float distanceToMove = Main.Time.FrameSpeed * Time.deltaTime;

        // 한 번에 여러 개의 Belt를 이동할 수 있으므로 while을 통해 계산.
        while (distanceToMove > 0f && Knit.State == KnitState.OnBelt && Knit.Belt != null)
        {
            // 벨트에 있는 큐 검사.
            if (Knit.Belt.EnterBox(Knit)) break;

            float remainingRatio = 1 - _moveProgress; // 현재 벨트에서 남은 진행도.
            float step = Mathf.Min(remainingRatio, distanceToMove); // 이번에 소비할 진행도.
            float t = _moveProgress + step;

            // 위치 갱신.
            Vector2 position = Knit.Belt.GetPosition(t, Knit.Lane);
            this.transform.position = position;

            _moveProgress = t;
            distanceToMove -= step;

            // 완전히 이동했다면 다음 벨트로.
            if (_moveProgress >= 1f - 1e-6f)
            {
                Belt nextBelt = Knit.Belt.NextBelt;
                // 벨트 끝 도달 시 Dock으로 이동.
                if (nextBelt == null)
                {
                    if (Knit.Belt.MoveMachine != null)
                    {
                        _moveProgress = 0f;
                        Knit.MoveToMoveMachine(Knit.Belt.MoveMachine);
                        break;
                    }

                    _moveProgress = 1f;
                    Knit.MoveToDock();
                    break;
                }

                Knit.Belt = nextBelt;
                _moveProgress = 0f;

                this.transform.position = nextBelt.GetPosition(0, Knit.Lane);
            }
        }
    }

    public void MoveToDockSlot()
    {
        // ... 중략 ...
        if (Knit.State != KnitState.MoveToDockSlot || Knit.DockSlot == null) return;
        this.transform.SetParent(Knit.DockSlot.Object.transform);
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector3.zero, 1f))
            .Join(this.transform.DOScale(1, 1f))
            .OnComplete(() =>
            {
                Knit.State = KnitState.OnDockSlot;
            });
    }

    public void MoveToAddSpaceSlot()
    {
        if (Knit.State != KnitState.MoveToAddSpaceSlot || Knit.AddSpaceSlot == null) return;
        this.transform.SetParent(Knit.AddSpaceSlot.Object.transform);
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector3.zero, 1f))
            .Join(this.transform.DOScale(1, 1f))
            .OnComplete(() => { Knit.State = KnitState.OnAddSpaceSlot; });
    }

    public void MoveToBox()
    {
        if (Knit.State != KnitState.MoveToBox || Knit.BoxSlot == null) return;
        this.transform.SetParent(Knit.BoxSlot.Object.transform);
        _knitSkeletonAnimation.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector3.up * 0.125f, 0.1f))
            .OnComplete(() =>
            {
                if (_coMove != null) StopCoroutine(_coMove);
                _coMove = StartCoroutine(CoMoveToBox());
            });
    }

    private IEnumerator CoMoveToBox()
    {
        yield return new WaitUntil(() => Knit.BoxSlot.Box.State == BoxState.Idle);

        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOLocalMove(Vector3.zero, 0.1f))
            .OnComplete(() =>
            {
                Knit.State = KnitState.OnBox;
                Knit.BoxSlot.Box.OnCompletedMove();
            });
    }

    public void MoveToMoveMachine()
    {
        MoveMachine inputMoveMachine = Knit.MoveMachine;
        MoveMachine outputMoveMachine = inputMoveMachine.ConnectMoveMachine;

        Vector3 startPos = inputMoveMachine.Object.transform.position;
        Vector3 endPos = outputMoveMachine.Object.transform.position;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(this.transform.DOMove(startPos + Vector3.up, 0.1f));
        _sequence.Append(this.transform.DOMove(startPos, 0.1f));
        _sequence.AppendCallback(() => this.transform.position = endPos);
        _sequence.Append(this.transform.DOMove(endPos + Vector3.up, 0.1f));
        _sequence.Append(this.transform.DOMove(outputMoveMachine.GetStartPos(), 0.1f));
        _sequence.AppendCallback(() => outputMoveMachine.Poop(Knit));
    }

    private void OnInGameProcessing()
    {
        _tailSkeletonAnimation.enabled = true;
        _knitSkeletonAnimation.enabled = true;
    }

    private void OnInGameStop()
    {
        _tailSkeletonAnimation.enabled = false;
        _knitSkeletonAnimation.enabled = false;
    }

    #endregion

    public void Destroy()
    {
        if (Knit == null)
        {
            Debug.LogError($"Knit is null");
            return;
        }

        if (Knit.Bundle != null)
        {
            if (Knit.Bundle.WaitingKnits.Contains(Knit))
            {
                Knit.Bundle.RemoveWaitKnit(Knit);
            }
            else if (Knit.Bundle.InKnit.Contains(Knit))
            {
                Knit.Bundle.RemoveInKnit(Knit);
            }
        }

        if (Knit.TryGet(out KnitHidden hidden)) hidden.OnRemoved();
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(this.transform.DOScale(0, 0.25f).SetEase(Ease.InOutBounce))
            .OnComplete(() =>
            {
                Knit.State = KnitState.None;
                Main.Object.Destroy(this);
            });
    }
}