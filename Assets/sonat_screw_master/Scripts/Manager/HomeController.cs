using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : SingletonBase<HomeController>
{
    [SerializeField] private List<HomePanel> homePanels;
    [SerializeField] private Canvas panelBot;

    private EPanelHome curPanel = EPanelHome.None;

    [SerializeField] private HorizontalLayoutGroup botPanels;

    private void Awake()
    {
        //GameManager.Instance.GameState = GameState.Home;
    }

    private void Start()
    {
        SetActionAllPanels();

        SelectPanel(EPanelHome.Home);
    }

    public void GoHome()
    {
        SelectPanel(EPanelHome.Home);
    }

    private void SetActionAllPanels()
    {
        foreach (var panel in homePanels)
        {
            panel.SetAction(() =>
            {
                SelectPanel(panel.ePanelHome);
            });
        }
    }

    public void SelectPanel(EPanelHome panelHome)
    {
        if (curPanel == panelHome) return;

        curPanel = panelHome;

        if (curPanel == EPanelHome.Home)
        {
            SetPanelBotLayer(0);
        }
        else
        {
            SetPanelBotLayer(1);
        }

        if (curPanel == EPanelHome.Leaderboard || curPanel == EPanelHome.Area)
        {
            UIManager.Instance.SendCurrencyToBack();
        }
        else
        {
            UIManager.Instance.SendCurrencyToFront();
        }

        foreach (var homePanel in homePanels)
        {
            homePanel.OnSelect(homePanel.ePanelHome == curPanel);
        }
    }

    public void SetPanelBotLayer(int layer)
    {
        panelBot.sortingOrder = layer;
    }

    public Transform GetPanel(EPanelHome panel)
    {
        HomePanel homePanel = homePanels.Find(p => p.ePanelHome == panel);

        Debug.Log(homePanel);

        if (homePanel == null) return null;
        else return homePanel.transform;
    }
}

public enum EPanelHome
{
    None = -1,
    Home = 0,
    Shop = 1,
    Leaderboard = 2,
    Area = 3,
    Collection = 4,
}