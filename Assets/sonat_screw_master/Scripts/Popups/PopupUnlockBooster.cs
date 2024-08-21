using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUnlockBooster : PopupBase
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI boosterName;
    [SerializeField] private Image boosterIcon;
    [SerializeField] private Button claimBtn;
    [SerializeField] private BoosterSO boosterSO;

    private EBoosterType boosterType;

    private void Awake()
    {
        claimBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "unlock_booster", "Ingame", "close");

            Hide(false, false, false);

            DOVirtual.DelayedCall(0.5f, () =>
            {
                UIManager.Instance.ClaimBooster(boosterType, 1);
            });
        });
    }

    public void SetBooster(EBoosterType type)
    {
        boosterType = type;

        foreach (var booster in boosterSO.boosters)
        {
            if (booster.type == type)
            {
                boosterName.text = booster.name;
                description.text = booster.description;
                boosterIcon.sprite = booster.bigIcon;
                boosterIcon.SetNativeSize();

                return;
            }
        }
    }
}
