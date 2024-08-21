using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopPack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image packIcon;
    [SerializeField] private Button buyBtn;

    public ShopItemKey key;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
#if UNITY_EDITOR
        costText.text = "$" + UIManager.Instance.UIShop.GetShopPackData(key).costValue;
#else
        ////costText.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)key);
#endif

        buyBtn.onClick.AddListener(() =>
        {
            Debug.Log("click buy " + key.ToString());

            
        });
    }

    protected virtual void OnBuySuccess()
    {
        PopupManager.Instance.ShowReward(UIManager.Instance.UIShop.GetShopPackData(key).rewards, GameplayManager.Instance.GameplayState == GameplayState.Playing);
    }
}
