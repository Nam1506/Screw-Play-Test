using DarkTonic.PoolBoss;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : SingletonBase<BoxController>
{
    [SerializeField] private Transform boxPrefab;
    [SerializeField] private Transform boxHolePrefab;
    [SerializeField] private Transform boxParent;

    [SerializeField] private BoxAvailableSO boxAvailableSO;

    public int boxID = 0;

    public int slotCount = 99;

    public bool setCanvasIngame;

    private bool isCompleteSetDistance = false;

    private float boxDistance = Screen.width / 2f;

    private const float BOX_ANCHORPOS_Y = -400f;

    public BoxAvailableSO BoxData => boxAvailableSO;

    public BoxSO GetCurSkin()
    {
        return boxAvailableSO.GetCurSkin();
    }

    public void SetupBoxes()
    {
        if (DataManager.Instance.playerData.saveLevelData.currentLevel >= 1)
        {
            boxDistance = CanvasManager.Instance.canvaIngame.GetComponent<RectTransform>().sizeDelta.x / 2f;
        }

        Debug.Log("BoxDistance: " + boxDistance);

        for (int i = 0; i < DataManager.Instance.levelData.boxes.Count; i++)
        {
            BoxData boxData = DataManager.Instance.levelData.boxes[i];

            Vector2 anchorPos;

            Debug.Log("i: " + -boxDistance);

            anchorPos = new Vector2(-boxDistance * (i + 1), BOX_ANCHORPOS_Y);

            Box box = PoolBoss.Spawn(boxPrefab, Vector3.zero, Quaternion.identity, boxParent).GetComponent<Box>();

            box.rectTransform.anchoredPosition = anchorPos;
            box.transform.localScale = Vector3.one * 1.2f;

            box.Init(boxData.eColor, boxData.holes.Count);

            for (int j = 0; j < box.holes.Count; j++)
            {
                box.holes[j].type = boxData.holes[j].eType;
                box.holes[j].screw = null;

                box.SetHoleType(j);
            }

            GameplayManager.Instance.boxes.Add(box);
        }

        Helper.WaitForTransition(OnNext);
    }

    public void OnNext()
    {
        foreach (Box box in GameplayManager.Instance.boxes)
        {
            bool isReady;
            float nextPosX;

            int index = GameplayManager.Instance.boxes.IndexOf(box);

            if (index == 0)
            {
                isReady = true;

                if (box.rectTransform.anchoredPosition.x > 0) return;

                if (box.rectTransform.anchoredPosition.x > -boxDistance)
                {
                    nextPosX = 0;
                }
                else
                {
                    nextPosX = index * -boxDistance;
                }
            }
            else
            {
                isReady = false;

                nextPosX = index * -boxDistance;
            }

            box.MoveToPosition(new Vector2(nextPosX, box.rectTransform.anchoredPosition.y), isReady);
        }

        BoosterManager.Instance.BlockBooster(EBoosterType.AddBox, false);

        SoundManager.Instance.PlaySound(KeySound.BoxMove);

        GameplayManager.Instance.IsAddingBox = false;

        BoosterManager.Instance.boosterBox.CheckToggleBG();
    }

    public void AddOneMoreBox()
    {
        List<Box> boxes = GameplayManager.Instance.boxes;

        if (boxes.Count <= 1) return;

        boxes[0].MoveToPosition(new Vector2(boxes[0].rectTransform.anchoredPosition.x + boxDistance * 0.4f, boxes[0].rectTransform.anchoredPosition.y), true);
        boxes[1].MoveToPosition(new Vector2(boxes[1].rectTransform.anchoredPosition.x + boxDistance * 0.6f, boxes[1].rectTransform.anchoredPosition.y), true);

        boxes[1].IsReady = true;

        GameplayManager.Instance.IsAddingBox = true;
    }
}
