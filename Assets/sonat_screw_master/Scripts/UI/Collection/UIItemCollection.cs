using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemCollection : MonoBehaviour
{
    [SerializeField] private Button clickBtn;
    [SerializeField] private ToggleScriptSetImage toggleBG;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text price;

    [SerializeField] private Transform buyButton;
    [SerializeField] private Transform equipTransform;
    [SerializeField] private Transform lockTransform;
    [SerializeField] private Transform fakeLock;
    [SerializeField] private Button useButton;

    private DataItem _data;

    public DataItem Data => _data;

    private void Awake()
    {
        clickBtn.onClick.AddListener(() =>
        {
            SetClick();
        });

        useButton.onClick.AddListener(() =>
        {
            SetUse();
        });
    }

    public void SetClick()
    {
        UIManager.Instance.UICollection.PanelItem.SelectItem(_data.id);
    }

    public void SetBuy()
    {
        var coin = DataManager.Instance.playerData.coins;

        if (coin < _data.price)
        {
            PopupManager.Instance.ShowNotiAlert("Not enough coins");
        }
        else
        {
            DataManager.Instance.ChangeCoins(-(int)_data.price);

            SkinSystem.UnlockSkin(_data.eSkin, _data.id);
            SkinSystem.EquipSkin(_data.eSkin, _data.id);

            UIManager.Instance.UICollection.Reload();
        }
    }

    public void SetUse()
    {
        SkinSystem.EquipSkin(_data.eSkin, _data.id);

        UIManager.Instance.UICollection.Reload();
    }

    public void Select(bool state)
    {
        toggleBG.OnChanged(state);
    }

    public void SetData(DataItem data)
    {
        _data = data;

        icon.sprite = _data.icon;
        icon.SetNativeSize();

        icon.transform.localScale = data.eSkin == SkinSystem.ESkin.Box ? Vector3.one : Vector3.one * 0.7f;

        price.text = _data.price.ToString();

        SetState(data.eState);
    }

    public void SetFake()
    {
        SetState(EState.Fake);
    }

    public void SetState(EState eState)
    {
        clickBtn.interactable = eState != EState.Fake;

        icon.gameObject.SetActive(eState != EState.Fake);

        buyButton.gameObject.SetActive(eState == EState.Unlocked);
        equipTransform.gameObject.SetActive(eState == EState.Equipped);
        lockTransform.gameObject.SetActive(eState == EState.Locked);
        useButton.gameObject.SetActive(eState == EState.Collected);
        fakeLock.gameObject.SetActive(eState == EState.Fake);
    }

    public enum EState
    {
        Locked = 0,
        Unlocked = 1,
        Collected = 2,
        Equipped = 3,
        Fake = 4
    }

    [Serializable]
    public class DataItem
    {
        public SkinSystem.ESkin eSkin;
        public int id;

        public Sprite icon;
        public float price;
        public EState eState;

        public DataItem(SkinSystem.ESkin eSkin, int id, Sprite icon, float price, EState eState)
        {
            this.eSkin = eSkin;
            this.id = id;
            this.icon = icon;
            this.price = price;
            this.eState = eState;
        }
    }
}
