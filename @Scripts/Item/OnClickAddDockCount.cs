using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickAddDockCount : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler
{
    private const float OriginPosX = -0.03f;
    private const float OriginPosY = 0.188f;
    private const float DownPosY = 0.03f;

    private bool _canCLick = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_canCLick) return;
        if (PlayerData.Gold.Value < GameScene.DockAddSlotCoin) return;
        PlayerData.Gold.Value -= GameScene.DockAddSlotCoin;
        ResetPos();
        Main.Board.Dock.AddDockSlot(Dock.AddSlotCount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _canCLick = false;
        ResetPos();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _canCLick = true;
        transform.DOLocalMoveY(OriginPosY - DownPosY, 0.3f).SetEase(Ease.OutQuad);
        transform.DOScaleY(1 - DownPosY * 3f, 0.3f).SetEase(Ease.OutQuad);
    }

    private void ResetPos()
    {
        transform.DOKill();
        transform.DOLocalMoveY(OriginPosY, 0.3f).SetEase(Ease.OutQuad);
        transform.DOScaleY(1, 0.3f).SetEase(Ease.OutQuad);
    }
}
