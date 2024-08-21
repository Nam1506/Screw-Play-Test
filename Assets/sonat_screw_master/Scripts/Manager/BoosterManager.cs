using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoosterManager : SingletonBase<BoosterManager>
{
    public BoosterSO boosterSO;

    public BoosterHammer boosterHammer;
    public BoosterHole boosterHole;
    public BoosterBox boosterBox;

    public List<BoosterBase> boosterBases = new();

    public bool IsUsingHammer { get; set; }

    public bool CanSetText => GameManager.Instance.GameState != GameState.Ingame;

    public void LoadData()
    {
        foreach (var booster in boosterBases)
        {
            booster.LoadData();
        }
    }

    public void LoadInfo()
    {
        foreach (var booster in boosterBases)
        {
            booster.LoadInfo();
        }
    }

    public void MatchValue()
    {
        foreach (var booster in boosterBases)
        {
            booster.MatchValue();
        }
    }

    public BoosterInfo GetBoosterInfo(EBoosterType type)
    {
        return boosterSO.boosters.Find(e => e.type == type);
    }

    public void BlockBooster(EBoosterType type, bool isBlock)
    {
        boosterBases.Find(e => e.eBoosterType == type).IsBlocking = isBlock;
    }

    public BoosterBase GetBooster(EBoosterType type)
    {
        return boosterBases.Find(e => e.eBoosterType == type);
    }

    public void AddBooster(EBoosterType type, int value, bool isLog = true, bool isPreBooster = false)
    {
        foreach (var booster in boosterBases)
        {
            if (booster.eBoosterType == type)
            {
                Debug.Log("Add " + type);
                booster.Add(value, isLog, isPreBooster, CanSetText);
                return;
            }
        }
    }

    public void CheckCancelHammer()
    {
        if (IsUsingHammer)
        {
            UIManager.Instance.UnPrepareHammer();
            IsUsingHammer = false;
        }
    }

    public bool IsLevelUnlockBooster()
    {
        int currentLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

        foreach (var booster in boosterBases)
        {
            if (booster.levelUnlock == currentLevel)
                return true;
        }

        return false;
    }

    public void ForceUse(EBoosterType boosterType, bool isFree = false)
    {
        switch (boosterType)
        {
            case EBoosterType.AddHole:
                boosterHole.ForceUse(isFree);
                break;
            case EBoosterType.AddBox:
                boosterBox.ForceUse(isFree);
                break;
            case EBoosterType.Hammer:
                boosterHammer.MatchValue();
                boosterHammer.Prepare();
                break;
        }
    }
}

public enum EBoosterType
{
    None = -1,
    AddHole = 0,
    Hammer = 1,
    AddBox = 2
}

[Serializable]
public class BoosterNotice
{
    public GameObject noticeRed;
    public GameObject noticePlus;
    public TMP_Text amount;

    public void SetText(int value)
    {
        amount.text = value.ToString();
    }
}

[Serializable]
public class BoosterInfoRef
{
    public GameObject buyInfo;
    public GameObject unlockInfo;
}