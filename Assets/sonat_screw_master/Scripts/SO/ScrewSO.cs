using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ScrewSO", fileName = "ScrewSO")]
public class ScrewSO : ScriptableObject
{
    public int id;

    public ShopItemCollection infoShop;

    public string nameSkinBomb;
    public string nameSkinIce;

    public List<ScrewSprite> screwSprites;

    public ScrewSprite GetScrewSprite(EHoleType type, EScrewColor color)
    {
        ScrewSprite screw;

        screw = screwSprites.Find(x => x.type == type && x.color == color);

        if (screw == null)
        {
            Debug.LogError($"not found type {type}, color {color}");
        }

        return screw;
    }

    public bool IsUnlocked()
    {
        return DataManager.Instance.playerData.saveLevelData.currentLevel >= infoShop.levelUnlock;
    }
}

[Serializable]
public class ScrewSprite
{
    public Sprite headSprite;
    public Sprite tailSprite;
    public EHoleType type;
    public EScrewColor color;
}
