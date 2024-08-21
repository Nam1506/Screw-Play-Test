using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LeaderboardManager;

public class UIPlayerLeaderboard : EnhancedScrollerCellView
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text levelTxt;
    [SerializeField] private TMP_Text orderTxt;

    [SerializeField] private Image avt;

    public void Init(ref SmallList<PlayerRanking> data, int startingIndex)
    {
        var _data = data[startingIndex];

        UpdateRank(_data, startingIndex + 4);
    }

    public void UpdateRank(PlayerRanking playerRanking, int order = -1)
    {
        nameTxt.text = playerRanking.name;
        levelTxt.text = playerRanking.level.ToString();

        if (avt)
        {
            avt.sprite = DataManager.Instance.avtLeaderboardDatas[playerRanking.avtId];
        }

        if (orderTxt == null) return;

        if (order != -1)
            orderTxt.text = order + ".";
    }

    public void UpdateMyRank()
    {
        var myDataIndex = saveLeaderboard.players.FindIndex(x => x.name == "YOU");

        var myData = saveLeaderboard.players[myDataIndex];

        UpdateRank(myData, myDataIndex + 1);

    }
}
