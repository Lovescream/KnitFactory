using ProjectCore.Monetize;
using UnityEngine;

public class IAPPurchaseIndicator : MonoBehaviour
{
    #region Fields

    [SerializeField] [IAPSku] private string _skuId;
    #endregion
    
    // private ReceiptPrefs _receiptPrefs;
    //
    //
    // private void Awake()
    // {
    //     _receiptPrefs = Prefs.Get<ReceiptPrefs>();
    //     
    //     TurnOnOffVisibility();
    // }
    //
    // private void OnEnable()
    // {
    //     IAPCallback.OnPurchaseCompleted += OnPurchaseCompleted;
    // }
    //
    // private void OnDisable()
    // {
    //     IAPCallback.OnPurchaseCompleted -= OnPurchaseCompleted;
    // }
    //
    // private async void OnPurchaseCompleted(string skuId)
    // {
    //     await UniTask.NextFrame();
    //     if (_skuId == skuId) Util.DebugCheckError($"now skuid = {skuId}");
    //     if (_skuId != skuId) return;
    //
    //     TurnOnOffVisibility();
    // }
    //
    // private void TurnOnOffVisibility()
    // {
    //     var hasPurchase = _receiptPrefs.HasPurchase(_skuId);
    //     gameObject.SetActive(!hasPurchase);
    // }
}