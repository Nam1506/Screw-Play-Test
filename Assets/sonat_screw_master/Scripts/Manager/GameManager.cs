using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class GameManager : SingletonBase<GameManager>
{
    public event EventHandler OnChangeStateAction;

    [SerializeField] private GameState gameState;

    public EScreen eScreen;
    public bool isBusy;

    private bool isInitialized = false;

    public bool isForceGoPlay = false;

    public GameState GameState
    {
        get
        {
            return gameState;
        }

        set
        {
            if (gameState != value)
            {
                gameState = value;

                switch (gameState)
                {
                    case GameState.Home:

                        if (isForceGoPlay)
                        {
                            UIManager.Instance.ShowCoin(false);
                            UIManager.Instance.ShowLive(false);
                        }
                        else
                        {
                            Helper.DelayForTransition(() =>
                            {
                                UIManager.Instance.ShowCoin(false);
                                UIManager.Instance.ShowLive(false);
                            });
                        }


                        SonatTracking.SetCurrentScreenName(EScreen.Home);
                        break;
                    case GameState.Ingame:
                        Helper.DelayForTransition(() =>
                        {
                            UIManager.Instance.HideCoin(false);
                            UIManager.Instance.HideLive(false);
                        });


                        SonatTracking.SetCurrentScreenName(EScreen.Ingame);

                        break;
                    default:
                        break;
                }

                if (isInitialized)
                {
                    UIManager.Instance.PlayTransition();
                }

                OnChangeStateAction?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void Start()
    {
        gameState = GameState.None;

        Init();

        Debug.Log
            ("Init");
    }

    private void Init()
    {

        //if (DataManager.Instance.playerData.saveLevelData.currentLevel < GameDefine.LEVEL_GO_HOME && DataManager.Instance.playerData.lives.amount > 0)
        //{
        isForceGoPlay = true;
        GameState = GameState.Ingame;
        isForceGoPlay = false;

        BoxController.Instance.setCanvasIngame = true;

        //}
        //else
        //{
        //isForceGoPlay = true;
        //GameState = GameState.Home;
        //isForceGoPlay = false;

        //BoxController.Instance.setCanvasIngame = false;
        //}

        //GameState = GameState.Home;

        isInitialized = true;
    }
}
