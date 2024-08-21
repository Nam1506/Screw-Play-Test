using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : SingletonBase<DataManager>
{
    private const string PLAYERDATA_KEY = "PlayerData";
    private const string LEVEL_FILE_PATH = "Levels/";

    public PlayerData playerData = new PlayerData();
    public LevelData levelData = new LevelData();
    //public PreMoveData preMoveData = new PreMoveData();
    public List<string> botNamesRaw;
    public List<Sprite> avtLeaderboardDatas;

    public event EventHandler OnUpdateLiveAction;
    public event EventHandler OnUpdateCoinAction;

    private DateTime unlimitedTimeEnd;

    public bool IsUnlimitedLives
    {
        get
        {
            return unlimitedTimeEnd > DateTime.Now;
        }
    }

    public PlayerData SavePlayerData
    {
        get
        {
            if (PlayerPrefs.HasKey(PLAYERDATA_KEY))
            {
                return JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(PLAYERDATA_KEY));
            }
            else
            {
                var newDataPlayer = InitPlayerData();

                SavePlayerData = newDataPlayer;

                return newDataPlayer;
            }
        }
        set
        {
            if (value == null)
                PlayerPrefs.DeleteKey(PLAYERDATA_KEY);
            else
            {
                PlayerPrefs.SetString(PLAYERDATA_KEY, JsonConvert.SerializeObject(value));
            }

        }
    }

    private PlayerData InitPlayerData()
    {
        var newDataPlayer = new PlayerData()
        {
            saveLevelData = new Level()
            {
                currentLevel = 1,
                playCount = 0,
            },

            coins = GameDefine.DEFAULT_COIN,
            lives = new Lives
            {
                amount = GameDefine.MAX_LIVES,
            },
            boosters = new List<Booster>()
            {
                new Booster(EBoosterType.AddBox, 1, ""),
                new Booster(EBoosterType.Hammer, 1, ""),
                new Booster(EBoosterType.AddHole, 1, ""),
            },
            settings = new Settings()
            {
                isMusicOn = true,
                isSoundOn = true,
                isVibrateOn = true,
                isNotiOn = true,
            }
        };

        return newDataPlayer;
    }

    public bool LoadLevel(int level)
    {
        if (level <= 0)
        {
            level = 1;
        }

        TextAsset textAsset = null;

#if UNITY_EDITOR

        string filePath = LEVEL_FILE_PATH + level;

        textAsset = Resources.Load<TextAsset>(filePath);
#else

        var fileName = $"{LEVEL_FILE_PATH}/{level}.json";
        var fileName1 = $"{LEVEL_FILE_PATH}/{level}.txt";

        if (File.Exists(fileName))
        {
            textAsset = new TextAsset(File.ReadAllText(fileName));
        }
        else if (File.Exists(fileName1))
        {
            textAsset = new TextAsset(File.ReadAllText(fileName1));
        }
#endif

        if (textAsset == null)
        {
            PopupManager.Instance.ShowNotiAlert("Not have level: " + level);

            return false;
        }

        string pathTarget = textAsset.ToString();
        LevelData data = JsonConvert.DeserializeObject<LevelData>(pathTarget);

        levelData = data;

        Debug.Log("Load Successfully");

        return true;
    }

    public bool ExistLevel(int level)
    {
        string filePath = LEVEL_FILE_PATH + level;

        TextAsset textAsset = Resources.Load<TextAsset>(filePath);

        if (textAsset == null)
        {
            return false;
        }

        return true;
    }

    public EDifficulty GetLevelDiff(int level)
    {
        if (ExistLevel(level))
        {
            //if (level < 16) return EDifficulty.Normal;
            //else if (level == 16) return EDifficulty.Hard;
            //else
            //{
            //    if (level == 19) return EDifficulty.Hard;
            //    else if(level % 10 == 5) return EDifficulty.Hard;
            //    else if (level % 10 == 9) return EDifficulty.Super_Hard;
            //    else return EDifficulty.Normal;
            //}

            string filePath = LEVEL_FILE_PATH + level;

            TextAsset textAsset = Resources.Load<TextAsset>(filePath);

            LevelData data = JsonConvert.DeserializeObject<LevelData>(textAsset.ToString());

            return data.eDifficulty;
        }
        else
        {
            int partLevel = GameDefine.MAX_LEVEL / 2;
            int randomLevel = partLevel + level % partLevel;

            Debug.Log(randomLevel);

            return GetLevelDiff(randomLevel);
        }
    }

    public EDifficulty GetCurrentLevelDiff()
    {
        return GetLevelDiff(playerData.saveLevelData.currentLevel);
    }

    public EDifficulty GetPreLevelDiff()
    {
        return GetLevelDiff(playerData.saveLevelData.currentLevel - 1);
    }

    public Sprite GetMyAvt()
    {
        var myDataIndex = LeaderboardManager.saveLeaderboard.players.FindIndex(x => x.name == "YOU");

        var myData = LeaderboardManager.saveLeaderboard.players[myDataIndex];

        return avtLeaderboardDatas[myData.avtId];
    }

    public void Win()
    {
        //foreach (var shapeData in levelData.shapes)
        //{
        //    foreach (var hole in shapeData.holes)
        //    {
        //        playerData.AddCollectHoleData(hole.eScrewColor, 1);
        //    }
        //}

        UIManager.Instance.UIHome.canAnimBox = true;

        playerData.saveLevelData.currentLevel++;
        playerData.saveLevelData.playCount = 0;
        playerData.preLevel = playerData.nextLevel;
        playerData.coins += GameDefine.COIN_WIN_DEFAULT;

        Save();

        int levelComplete = playerData.saveLevelData.currentLevel - 1;

        SonatTracking.LogLevelEnd(levelComplete,
            playerData.totalUseBooster,
             true,
            ((int)GameplayManager.Instance.timePlay),
            IsFirstPlay(),
            "",
            0,
            0, //GameplayManager.Instance.score,
            GameplayManager.Instance.numContinue == 0 ? "" : "revive",
            GameplayManager.Instance.numContinue
            );

        SonatTracking.LogCompleteLevelUA(levelComplete);
        SonatTracking.LogLevelUp(levelComplete + 1);
        SonatTracking.LogEarnCurrency("coin", "currency", GameDefine.COIN_WIN_DEFAULT, "Ingame", "non_iap", levelComplete);
    }

    public void Save()
    {
        SavePlayerData = playerData;
    }

    private void Awake()
    {
        playerData = SavePlayerData;

        CountLevel();

        unlimitedTimeEnd = Helper.StringToDate(playerData.lives.unlimitedTimeEnd);

        LoadConfig();
    }

    private void Start()
    {
        BoosterManager.Instance.LoadInfo();
    }

    private void LoadConfig()
    {
        botNamesRaw = new List<string>();
        botNamesRaw = JsonConvert.DeserializeObject<DataManager>(Helper.LoadTextFileFromResource("gameConfig.json")).botNamesRaw;

        RaceEvent.Init();
        LeaderboardManager.Load();
        LevelChestManager.Load();

        PopupManager.Instance.LoadConfig();

        SkinSystem.Load();
    }

    private void CountLevel()
    {
        UnityEngine.Object[] assets = Resources.LoadAll("Levels");

        GameDefine.MAX_LEVEL = assets.Length;
    }

    public bool IsFirstPlay()
    {
        return playerData.saveLevelData.playCount == 1;
    }

    public void StartNewLevel()
    {
        playerData.totalUseBooster = 0;
        playerData.saveLevelData.playCount++;
        Save();
    }

    public void UseLive()
    {
        if (IsUnlimitedLives) return;

        Debug.Log("Use Life");

        if (playerData.lives.amount == 0)
            return;

        playerData.lives.amount--;

        if (playerData.lives.amount + 1 == GameDefine.MAX_LIVES)
        {
            playerData.lives.nextLiveTime = Helper.AddDuration(DateTime.Now, GameDefine.RESTORELIVES_DURATION).DateTimeToString();
        }

        Save();

        OnUpdateLiveAction?.Invoke(this, EventArgs.Empty);

        SonatTracking.LogSpendCurrency("live", "other_source", 1, "Ingame");
    }

    public void SaveLive(int currentLives, DateTime nextLiveTime, DateTime lastAddedTime, DateTime unlimitedTimeEnd = default)
    {
        playerData.lives.amount = currentLives;
        playerData.lives.nextLiveTime = nextLiveTime.DateTimeToString();
        playerData.lives.lastAddedTime = lastAddedTime.DateTimeToString();

        if (unlimitedTimeEnd != default)
        {
            playerData.lives.unlimitedTimeEnd = unlimitedTimeEnd.DateTimeToString();
        }

        Save();
    }

    public void AddLives(int amount, bool isSetText = true)
    {
        playerData.lives.amount += amount;
        playerData.lives.amount = Mathf.Min(playerData.lives.amount, GameDefine.MAX_LIVES);

        Save();

        if (isSetText)
        {
            MatchLives();
        }

        SonatTracking.LogEarnVirtualCurrencyUA();
    }

    public void SetUnlimitedLives(int second, bool isSetText = true)
    {
        DateTime unlimitedTimeEnd;

        if (IsUnlimitedLives)
        {
            unlimitedTimeEnd = Helper.AddDuration(Helper.StringToDate(playerData.lives.unlimitedTimeEnd), second);
        }
        else
        {
            unlimitedTimeEnd = Helper.AddDuration(DateTime.Now, second);
        }

        playerData.lives.unlimitedTimeEnd = unlimitedTimeEnd.DateTimeToString();
        playerData.lives.amount = GameDefine.MAX_LIVES;

        Save();

        if (isSetText)
        {
            MatchLives();
        }
    }

    public void ChangeCoins(int amount, bool isSetText = true, bool isTrackingUA = true)
    {
        Debug.Log("namtt change");

        playerData.coins += amount;
        playerData.coins = Mathf.Max(0, playerData.coins);

        Save();

        if (isSetText)
        {
            MatchCoins();
        }

        if (amount > 0 && isTrackingUA)
        {
            SonatTracking.LogEarnVirtualCurrencyUA();
        }
    }

    public void MatchCoins()
    {
        OnUpdateCoinAction?.Invoke(this, EventArgs.Empty);
    }

    public void MatchLives()
    {
        unlimitedTimeEnd = Helper.StringToDate(playerData.lives.unlimitedTimeEnd);

        OnUpdateLiveAction?.Invoke(this, EventArgs.Empty);
    }

    public void FakeReset()
    {
        UseLive();
    }

    public void RevertFakeReset()
    {
        AddLives(1);

        GameplayManager.Instance.IsLose = false;
    }

    public void RestorePack(ShopItemKey key)
    {
        playerData.restoredPacks.Add((int)key);
        Save();
    }

    public bool IsFirstBuy()
    {
        return playerData.buyCount == 0;
    }

    public void BuyItem()
    {
        playerData.buyCount++;
        Save();
    }

    public void ToggleNoti()
    {


        Save();
    }

    public bool IsNoAds24hExpired()
    {
        Debug.Log("abc timeBuyNoAds24h: " + playerData.timeBuyNoAds24h);

        if (string.IsNullOrEmpty(playerData.timeBuyNoAds24h)) return false;

        //DateTime timeToExpired = Helper.StringToDate(playerData.timeBuyNoAds24h).AddDays(1);

        DateTime timeBuy = playerData.timeBuyNoAds24h.StringToDate();

        if (timeBuy - DateTime.Now <= TimeSpan.FromMilliseconds(2d))
        {
            playerData.timeBuyNoAds24h = DateTime.Now.DateTimeToString();
            Save();
        }

        DateTime timeToExpired = timeBuy.AddDays(1);

        if (DateTime.Now > timeToExpired)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetPackTimer()
    {
        playerData.timeShowNoAds24h = null;
        playerData.timeShowValueBundle = null;
        playerData.timeShowBestValueBundle = null;

        Save();
    }

    public void ClearCollectScrewData()
    {
        playerData.collectScrewData.Clear();

        Save();
    }
}

