using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
    [Header("Left")]

    [SerializeField] private Button justFunBtn;

    [Space]

    [Header("Right")]
    [SerializeField] private Button settingButton;

    [Space]

    [Header("Mid")]

    [SerializeField] private Button playButton;
    [SerializeField] private ToggleScript[] toggleDiff;

    [SerializeField] private SkeletonGraphic boxAnim;
    [SerializeField] private SkeletonUtilityBone boxBone;

    [SerializeField] private TMP_Text levelTxt;

    [SerializeField] private ScrewToBox screwToBox;

    private const bool HARD = true;
    private const bool SUPER_HARD = false;

    private Canvas playCanva => playButton.GetComponent<Canvas>();

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            if (DataManager.Instance.playerData.lives.amount > 0)
            {
                UIManager.Instance.CheckNoAds24h();

                PopupManager.Instance.ClearForceAction();
                PopupManager.Instance.canSetInteractable = true;
                PopupManager.OnSetInteractableMask?.Invoke(false);

                UIManager.Instance.MatchAllProgress();

                PopPlayBtn();

                GameManager.Instance.GameState = GameState.Ingame;
            }
            else
            {
                PopupManager.Instance.ShowMoreLives();
            }
        });

        settingButton.onClick.AddListener(() =>
        {
            PopupManager.Instance.ShowSetting();
        });

        justFunBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.ShowJustFun(false);
        });
    }

    private void OnEnable()
    {
        EDifficulty difficulty = DataManager.Instance.GetCurrentLevelDiff();

        //levelTxt.text = DataManager.Instance.playerData.saveLevelData.currentLevel.ToString();

        switch (difficulty)
        {
            case EDifficulty.Hard:
                toggleDiff.SetActiveAll(true);
                toggleDiff.OnChanged(HARD);
                //box.sprite = hardBox;
                break;
            case EDifficulty.Super_Hard:
                toggleDiff.SetActiveAll(true);
                toggleDiff.OnChanged(SUPER_HARD);
                //box.sprite = superhardBox;
                break;
            default:
                toggleDiff.SetActiveAll(false);
                //box.sprite = normalBox;
                break;
        }

        //Helper.WaitForTransitionExceptIn(() =>
        //{
        //    Setup();
        //});

    }

    public void PushPlayBtn()
    {
        playCanva.sortingLayerName = "Interactable";
        playCanva.sortingOrder = 1;
    }

    public void PopPlayBtn()
    {
        playCanva.sortingLayerName = "UI";
        playCanva.sortingOrder = 0;
    }

    public bool SetupBox()
    {
        EDifficulty difficulty = DataManager.Instance.GetCurrentLevelDiff();
        EDifficulty preDifficulty = DataManager.Instance.GetPreLevelDiff();

        boxBone.Reset();

        if (CheckAnimBox())
        {
            var curLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;
            var preLevel = curLevel - 1;

            levelTxt.text = preLevel.ToString();
            boxAnim.initialSkinName = MappingDifficultyToSkinBox(preDifficulty).ToString();

            boxAnim.Initialize(true);

            return true;
        }
        else
        {
            var curLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

            levelTxt.text = curLevel.ToString();
            boxAnim.initialSkinName = MappingDifficultyToSkinBox(difficulty).ToString();

            boxAnim.Initialize(true);

            return false;
        }
    }

    public void Setup(bool canAnim)
    {
        EDifficulty difficulty = DataManager.Instance.GetCurrentLevelDiff();

        if (canAnim)
        {
            EDifficulty preDifficulty = DataManager.Instance.GetPreLevelDiff();

            StartCoroutine(PlayAnimBox(preDifficulty, difficulty));
            screwToBox.Spawn();
        }
        else
        {
            PopupManager.Instance.StartForceAction();
        }
    }

    public bool canAnimBox = false;

    public bool CheckAnimBox()
    {
        //return DataManager.Instance.playerData.collectScrewData.Count != 0;

        if (!canAnimBox) return false;

        canAnimBox = false;

        return true; 
    }

    private IEnumerator PlayAnimBox(EDifficulty pre, EDifficulty cur)
    {
        SoundManager.Instance.PlayBGSound();

        PopupManager.OnSetInteractableMask?.Invoke(true);

        var curLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

        boxAnim.Initialize(true);

        yield return new WaitForSeconds(screwToBox.timeDelayBox);

        boxAnim.AnimationState.SetAnimation(0, EStateBox.Drop.ToString(), true);

        SoundManager.Instance.PlaySound(KeySound.Home_Box_Fill);

        yield return new WaitForSeconds(screwToBox.totalTime + screwToBox.timeAfter);

        ResizeCamera.Instance.Resize();

        boxAnim.AnimationState.SetAnimation(0, EStateBox.Out.ToString(), false);

        SoundManager.Instance.PlaySound(KeySound.Home_Box_Out);

        yield return new WaitForSeconds(Helper.GetTimeAnim(boxAnim, EStateBox.Out.ToString()));

        levelTxt.text = curLevel.ToString();
        boxAnim.initialSkinName = MappingDifficultyToSkinBox(cur).ToString();

        boxAnim.Initialize(true);

        boxAnim.AnimationState.SetAnimation(0, EStateBox.In.ToString(), false);
        boxAnim.AnimationState.AddAnimation(0, EStateBox.Idle.ToString(), true, 0);

        SoundManager.Instance.PlaySound(KeySound.Home_Box_In);

        DataManager.Instance.ClearCollectScrewData();

        yield return new WaitForSeconds(Helper.GetTimeAnim(boxAnim, EStateBox.In.ToString()));

        SoundManager.Instance.DecreaseVolume();

        if (PopupManager.Instance.HasAnyForceAction())
        {
            PopupManager.Instance.StartForceAction();
        }
        else
        {
            PopupManager.OnSetInteractableMask?.Invoke(false);
        }

    }

    private ESkinBox MappingDifficultyToSkinBox(EDifficulty eDiff)
    {
        switch (eDiff)
        {
            case EDifficulty.Normal:
                return ESkinBox.Normal;

            case EDifficulty.Hard:
                return ESkinBox.Hard;

            case EDifficulty.Super_Hard:
                return ESkinBox.Superhard;

            default: return ESkinBox.Normal;
        }
    }

    private enum ESkinBox
    {
        Hard,
        Normal,
        Superhard
    }

    private enum EStateBox
    {
        Drop,
        Idle,
        In,
        Out
    }
}