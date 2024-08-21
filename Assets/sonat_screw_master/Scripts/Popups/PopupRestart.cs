using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PopupRestart : PopupBase
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private GameObject areYouSureText;
    [SerializeField] private GameObject restartText;

    [SerializeField] private SkeletonGraphic anim;

    private int levelLoseConfig;

    private void Awake()
    {
        levelLoseConfig = 0;

        continueButton.onClick.AddListener(() =>
        {
            //if (DataManager.Instance.playerData.saveLevelData.currentLevel == 1)
            //{
            //    if (DataManager.Instance.playerData.lives.amount > 1)
            //    {
            //        Hide(false, false, false);
            //        GameplayManager.Instance.StartLevel();
            //    }
            //    else
            //    {
            //        Hide(false, false, false);

            //        GameManager.Instance.GameState = GameState.Home;

            //        GameplayManager.Instance.ResetData();

            //        PopupManager.Instance.ShowNotiAlert("Not enough lives");
            //    }
            //    DataManager.Instance.UseLive();
            //}
            //else
            //{

            if (DataManager.Instance.playerData.saveLevelData.playCount < levelLoseConfig)
            {
                OnContinue();
            }
            else
            {
                SonatTracking.ShowInterstitial("Ingame", () =>
                {
                    OnContinue();
                });
            }

        });
    }

    private void OnContinue()
    {
        Helper.DelayForTransition(() =>
        {
            Hide(false, false, false);
        });

        GameManager.Instance.GameState = GameState.Home;

        GameplayManager.Instance.ResetData();

        DataManager.Instance.UseLive();

        SonatTracking.LogLevelEnd(DataManager.Instance.playerData.saveLevelData.currentLevel,
               DataManager.Instance.playerData.totalUseBooster,
                false,
               (int)GameplayManager.Instance.timePlay,
               DataManager.Instance.IsFirstPlay(),
               "replay",
               GameplayManager.Instance.moveCount,
               0, //GameplayManager.Instance.score,
               GameplayManager.Instance.numContinue == 0 ? "" : "revive",
               GameplayManager.Instance.numContinue
               );
    }

    public void AddAction(ERestartFrom eRestartFrom)
    {
        closeButton.onClick.RemoveAllListeners();
        replayButton.onClick.RemoveAllListeners();

        if (eRestartFrom == ERestartFrom.SettingHome)
        {
            areYouSureText.SetActive(true);
            restartText.SetActive(false);

            replayButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            areYouSureText.SetActive(false);
            restartText.SetActive(true);

            replayButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
        }

        replayButton.onClick.AddListener(() =>
        {
            switch (eRestartFrom)
            {
                case ERestartFrom.Lose:
                    if (DataManager.Instance.playerData.lives.amount > 0)
                    {
                        if (DataManager.Instance.playerData.saveLevelData.playCount < levelLoseConfig)
                        {
                            RestartLevel();
                        }
                        else
                        {
                            SonatTracking.ShowInterstitial("start_level", () =>
                            {
                                RestartLevel();
                            });
                        }
                    }
                    else
                    {
                        Hide(false, true, false);

                        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "more_live", "Ingame", "open");

                        PopupManager.Instance.ShowMoreLives();
                        PopupManager.Instance.PrePopups.Add(this);

                        //UIManager.Instance.ShowShop();
                    }
                    break;

                case ERestartFrom.SettingReplay:

                    if(DataManager.Instance.playerData.lives.amount > 1)
                    {
                        if (DataManager.Instance.playerData.saveLevelData.playCount < levelLoseConfig)
                        {
                            OnRestart();
                        }
                        else
                        {
                            SonatTracking.ShowInterstitial("start_level", () =>
                            {
                                OnRestart();
                            });
                        }
                    }
                    else
                    {
                        Hide(false, false, false);
                        PopupManager.Instance.PrePopups.Add(this);

                        UIManager.Instance.ShowShop();
                    }
                    break;

                case ERestartFrom.SettingHome:
                    break;

                default:
                    break;
            }
        });

        closeButton.onClick.AddListener(() =>
        {
            if (eRestartFrom == ERestartFrom.Lose)
            {
                Hide(false, false, false);

                GameManager.Instance.GameState = GameState.Home;

                GameplayManager.Instance.ResetData();
            }
            else
            {
                Hide(false, true, false);

                PopupManager.Instance.ShowPrePopup(null);
            }
        });
    }

    private void OnRestart()
    {
        int levelComplete = DataManager.Instance.playerData.saveLevelData.currentLevel;

        SonatTracking.LogLevelEnd(levelComplete,
        DataManager.Instance.playerData.totalUseBooster,
         false,
        (int)GameplayManager.Instance.timePlay,
        DataManager.Instance.IsFirstPlay(),
        "replay",
        GameplayManager.Instance.moveCount,
        0, //GameplayManager.Instance.score,
        GameplayManager.Instance.numContinue == 0 ? "" : "revive",
        GameplayManager.Instance.numContinue
        );

        DataManager.Instance.UseLive();

        RestartLevel();
    }

    private void RestartLevel()
    {
        if (DataManager.Instance.playerData.saveLevelData.currentLevel >= GameDefine.LEVEL_SHOW_PREBOOSTER)
        {
            Hide(false, true, true);
        }
        else
        {
            if (GameplayManager.Instance.difficulty != EDifficulty.Normal || TutorialManager.Instance.IsLevelUnlockObs())
            {
                Hide(false, true, true);
            }
            else
            {
                Hide(true, false, true);
            }
        }

        GameplayManager.Instance.StartLevel();
    }

    private void OnEnable()
    {
        SoundManager.Instance.PlaySound(KeySound.GiveUp);

        anim.initialSkinName = "Lose_live";

        anim.Initialize(true);

        anim.AnimationState.SetAnimation(0, "animation", true);
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        UIManager.Instance.HideLive(false);

        anim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        UIManager.Instance.ShowLive();

        anim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }
}

