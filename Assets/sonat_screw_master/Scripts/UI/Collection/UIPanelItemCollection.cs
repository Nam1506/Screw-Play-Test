using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelItemCollection : MonoBehaviour
{
    [SerializeField] private UIItemCollection itemPrefab;

    [SerializeField] private GridLayoutGroup grid;

    private List<UIItemCollection> items = new();

    private UICollection.ETab curTab;

    private const int numFake = 10;

    public void Reload()
    {
        Select(curTab);
    }

    public void Select(UICollection.ETab eTab)
    {
        Clear();

        curTab = eTab;

        switch (eTab)
        {
            case UICollection.ETab.Box:
                SelectBox();
                break;

            case UICollection.ETab.Screw:
                SelectScrew();
                break;
        }

        RecalculateGridSize();
    }

    private void RecalculateGridSize()
    {
        int index = 0;

        if (items.Count % 3 == 0)
        {
            index = (items.Count / 3);
        }
        else
        {
            index = (items.Count / 3) + 1;
        }

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, index * (grid.cellSize.y + grid.spacing.y));
    }

    private void SelectBox()
    {
        var listBox = BoxController.Instance.BoxData.boxes;

        for (int i = 0; i < listBox.Count; i++)
        {
            var boxData = listBox[i];

            var item = GetItem(i);

            var id = boxData.id;

            UIItemCollection.EState eState;

            if (SkinSystem.IsCollectedSkin(SkinSystem.ESkin.Box, id))
            {
                if (SkinSystem.IsEquippedSkin(SkinSystem.ESkin.Box, id))
                {
                    eState = UIItemCollection.EState.Equipped;
                }
                else
                {
                    eState = UIItemCollection.EState.Collected;
                }
            }
            else
            {
                if (SkinSystem.IsUnlockedSkin(SkinSystem.ESkin.Box, id))
                {
                    eState = UIItemCollection.EState.Unlocked;
                }
                else
                {
                    eState = UIItemCollection.EState.Locked;
                }
            }

            item.SetData(new UIItemCollection.DataItem(SkinSystem.ESkin.Box, boxData.id, boxData.infoShop.icon, boxData.infoShop.price, eState));
        }

        int numBox = listBox.Count;

        while (numBox < numFake)
        {
            var item = GetItem(numBox++);

            item.SetFake();
        }

        SelectItem(SkinSystem.data.boxCollection.curSkin);
    }

    private void SelectScrew()
    {
        var listScrew = LevelGenerator.Instance.ScrewData.screws;

        for (int i = 0; i < listScrew.Count; i++)
        {
            var screwData = listScrew[i];

            var item = GetItem(i);

            var id = screwData.id;

            UIItemCollection.EState eState;

            if (SkinSystem.IsCollectedSkin(SkinSystem.ESkin.Screw, id))
            {
                if (SkinSystem.IsEquippedSkin(SkinSystem.ESkin.Screw, id))
                {
                    eState = UIItemCollection.EState.Equipped;
                }
                else
                {
                    eState = UIItemCollection.EState.Collected;
                }
            }
            else
            {
                if (SkinSystem.IsUnlockedSkin(SkinSystem.ESkin.Screw, id))
                {
                    eState = UIItemCollection.EState.Unlocked;
                }
                else
                {
                    eState = UIItemCollection.EState.Locked;
                }
            }

            item.SetData(new UIItemCollection.DataItem(SkinSystem.ESkin.Screw, screwData.id, screwData.infoShop.icon, screwData.infoShop.price, eState));
        }

        int numScrew = listScrew.Count;

        while (numScrew < numFake)
        {
            var item = GetItem(numScrew++);

            item.SetFake();
        }

        SelectItem(SkinSystem.data.screwCollection.curSkin);
    }

    public void SelectItem(int id)
    {
        foreach (var item in items)
        {
            if (item.Data == null) continue;

            item.Select(item.Data.id == id);
        }

        UIManager.Instance.UICollection.SetPreview(items[id]);
    }

    private UIItemCollection GetItem(int index)
    {
        if (index < items.Count)
        {
            items[index].gameObject.SetActive(true);
            return items[index];
        }

        var item = Instantiate(itemPrefab, transform);

        items.Add(item);

        return item;
    }

    private void Clear()
    {
        foreach (var item in items)
        {
            item.gameObject.SetActive(false);
        }
    }
}
