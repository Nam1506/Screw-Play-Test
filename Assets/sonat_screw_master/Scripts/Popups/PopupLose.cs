using DanielLochner.Assets.SimpleScrollSnap;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLose : PopupBase
{
    [SerializeField] private Button keepPlayingButton;
    [SerializeField] private Button giveUpButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI playOnText;
    [SerializeField] private Image loseImg;
    [SerializeField] private SkeletonGraphic anim;
    [SerializeField] private ReviveBundle safetyPack;
    [SerializeField] private ReviveBundle hardLevelPack;

    [SerializeField] private GameObject packParent;

    [SerializeField] private LoseSO loseSO;

    [SerializeField] private SimpleScrollSnap snap;

    private int coinRevive;
    private LoseCause loseCause;

    private void Awake()
    {
        snap.ForceStart();

        snap.ForceResize(new Vector2(980.2109f, 469.66f));

        if (GameplayManager.Instance.difficulty == EDifficulty.Normal)
        {
            snap.StartingPanel = 0;
        }
        else
        {
            snap.StartingPanel = 1;
        }

        keepPlayingButton.onClick.AddListener(() =>
        {
            int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

            SonatTracking.ClickIcon("keep_playing", level, "Lose");

            if (DataManager.Instance.playerData.coins < coinRevive)
            {
                HideImmediate();

                PopupManager.Instance.PrePopups.Add(this);

                UIManager.Instance.ShowShop();

                PopupManager.Instance.ShowNotiAlert("Not enough coins");
            }
            else
            {
                DataManager.Instance.ChangeCoins(-coinRevive);

                SonatTracking.LogSpendCurrency("coin", "currency", coinRevive, "Lose", "booster", "revive");

                Revive();
            }
        });

        giveUpButton.onClick.AddListener(() =>
        {
            int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

            SonatTracking.ClickIcon("give_up", level, "Lose");
            SonatTracking.LogShowUI("user", level, "Lose", "Ingame", "close");

            Hide(false, true, false);
            UIManager.Instance.HideCoin();

            PopupManager.Instance.ShowRestart(ERestartFrom.Lose);
            PopupManager.Instance.PrePopups.Add(this);
        });
    }

    private void OnEnable()
    {
        if (GameplayManager.Instance.LoseCause == LoseCause.FullHole)
        {
            keepPlayingButton.gameObject.SetActive(!HolesQueueController.Instance.IsMaxHoleLose());
            playOnText.text = "+2 holes";
        }
        else
        {
            playOnText.text = "remove obstacles";
            keepPlayingButton.gameObject.SetActive(true);
        }

        packParent.gameObject.SetActive(!HolesQueueController.Instance.IsMaxHoleLose());

        safetyPack.gameObject.SetActive(!HolesQueueController.Instance.IsMaxHoleLose());
        //hardLevelPack.gameObject.SetActive(false);

        safetyPack.SetBonusText(GameplayManager.Instance.LoseCause == LoseCause.FullHole ? "Continue with +2 holes" : "Keep playing");

        //safetyPack.gameObject.SetActive(false);
        hardLevelPack.gameObject.SetActive(!HolesQueueController.Instance.IsMaxHoleLose());

        hardLevelPack.SetBonusText(GameplayManager.Instance.LoseCause == LoseCause.FullHole ? "Continue with +2 holes" : "Keep playing");

        if (GameplayManager.Instance.difficulty == EDifficulty.Normal)
        {
            snap.GoToPanel(0);
        }
        else
        {
            snap.GoToPanel(1);
        }

        //if (GameplayManager.Instance.difficulty == EDifficulty.Normal)
        //{
        //    safetyPack.gameObject.SetActive(!HolesQueueController.Instance.IsMaxHoleLose());
        //    hardLevelPack.gameObject.SetActive(false);

        //    safetyPack.SetBonusText(GameplayManager.Instance.LoseCause == LoseCause.FullHole ? "Continue with +2 holes" : "Keep playing");
        //}
        //else
        //{
        //    safetyPack.gameObject.SetActive(false);
        //    hardLevelPack.gameObject.SetActive(!HolesQueueController.Instance.IsMaxHoleLose());

        //    hardLevelPack.SetBonusText(GameplayManager.Instance.LoseCause == LoseCause.FullHole ? "Continue with +2 holes" : "Keep playing");
        //}

        Setup();
    }

    private void Setup()
    {
        loseCause = GameplayManager.Instance.LoseCause;

        LoseInfo loseInfo = loseSO.GetLoseInfo(loseCause);

        coinRevive = loseInfo.coinRevive;

        switch (loseCause)
        {
            case LoseCause.FullHole:
                anim.gameObject.SetActive(true);
                loseImg.gameObject.SetActive(false);

                anim.initialSkinName = "Lose";
                anim.Initialize(true);
                anim.AnimationState.SetAnimation(0, "animation", true);

                playOnText.text = "+2 holes";
                break;
            default:
                anim.gameObject.SetActive(false);
                loseImg.gameObject.SetActive(true);
                loseImg.sprite = loseInfo.sprite;
                loseImg.SetNativeSize();

                playOnText.text = "remove obstacles";
                break;
        }

        contentText.text = loseInfo.description;
        costText.text = loseInfo.coinRevive.ToString();
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        UIManager.Instance.ShowCoin();
        UIManager.Instance.ShowLive();
    }

    public void Revive(bool isByCoin = true)
    {

        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        switch (loseCause)
        {
            case LoseCause.FullHole:
                HolesQueueController.Instance.Revive();
                break;
            case LoseCause.Ice:
                foreach (Screw screw in GameplayManager.Instance.screws)
                {
                    if (!screw.IsOverlap())
                        screw.BreakAllIce();
                }
                break;
            case LoseCause.Gate:
                foreach (Screw screw in GameplayManager.Instance.screws)
                {
                    if (!screw.IsOverlap())
                        screw.RemoveGate();
                }
                break;
            default:
                break;
        }

        Hide(true, false, true);

        UIManager.Instance.HideCoin();
        UIManager.Instance.HideLive();

        DataManager.Instance.RevertFakeReset();

        if (isByCoin)
        {
            SonatTracking.LogBuyShopItem("booster", "revive", 1, "coin", DataManager.Instance.IsFirstBuy());
            SonatTracking.LogEarnCurrency("revive", "booster", 1, "Lose", "non_iap", level, "currency", "coin");
        }
        else
        {
            SonatTracking.LogBuyShopItem("booster", "revive", 1, IAPManager.packClicked.ToString(), DataManager.Instance.IsFirstBuy());
            SonatTracking.LogEarnCurrency("revive", "booster", 1, "Lose", "iap", level, "pack", IAPManager.packClicked.ToString());
        }

        SonatTracking.LogSpendCurrency("revive", "booster", 1, "Lose", loseCause.ToString(), "booster");
        SonatTracking.LogUseBooster(level, "revive");


        DataManager.Instance.playerData.totalUseBooster++;

        GameplayManager.Instance.CheckLose();
        GameplayManager.Instance.CheckObstaclesLose();

        HolesQueueController.Instance.StopWarningAll();
    }
}
