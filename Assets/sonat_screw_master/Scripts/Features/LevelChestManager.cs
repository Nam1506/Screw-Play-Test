using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class LevelChestManager
{
    public class LevelChest
    {
        public int point;

        public LevelChest(int point)
        {
            this.point = point;
        }
    }

    public static readonly string KEY_LEVEL_CHEST = "LEVEL_CHEST_DATA";
    public static readonly bool IS_DISABLED = false;
    public static readonly float END_POINT = 10;

    public static LevelChest data;

    private static LevelChest Data
    {
        get
        {
            if (PlayerPrefs.HasKey(KEY_LEVEL_CHEST))
            {
                return JsonConvert.DeserializeObject<LevelChest>(PlayerPrefs.GetString(KEY_LEVEL_CHEST));
            }
            else
            {
                var newLevelChest = new LevelChest(0);

                PlayerPrefs.SetString(KEY_LEVEL_CHEST, JsonConvert.SerializeObject(newLevelChest));

                return newLevelChest;
            }

        }
        set
        {
            PlayerPrefs.SetString(KEY_LEVEL_CHEST, JsonConvert.SerializeObject(value));
        }
    }

    public static void Load()
    {
        if (IS_DISABLED) return;

        data = Data;
    }

    public static void Save()
    {
        if (IS_DISABLED) return;

        Data = data;
    }

    public static void OnWinLevel()
    {
        if (IS_DISABLED) return;

        if (DataManager.Instance.playerData.saveLevelData.currentLevel < GameDefine.LEVEL_GO_HOME) return;

        data.point++;

        data.point = Mathf.Min(data.point, ((int)END_POINT));

        Save();
    }

    public static void OnSuccess()
    {
        if (IS_DISABLED) return;

        data.point = 0;

        Save();
    }

    public static bool IsCompleted()
    {
        return data.point == END_POINT;
    }

    public static void SetCheat(int value)
    {
        data.point = Mathf.Min(value, ((int)END_POINT));

        Save();
    }

    public static  List<Reward> GenerateRewards()
    {
        var rewards = new List<Reward>
        {
            new Reward(RewardID.coin, RewardType.currency, UnityEngine.Random.Range(10, 16)),

            new Reward(GetRandomRewardBooster(), RewardType.booster, 1)
        };

        return rewards;
    }

    private static RewardID GetRandomRewardBooster()
    {
        System.Random rand = new System.Random();

        double randomValue = rand.NextDouble();

        if (randomValue < 0.40)
        {
            return RewardID.addHole;
        }
        else if (randomValue < 0.70)
        {
            return RewardID.hammer;
        }
        else
        {
            return RewardID.addBox;
        }
    }
}
