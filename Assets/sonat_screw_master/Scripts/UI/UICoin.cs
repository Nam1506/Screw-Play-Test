using UnityEngine;

public class UICoin : UICurrency
{
    private bool isKeepCoin = false;

    private bool isForce = false;
    private int forceStart;
    private int forceEnd;

    public string CurrentCoinText => valueText.text;

    public void SetKeepCoin()
    {
        isKeepCoin = true;
    }

    protected override void DoMoreAction()
    {
        base.DoMoreAction();

        if (PopupManager.Instance.IsShowingMiniShop || !moreIcon.activeSelf) return;

        //SonatTracking.ClickIcon("more_coin", PlayerData.Instance.savePlayerData.currentLevel, GameManager.Instance.eScreen.ToString());

        SoundManager.Instance.PlaySound(KeySound.BtnOpen);

        PopupBase showingPopup = PopupManager.Instance.GetShowingPopup();

        if (showingPopup != null)
        {
            PopupManager.Instance.PrePopups.Add(showingPopup);
            showingPopup.HideImmediate();
        }

        //if (GameManager.Instance.GameState == GameState.Ingame)
        //{
        //UIManager.Instance.ShowShop();

        PopupManager.Instance.AddCurPopupToPre();

        PopupManager.Instance.ShowMiniShop();

        //}
        //else
        //{
        //    HomeController.Instance.SelectPanel(EPanelHome.Shop);

        //    UIManager.Instance.UIShop.ScrollTo(UIShop.EContent.Coin);
        //}


        SonatTracking.LogOpenShop();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        DataManager.Instance.OnUpdateCoinAction -= DataManager_OnUpdateCoinAction;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (isKeepCoin)
        {
            isKeepCoin = false;
        }
        else
        {
            SetText(DataManager.Instance.playerData.coins);
        }

        CheckWin();

        DataManager.Instance.OnUpdateCoinAction += DataManager_OnUpdateCoinAction;
    }

    private void DataManager_OnUpdateCoinAction(object sender, System.EventArgs e)
    {
        if (isForce)
        {
            isForce = false;

            TweenText(forceStart, forceEnd, 1f);

            return;
        }

        TweenText(int.Parse(valueText.text), DataManager.Instance.playerData.coins, 1f);
    }

    private void CheckWin()
    {
        if (PopupManager.Instance.IsWinning)
        {
            SetText(DataManager.Instance.playerData.coins - GameDefine.COIN_WIN_DEFAULT);
        }
    }

    public void SetForce(int start, int add)
    {
        isForce = true;
        forceStart = start;
        forceEnd = start + add;
    }
}
