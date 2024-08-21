using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterBase : MonoBehaviour
{
    public EBoosterType eBoosterType;

    private BoosterInfo info;

    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI levelUnlockText;
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected Button button;

    [SerializeField] protected GameObject lockObj;
    [SerializeField] protected GameObject moreObj;
    [SerializeField] protected GameObject amountObj;
    [SerializeField] protected ToggleScript toggleBg;
    [SerializeField] protected ParticleSystem unlockEffect;

    public int levelUnlock;

    public bool IsLocking { get; set; }
    public bool IsBlocking { get; set; }
    public int Amount { get; set; }

    public void LoadInfo()
    {
        info = BoosterManager.Instance.GetBoosterInfo(eBoosterType);

        name = eBoosterType.ToString();

        icon.sprite = info.icon;
        icon.SetNativeSize();

        levelUnlock = info.levelUnlock;
        levelUnlockText.text = "LEVEL " + info.levelUnlock;
    }

    public virtual void LoadData()
    {
        var boosters = DataManager.Instance.playerData.boosters;

        foreach (var booster in boosters)
        {
            if (booster.type == eBoosterType.ToString())
            {
                Amount = booster.amount;
                break;
            }
        }

        amountText.text = Amount.ToString();

        if (DataManager.Instance.playerData.saveLevelData.currentLevel < levelUnlock)
        {
            IsLocking = true;
            Lock();
        }
        else
        {
            IsLocking = false;
            Unlock(DataManager.Instance.playerData.saveLevelData.currentLevel == levelUnlock && DataManager.Instance.IsFirstPlay());
        }
    }

    public virtual void Lock()
    {
        lockObj.SetActive(true);
        moreObj.SetActive(false);
        toggleBg.OnChanged(false);
        amountObj.SetActive(false);
        icon.gameObject.SetActive(false);
        levelUnlockText.gameObject.SetActive(true);

        button.onClick.RemoveAllListeners();
    }

    public virtual void Unlock(bool isStartUnlock)
    {
        //if (isStartUnlock)
        //{
        //    lockObj.SetActive(true);
        //    moreObj.SetActive(false);
        //    toggleBg.OnChanged(false);
        //    amountObj.SetActive(false);
        //    icon.gameObject.SetActive(false);
        //    levelUnlockText.gameObject.SetActive(true);

        //    PopupManager.Instance.ShowUnlockBooster(eBoosterType);
        //}
        //else
        //{
            UnlockVisual();
        //}
    }

    public virtual void UnlockVisual()
    {
        lockObj.SetActive(false);
        toggleBg.OnChanged(true);
        icon.gameObject.SetActive(true);
        levelUnlockText.gameObject.SetActive(false);

        CheckOutOfBooster();
    }

    public virtual void CheckOutOfBooster()
    {
        button.onClick.RemoveAllListeners();

        if (!IsLocking)
        {
            CheckAmount();
        }
        else
        {
            button.onClick.AddListener(() =>
            {
                //PopupManager.Instance.ShowNotiAlert($"Unlock on level {levelUnlock}");
            });
        }
    }

    protected virtual void CheckAmount()
    {
        if (Amount > 0)
        {
            amountObj.SetActive(true);
            moreObj.SetActive(false);

            SetUseAction();
        }
        else
        {
            amountObj.SetActive(false);
            moreObj.SetActive(true);

            button.onClick.AddListener(() =>
            {
                Add(1);
            });
        }
    }

    public virtual void Add(int value, bool isLog = true, bool isPreBooster = false, bool canSetText = true)
    {
        var boosters = DataManager.Instance.playerData.boosters;

        foreach (var booster in boosters)
        {
            if (booster.type == eBoosterType.ToString())
            {
                Amount = booster.amount;
                break;
            }
        }

        Amount += value;

        if (canSetText)
        {
            amountText.text = Amount.ToString();
        }

        Save();

        CheckOutOfBooster();

        if (isLog)
        {
            SonatTracking.LogEarnCurrency(eBoosterType.ToString(), isPreBooster ? "pre_booster" : "booster", value, "ingame", "non_iap",
                DataManager.Instance.playerData.saveLevelData.currentLevel, "currency", "coin");
        }
    }

    public virtual void MatchValue()
    {
        amountText.text = Amount.ToString();
    }

    public virtual void Use()
    {
        Amount--;
        Amount = Mathf.Max(Amount, 0);
        amountText.SetText(Amount.ToString());

        DataManager.Instance.playerData.totalUseBooster++;

        Save();

        CheckOutOfBooster();

        SonatTracking.LogUseBoosterUA();
        SonatTracking.LogUseBooster(DataManager.Instance.playerData.saveLevelData.currentLevel, eBoosterType.ToString());
        SonatTracking.LogSpendCurrency(eBoosterType.ToString(), eBoosterType.ToString(), 1, "Ingame", "booster");
    }

    public virtual void UseFree()
    {
        DataManager.Instance.playerData.totalUseBooster++;

        Save();

        SonatTracking.LogUseBoosterUA();
        SonatTracking.LogUseBooster(DataManager.Instance.playerData.saveLevelData.currentLevel, eBoosterType.ToString());
        SonatTracking.LogSpendCurrency(eBoosterType.ToString(), eBoosterType.ToString(), 1, "Ingame", "booster");
    }

    public virtual void ForceUse(bool isFree = false)
    {
        MatchValue();
    }

    public virtual void Prepare()
    {
        UIManager.Instance.PrepareHammer();

        BoosterManager.Instance.IsUsingHammer = true;
    }

    public virtual void UnPrepare()
    {
        if (eBoosterType != EBoosterType.Hammer) return;

        UIManager.Instance.UnPrepareHammer();

        BoosterManager.Instance.IsUsingHammer = false;
    }

    private void SetUseAction()
    {
        if (eBoosterType == EBoosterType.Hammer)
        {
            button.onClick.AddListener(Prepare);
        }
        else
        {
            button.onClick.AddListener(Use);
        }
    }

    public void PlayUnlockEffect()
    {
        unlockEffect.Play();
    }

    protected virtual bool CanClick()
    {
        if (IsBlocking || GameplayManager.Instance.GameplayState == GameplayState.Pausing)
            return false;

        return true;
    }

    protected virtual bool CheckValid()
    {
        return true;
    }

    private void Save()
    {
        foreach (Booster booster in DataManager.Instance.playerData.boosters)
        {
            if (booster.type == eBoosterType.ToString())
            {
                booster.amount = Amount;
                DataManager.Instance.Save();
            }
        }
    }
}
