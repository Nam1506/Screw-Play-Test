using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BoxSO", fileName = "BoxSO")]
public class BoxSO : ScriptableObject
{
    public int id;
    public Vector3 offset;

    public ShopItemCollection infoShop;

    public List<BoxSprite> boxSprites = new();
    public List<Sprite> glassSprites = new();

    public Sprite GetSprite(EScrewColor color, int holeCount)
    {
        BoxSprite boxSprite = boxSprites.Find(x => x.color == color);

        if (boxSprite == null)
        {
            Debug.LogError("Not found box sprite color: " + color.ToString());
            return null;
        }
        else
        {
            return boxSprite.sprites[holeCount - 2];
        }
    }

    public Sprite GetGlassSprite(int holeCount)
    {
        return glassSprites[holeCount - 2];
    }

    public Vector3 GetOffset()
    {
        return offset;
    }

    public bool IsUnlocked()
    {
        return DataManager.Instance.playerData.saveLevelData.currentLevel >= infoShop.levelUnlock;
    }
}

[Serializable]
public class BoxSprite
{
    public List<Sprite> sprites;
    public EScrewColor color;
}
