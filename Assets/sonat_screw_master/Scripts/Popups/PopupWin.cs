using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : PopupBase
{
    [SerializeField] private SkeletonGraphic titleAnim;

    //[SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI rewardAdsText;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button adsButton;

    //[SerializeField] private RectTransform container;

    [Header("Obstacle")]
    [SerializeField] private Image obstacleIcon;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private ParticleSystem unlockEffect;
    [SerializeField] private ObstacleSO obstacleSO;

    private bool isObstacleUnlocked = false;

    private void Awake()
    {
        rewardText.text = $"{GameDefine.COIN_WIN_DEFAULT}";
        rewardAdsText.text = $"{GameDefine.COIN_WIN_DEFAULT * 2}";

        nextButton.onClick.AddListener(() =>
        {
            OnNextLevel();

            //SonatTracking.ShowInterstitial("Win", () =>
            //{
            //    UIManager.Instance.CollectCoins(10, () =>
            //    {
            //        DOVirtual.DelayedCall(0.2f, () =>
            //        {
            //            OnNextLevel();
            //        });
            //    }, nextButton.transform.position, false);
            //});
        });

        adsButton.onClick.AddListener(() =>
        {
            SonatTracking.PlayVideoAds("coin", () =>
            {
                DataManager.Instance.ChangeCoins(GameDefine.COIN_WIN_DEFAULT, false);

                OnNextLevel();
            }, "currency", "Win");

            //SonatTracking.PlayVideoAds("coin", () =>
            //{
            //    DataManager.Instance.ChangeCoins(GameDefine.COIN_WIN_DEFAULT, false);

            //    UIManager.Instance.CollectCoins(10, () =>
            //    {
            //        DOVirtual.DelayedCall(0.2f, () =>
            //        {
            //            OnNextLevel();
            //        });
            //    }, adsButton.transform.position, false);
            //}, "currency", "Win");
        });
    }

    private void OnNextLevel()
    {
        if (DataManager.Instance.playerData.saveLevelData.currentLevel >= GameDefine.LEVEL_GO_HOME)
        {
            UIManager.Instance.UICoin.SetKeepCoin();

            Helper.DelayForTransition(() =>
            {
                Hide(false, false, false);

                GameplayManager.Instance.DespawnAllObjects();
            });

            PopupManager.Instance.ForceShowPopupActions.Add(() =>
            {
                UIManager.Instance.CollectCoins(10, () =>
                {

                }, Vector3.zero, false, 0.4f);

                PopupManager.Instance.CompleteForceAction();

            });

            if (!LevelChestManager.IsCompleted() && !RaceEvent.raceData.isForceShowFinish)
            {
                UIManager.Instance.UIHome.PushPlayBtn();
            }

            #region EVENT

            UIManager.Instance.LevelChest.SetCanChange(false);
            UIManager.Instance.Race.SetCanChange(false);

            #endregion

            GameManager.Instance.GameState = GameState.Home;
        }
        else
        {
            //bool isKeepingMask = BoosterManager.Instance.IsLevelUnlockBooster() || isObstacleUnlocked
            //|| DataManager.Instance.GetCurrentLevelDiff() > EDifficulty.Normal;

            //SonatTracking.ShowInterstitial("Win", () =>
            //{

            UIManager.Instance.CollectCoins(10, () =>
            {
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    Hide(false, true, true);

                    GameplayManager.Instance.StartLevel();
                });
            }, nextButton.transform.position, false);

            //});
        }

    }

    private void OnEnable()
    {
        //CanvasManager.Instance.ActiveBackgroundHome(true, true);

        UIManager.Instance.UIIngame.HideUI(true);

        //HolesQueueController.Instance.ActiveWatchAds(false);

        CheckObstacle();

        //levelText.text = $"LEVEL {DataManager.Instance.playerData.saveLevelData.currentLevel - 1}<br>COMPLETED";
        UIManager.Instance.UICoin.ActiveMoreIcon(false);
        //UIManager.Instance.UILive.ActiveMoreIcon(false);

        titleAnim.Initialize(true);

        titleAnim.AnimationState.SetAnimation(0, "animation", false);
    }

    private void OnDisable()
    {
        UIManager.Instance.UICoin.ActiveMoreIcon(true);
        //UIManager.Instance.UILive.ActiveMoreIcon(true);
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        //CanvasManager.Instance.ActiveBackgroundHome(false, true);

        UIManager.Instance.HideCoin(false);
        UIManager.Instance.HideLive(false);

        EffectManager.Instance.FadeLightWin();

        //HolesQueueController.Instance.ActiveWatchAds(true);
    }

    private void CheckObstacle()
    {
        int currentLevel = DataManager.Instance.playerData.saveLevelData.currentLevel - 1;

        isObstacleUnlocked = false;

        progressBar.gameObject.SetActive(true);

        //container.sizeDelta = new Vector2(container.sizeDelta.x, 1195f);

        for (int i = 0; i < obstacleSO.obstacles.Count; i++)
        {
            if (currentLevel <= obstacleSO.obstacles[i].levelUnlock)
            {
                progressBar.maxValue = obstacleSO.obstacles[i].levelUnlock - obstacleSO.obstacles[i].levelStart;

                progressBar.value = currentLevel - obstacleSO.obstacles[i].levelStart - 1;

                progressText.text = progressBar.value + 1 + "/" + progressBar.maxValue;

                obstacleIcon.sprite = obstacleSO.obstacles[i].icon;
                obstacleIcon.SetNativeSize();

                progressBar.DOValue(progressBar.value + 1, 0.3f).SetDelay(PopupConfig.TIME_FADE_MASK).OnComplete(() =>
                {
                    if (progressBar.value >= progressBar.maxValue)
                    {
                        isObstacleUnlocked = true;

                        SoundManager.Instance.PlaySound(KeySound.Unlock_Obstacle);

                        unlockEffect.Play();
                    }
                });

                return;
            }
        }

        //container.sizeDelta = new Vector2(container.sizeDelta.x, 990f);

        progressBar.gameObject.SetActive(false);
    }
}
