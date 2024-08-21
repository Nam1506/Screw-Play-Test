using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelHomeController : SingletonBase<UILevelHomeController>
{
    [SerializeField] private Transform nodeLevelPrefab;

    [SerializeField] private Transform content;

    private List<UINodeLevel> nodeLevelList = new();

    public Sprite nodeVisualOn;
    public Sprite nodeVisualOff;

    public Sprite nodePanelOn;
    public Sprite nodePanelOff;

    private bool isStarted = false;

    public int numberInitNode = 0;

    private void OnEnable()
    {
        if (!isStarted)
        {
            InitNode();

            isStarted = true;
        }

        UpdateNode();
    }

    private void InitNode()
    {
        for (int i = 0; i < numberInitNode; i++)
        {
            var nodeLevel = Instantiate(nodeLevelPrefab, content).GetComponent<UINodeLevel>();

            nodeLevelList.Add(nodeLevel);

            nodeLevel.SetTypeRope(i % 2 == 0 ? ERope.Left : ERope.Right);
        }
    }

    private void UpdateNode()
    {
        var curLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

        for (int i = 0; i < nodeLevelList.Count; i++)
        {
            var nodeLevel = nodeLevelList[i];

            switch (i)
            {
                case 0:
                    nodeLevel.UpdateData(curLevel, ENodePos.First);
                    break;
                case 1:
                    nodeLevel.UpdateData(curLevel + i, ENodePos.Second);
                    break;

                default:
                    nodeLevel.UpdateData(curLevel + i, ENodePos.Other);
                    break;
            }

        }
    }

    public enum ERope
    {
        Left = 0,
        Right = 1
    }

    public enum ENodePos
    {
        First = 0,
        Second = 1,
        Other = 2
    }
}
