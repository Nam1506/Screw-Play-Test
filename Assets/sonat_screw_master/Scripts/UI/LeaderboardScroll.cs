using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardScroll : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller scroller;

    public EnhancedScrollerCellView levelItemCellView;

    private int numberOfCellsPerRow = 1;
    private int numCount;

    private SmallList<LeaderboardManager.PlayerRanking> playerDatas = new();

    public UIPlayerLeaderboard myRank;

    public UIPlayerLeaderboard firstRank;
    public UIPlayerLeaderboard secondRank;
    public UIPlayerLeaderboard thirdRank;

    public void OnEnable()
    {

        scroller.Delegate = this;

        LeaderboardManager.UpdateData();

        LoadCellLevel();

        UpdateTopRank();

        UpdateMyRank();
    }

    private void Start()
    {
        JumpCurrent();
    }

    public void LoadCellLevel()
    {
        numCount = LeaderboardManager.saveLeaderboard.players.Count;

        if (playerDatas != null && playerDatas.Count > 0)
        {
            playerDatas.Clear();
        }
        else
        {
            playerDatas = new SmallList<LeaderboardManager.PlayerRanking>();
        }

        for (int i = 3; i < numCount; i++)
        {
            LeaderboardManager.PlayerRanking item = LeaderboardManager.saveLeaderboard.players[i];

            playerDatas.Add(item);
        }
    }

    private void JumpCurrent()
    {
        scroller.ReloadData();

        scroller.JumpToDataIndex(LeaderboardManager.GetIndexYou() - 3);
    }

    private void UpdateTopRank()
    {
        var leaderBoardData = LeaderboardManager.saveLeaderboard;

        firstRank.UpdateRank(leaderBoardData.players[0]);
        secondRank.UpdateRank(leaderBoardData.players[1]);
        thirdRank.UpdateRank(leaderBoardData.players[2]);
    }

    private void UpdateMyRank()
    {
        myRank.UpdateMyRank();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)numCount / (float)numberOfCellsPerRow) - 3;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 163f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UIPlayerLeaderboard cellView = scroller.GetCellView(levelItemCellView) as UIPlayerLeaderboard;

        cellView.name = "LevelItem_" + (dataIndex * numberOfCellsPerRow).ToString() + " to " + ((dataIndex * numberOfCellsPerRow) + numberOfCellsPerRow - 1).ToString();

        // in this example, we just pass the data to our cell's view which will update its UI
        cellView.Init(ref playerDatas, dataIndex * numberOfCellsPerRow);

        // return the cell to the scroller
        return cellView;
    }
}