[Serializable]
public class PlayerData
{
    public int preLevel = 0;
    public Level saveLevelData = new();
    public int nextLevel = 1;
    public int realLevel = 1;

    public int coins;

    public int avatarId = 6;

    public List<Booster> boosters = new List<Booster>();
    public Lives lives = new Lives();
    public List<int> restoredPacks = new List<int>();
    public Settings settings = new Settings();

    public int totalUseBooster = 0;
    public int buyCount = 0;

    public int numCompleteRewardAds = 0;
    public int numShowAds = 0;

    public string timeBuyNoAds24h;

    public string timeShowNoAds24h;
    public string timeShowValueBundle;
    public string timeShowBestValueBundle;
    public string timeEndBestValueBundle;
    public string timeShowPlusOffer;

    public string timeBuyExclusive;
    public string timeBuyWeekendSale;
    public string timeBuyPlusOffer;

    public List<CollectScrewData> collectScrewData = new();

    public void AddCollectHoleData(EScrewColor eColor, int num)
    {
        var findIndex = collectScrewData.FindIndex(x => x.eScrewColor == eColor);

        if (findIndex != -1)
        {
            collectScrewData[findIndex].num += num;
            return;
        }

        collectScrewData.Add(new CollectScrewData
        {
            eScrewColor = eColor,
            num = num
        });

        DataManager.Instance.Save();
    }
}

