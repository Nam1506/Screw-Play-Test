using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICollection : MonoBehaviour
{
    [SerializeField] private List<UIToggleBarCollection> bars;

    [SerializeField] private UIPanelItemCollection panelItems;

    [SerializeField] private Image previewItem;

    [SerializeField] private GameObject usingTxt;

    [SerializeField] private GameObject lockedTxt;

    [SerializeField] private Button buyBtn;
    [SerializeField] private TMP_Text price;

    [SerializeField] private Button equipBtn;

    private ETab _eTab = ETab.None;

    public UIPanelItemCollection PanelItem => panelItems;

    private void OnEnable()
    {
        //HomeController.Instance.SetPanelBotLayer(1);

        //UIManager.Instance.SendCurrencyToFront();

        _eTab = ETab.None;

        Select(ETab.Box);
    }

    private void OnDisable()
    {
        //HomeController.Instance.SetPanelBotLayer(0);

        //UIManager.Instance.SendCurrencyToBack();
    }

    private void Awake()
    {
        foreach (var toggle in bars)
        {
            var button = toggle.GetComponent<Button>();

            button.onClick.AddListener(() =>
            {
                Select(toggle.eTab);
            });
        }
    }

    private void Select(ETab eTab)
    {
        if (_eTab == eTab) return;

        _eTab = eTab;

        panelItems.Select(_eTab);

        foreach (var toggle in bars)
        {
            toggle.OnChange(toggle.eTab == _eTab);
        }
    }

    public void SetPreview(UIItemCollection itemCollection)
    {
        var data = itemCollection.Data;

        previewItem.sprite = data.icon;
        price.text = data.price.ToString();

        usingTxt.SetActive(data.eState == UIItemCollection.EState.Equipped);
        lockedTxt.SetActive(data.eState == UIItemCollection.EState.Locked);
        buyBtn.gameObject.SetActive(data.eState == UIItemCollection.EState.Unlocked);
        equipBtn.gameObject.SetActive(data.eState == UIItemCollection.EState.Collected);

        buyBtn.onClick.RemoveAllListeners();
        equipBtn.onClick.RemoveAllListeners();

        buyBtn.onClick.AddListener(() =>
        {
            itemCollection.SetBuy();
        });

        equipBtn.onClick.AddListener(() =>
        {
            itemCollection.SetUse();
        });
    }

    public void Reload()
    {
        panelItems.Reload();
    }

    public enum ETab
    {
        None = -1,
        Box = 0,
        Screw = 1,
    }

}
