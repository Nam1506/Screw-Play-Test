using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupForceBooster : PopupBase
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI boosterName;
    [SerializeField] private Image boosterIcon;
    [SerializeField] private SkeletonGraphic smallBoosterAnim;
    [SerializeField] private Button claimBtn;

    private EBoosterType boosterType;

    //private void Awake()
    //{
    //    claimBtn.onClick.AddListener(() =>
    //    {
    //        SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "unlock_booster", "Ingame", "close");

    //        Hide(false, false, false);
    //    });
    //}

    private void OnEnable()
    {
        claimBtn.onClick.RemoveAllListeners();

        claimBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "unlock_booster", "Ingame", "close");

            Hide(true, false, false);

            var booster = BoosterManager.Instance.boosterBases.Find(x => x.eBoosterType == boosterType);

            if (booster is BoosterHammer)
            {
                booster.Prepare();

                BoosterManager.Instance.boosterHammer.isFree = true;
            }
            else
            {
                booster.UseFree();
            }
        });
    }

    public void SetBooster(EBoosterType type)
    {
        boosterType = type;

        foreach (var booster in BoosterManager.Instance.boosterBases)
        {
            if (booster.eBoosterType == type)
            {
                claimBtn.transform.parent.transform.position = booster.transform.position;

                break;
            }
        }

        foreach (var booster in BoosterManager.Instance.boosterSO.boosters)
        {
            if (booster.type == type)
            {
                boosterName.text = booster.name;
                description.text = booster.description;
                boosterIcon.sprite = booster.tutIcon;

                smallBoosterAnim.initialSkinName = booster.nameSkin;
                smallBoosterAnim.Initialize(true);
                smallBoosterAnim.AnimationState.SetAnimation(0, "animation", true);

                break;
            }
        }
    }
}
