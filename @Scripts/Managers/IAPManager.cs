using ActionFit_Plugin.IAP;
using Cysharp.Threading.Tasks;

public class IAPManager : ContentManager {

    private IAPInitializer  _iap;
    public bool IAPIsInitialed => _iap.IsInitialized(); 

    #region Initialize

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        _iap = new();
        
        return true;
    }

    public UniTask IAPInitialized() => _iap.Initialized();

    #endregion

}