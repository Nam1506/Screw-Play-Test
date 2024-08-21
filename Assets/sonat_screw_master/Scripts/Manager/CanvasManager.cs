using UnityEngine;

public class CanvasManager : SingletonBase<CanvasManager>
{
    public Canvas canvaHome;
    public Canvas canvaIngame;
    public Canvas canvaBackground;
    public Canvas canvaTransition;
    public Canvas canvaPopup;

    [SerializeField] private GameObject backgroundHome;

    private void Awake()
    {
        if (Screen.width * 1f / Screen.height >= 0.8f)
        {
            backgroundHome.GetComponent<RectTransform>().localScale = Vector3.one * 1.12f;
        }
    }
    private void Start()
    {
        GameManager.Instance.OnChangeStateAction += GameManager_OnChangeStateAction;
    }

    private void GameManager_OnChangeStateAction(object sender, System.EventArgs e)
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.Home:

                Helper.DelayForTransition(() => { GoHome(); });
                break;
            case GameState.Ingame:

                Debug.Log("Force: " + GameManager.Instance.isForceGoPlay);

                if (GameManager.Instance.isForceGoPlay)
                {
                    GoPlay();
                    GameManager.Instance.isForceGoPlay = false;
                }
                else
                {
                    Helper.DelayForTransition(() => { GoPlay(); });
                }

                break;
        }
    }

    public void GoHome()
    {
        canvaHome.gameObject.SetActive(true);

        canvaIngame.gameObject.SetActive(false);

        backgroundHome.SetActive(true);
    }

    public void GoPlay()
    {
        canvaHome.gameObject.SetActive(false);

        canvaIngame.gameObject.SetActive(true);

        backgroundHome.SetActive(false);

        GameplayManager.Instance.StartLevel();

        UIManager.Instance.UIIngame.ShowUI();

        SoundManager.Instance.StopLoopSound();
        SoundManager.Instance.StopBGSound();
    }

    public void ActiveBackgroundHome(bool isActive, bool isSetSortingLayer)
    {
        backgroundHome.SetActive(isActive);

        if (isSetSortingLayer)
        {
            canvaBackground.sortingLayerName = isActive ? "UI" : "Background";
        }
    }

    public bool IsPlayingTransiton()
    {
        return canvaTransition.gameObject.activeSelf;
    }
}