[Serializable]
public class Level
{
    public int currentLevel;
    public int playCount;
}

[Serializable]
public class Lives
{
    public int amount;
    public string nextLiveTime;
    public string lastAddedTime;
    public string unlimitedTimeEnd;
}

[Serializable]
public class Booster
{
    public string type;
    public int amount;
    public string unlimitedTimeEnd;

    public Booster(EBoosterType type, int amount, string unlimitedTimeEnd)
    {
        this.type = type.ToString();
        this.amount = amount;
        this.unlimitedTimeEnd = unlimitedTimeEnd;
    }
}


[Serializable]
public class Settings
{
    public bool isSoundOn;
    public bool isMusicOn;
    public bool isVibrateOn;
    public bool isNotiOn;
}

//[Serializable]
//public class PreMoveData
//{
//    public int screwId;
//}

public class LevelData
{
    public int level;
    public EDifficulty eDifficulty;
    public int holeQueue;

    public List<BoxData> boxes = new();
    public List<ShapeData> shapes = new();
    public List<ObstacleData> obstacles = new();
}

[Serializable]
public class BoxData
{
    public EScrewColor eColor;

    public List<BoxHoleData> holes = new();
}

[Serializable]
public class HoleData
{
    public int identify = 0;

    public EHoleType eType;

    public EScrewColor eScrewColor;

    public Vector3 localPos;
}

[Serializable]
public class BoxHoleData
{
    public EHoleType eType;
}

[Serializable]
public class ShapeData
{
    public int identify;

    public int id = 0;
    public int layer = 0;

    public float localRotationZ;
    public float localScaleX;
    public float localScaleY;

    public EColorShape eColorShape;

    public Vector3 worldPos;

    public List<HoleData> holes = new();
}

[Serializable]
public class ObstacleData
{
    public EObstacleType eObstacleType;

    public List<int> identifies = new();

    public int ValueBoom;

    public bool isOpen;
}

[Serializable]
public class CollectScrewData
{
    public EScrewColor eScrewColor;
    public int num;
}

[Serializable]
public class ShopItemCollection
{
    public float price;
    public int levelUnlock;
    public Sprite icon;
}