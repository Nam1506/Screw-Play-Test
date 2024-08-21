using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RaceEvent
{
    public static RaceData raceData;

    public static Action OnStartEvent;
    public static Action OnStartRace;
    public static Action OnFinishEvent;
    public static Action OnFinishRace;

    public const int TIME_EVENT = 72;
    public const int NUM_OTHER_PLAYER = 4;
    public const int RACE_RANGE = 15;
    public const int MY_ID = 0;
    public const int NEXT_TIME_EVENT = 4;

    public static int levelStartEvent = 11;
    //public static bool forceFinishEvent;

    public static RaceData RaceData
    {
        get
        {
            if (PlayerPrefs.HasKey("RaceData"))
                return JsonConvert.DeserializeObject<RaceData>(PlayerPrefs.GetString("RaceData"));
            else
            {
                RaceData defaultData = new RaceData();

                defaultData.isAvailable = false;
                defaultData.hasJoined = false;
                defaultData.isForceShowStart = false;
                defaultData.isForceShowFinish = false;

                defaultData.nextLevelForce = 0;
                defaultData.winLevelTarget = 26;
                defaultData.timeStart = null;

                return defaultData;
            }
        }
        set
        {
            PlayerPrefs.SetString("RaceData", JsonConvert.SerializeObject(value));
        }
    }

    public static void Init()
    {
        raceData = RaceData;

        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        Debug.Log("Init Race: " + level);

        if (!PlayerPrefs.HasKey("RaceData"))
        {
            Debug.Log("Init Race: " + level);
           
            Save();
        }

        CheckEventFinish();
    }

    public static void Save()
    {
        RaceData = raceData;
    }

    public static void OnWinLevel()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (level < levelStartEvent) return;

        if (level == levelStartEvent)
        {
            StartEvent();
            return;
        }

        if (!raceData.isAvailable) return;

        if (CheckRacing())
        {
            if (level >= raceData.winLevelTarget)
            {
                AddRacerFinish(0);

                raceData.isForceShowFinish = true;
            }

            AddPointForOtherPlayers();

            CheckRacing();
        }
        else if (CanForceShowStart())
        {
            raceData.isForceShowStart = true;
            Save();
        }
    }

    public static void StartEvent()
    {
        if (raceData.isAvailable) return;

        Debug.Log("Start event");

        raceData.isAvailable = true;
        raceData.hasJoined = false;
        raceData.isForceShowFinish = false;
        raceData.isForceShowStart = true;
        raceData.timeStart = DateTime.Now.DateTimeToString();
        raceData.nextTimeEvent = DateTime.Now.Date.AddDays(NEXT_TIME_EVENT).DateTimeToString();

        //forceFinishEvent = false;

        Save();

        OnStartEvent?.Invoke();
    }

    public static void RelaunchEvent()
    {
        raceData.isForceShowStart = false;
        raceData.isForceShowFinish = false;
        raceData.winLevelTarget = DataManager.Instance.playerData.saveLevelData.currentLevel + RACE_RANGE;
        raceData.hasJoined = false;

        ClearRank();
        JoinRace();
    }

    public static void JoinRace()
    {
        if (raceData.hasJoined) return;

        Debug.Log("Join race");

        raceData.hasJoined = true;

        if (raceData.raceMembers.Count == 0)
        {
            CreateRaceMember();
        }

        Save();

        OnStartRace?.Invoke();
    }

    public static void OnNoJoin()
    {
        raceData.isForceShowStart = false;
        raceData.hasJoined = false;
        raceData.nextLevelForce = DataManager.Instance.playerData.saveLevelData.currentLevel + RACE_RANGE;

        Save();
    }

    private static bool CanForceShowStart()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;
        if (level < raceData.nextLevelForce) return false;
        if (level >= levelStartEvent) return true;
        return false;
    }

    public static void CreateRaceMember()
    {
        var botNames = Helper.GetRandomElemntsInList(DataManager.Instance.botNamesRaw, 4);
        int startScore = raceData.winLevelTarget - RACE_RANGE;

        var ids = Helper.GetRandomElemntsInList(Enumerable.Range(1, 9).ToList(), NUM_OTHER_PLAYER);

        for (int i = 0; i < NUM_OTHER_PLAYER; i++)
        {
            RaceMember raceMember = RaceMember.CreateRandomOther(botNames[i], ids[i], startScore);
            raceData.raceMembers.Add(raceMember);
        }

        Save();
    }

    private static bool IsEventFinish()
    {
        if (!raceData.isAvailable) return true;

        DateTime currentTime = DateTime.Now;
        DateTime timeEnd = Helper.StringToDate(raceData.timeStart).AddSeconds(TIME_EVENT * GameConfig.HOUR_TO_SEC);

        if (currentTime > timeEnd)
        {
            raceData.isAvailable = false;
            Save();
            return true;
        }

        return false;
    }

    public static void CheckEventFinish()
    {
        if (!string.IsNullOrEmpty(raceData.nextTimeEvent))
        {
            if (!raceData.isAvailable && DateTime.Now.Date >= Helper.StringToDate(raceData.nextTimeEvent).Date)
            {
                StartEvent();
            }
        }

        if (!raceData.isAvailable || DataManager.Instance.playerData.saveLevelData.currentLevel < levelStartEvent) return;

        if (!raceData.hasJoined)
        {
            if (IsEventFinish())
            {
                raceData.isForceShowStart = false;
                FinishEvent(false);
                OnFinishEvent?.Invoke();
            }
        }
        else
        {
            CheckRacing();
        }
    }

    public static bool CheckRaceFinish()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (level < levelStartEvent) return false;
        if (level == levelStartEvent && string.IsNullOrEmpty(raceData.timeStart)) return false;
        if (level >= raceData.winLevelTarget)
        {
            raceData.isForceShowFinish = true;
            Save();
            return true;
        }
        else
        {
            DateTime currentTime = DateTime.Now;
            DateTime timeEnd = Helper.StringToDate(raceData.timeStart).AddSeconds(TIME_EVENT * GameConfig.HOUR_TO_SEC);

            if (currentTime > timeEnd)
            {
                raceData.isForceShowFinish = true;
                Save();
                return true;
            }
            else if (raceData.ranks.Count >= 3 && !raceData.ranks.Contains(MY_ID))
            {
                raceData.isForceShowFinish = true;
                Save();
                return true;
            }
        }

        raceData.isForceShowFinish = false;
        Save();

        return false;
    }

    public static bool CheckCanStartRacing()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (raceData.hasJoined || !raceData.isAvailable)
            return false;

        if (raceData.isForceShowStart)
            return true;

        if (!raceData.hasJoined && level >= levelStartEvent)
            return true;

        return false;
    }

    public static bool CheckRacing()
    {
        if (!raceData.isAvailable || !raceData.hasJoined) return false;

        DateTime currentTime = DateTime.Now;
        DateTime timeEnd = Helper.StringToDate(raceData.timeStart).AddSeconds(TIME_EVENT * GameConfig.HOUR_TO_SEC);

        if (currentTime > timeEnd)
        {
            FinishEvent();
        }
        else if (raceData.ranks.Count >= 3 && !raceData.ranks.Contains(MY_ID))
        {
            raceData.isForceShowFinish = true;
            Save();
        }

        return true;
    }

    public static void AddPointForOtherPlayers()
    {
        for (int i = 0; i < raceData.raceMembers.Count; i++)
        {
            if (raceData.ranks.Contains(raceData.raceMembers[i].id)) continue;

            int pointAdd = UnityEngine.Random.Range(0, 3);

            raceData.raceMembers[i].level += pointAdd;

            if (raceData.raceMembers[i].level >= raceData.winLevelTarget)
            {
                raceData.raceMembers[i].level = raceData.winLevelTarget;
                AddRacerFinish(raceData.raceMembers[i].id);
            }

            Save();
        }
    }

    /// <summary>
    /// noti = true if user has joined event.
    /// </summary>
    /// <param name="noti"></param>
    public static void FinishEvent(bool noti = true)
    {
        raceData.isAvailable = false;

        if (noti)
        {
            raceData.isForceShowFinish = true;
            //forceFinishEvent = true;
        }
        else
        {
            raceData.isForceShowFinish = false;
            //forceFinishEvent = false;
        }

        Save();
    }

    public static List<Reward> GetRewards()
    {
        List<Reward> rewards;

        int rank = GetMyRank();

        switch (rank)
        {
            case 1:
                rewards = new List<Reward>()
                {
                    new Reward(RewardID.coin, RewardType.currency, 100),
                    new Reward(RewardID.unlimitedLives, RewardType.currency, 900f),
                    new Reward(RewardID.hammer, RewardType.booster, 1)
                };
                break;
            case 2:
                rewards = new List<Reward>()
                {
                    new Reward(RewardID.coin, RewardType.currency, 75),
                    new Reward(RewardID.addBox, RewardType.booster, 1),
                };
                break;
            case 3:
                rewards = new List<Reward>()
                {
                    new Reward(RewardID.coin, RewardType.currency, 50),
                };
                break;
            default:
                rewards = new();
                break;
        }

        return rewards;
    }

    public static int GetMyRank()
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (level < raceData.winLevelTarget) return NUM_OTHER_PLAYER + 2;

        int rank = 1;

        if (raceData.ranks.Contains(MY_ID))
            return raceData.ranks.IndexOf(MY_ID) + 1;

        rank += raceData.ranks.Count;

        return rank;
    }

    public static void AddRacerFinish(int id)
    {
        raceData.ranks.Add(id);
        Save();
    }

    public static void OnClaimReward()
    {
        raceData.isForceShowFinish = false;
        raceData.hasJoined = false;

        if (IsEventFinish())
        {
            raceData.isForceShowStart = false;
            OnFinishRace?.Invoke();
            OnFinishEvent?.Invoke();
        }
        else
        {
            raceData.isForceShowStart = true;
            OnFinishRace?.Invoke();
        }

        raceData.isForceShowFinish = false;

        //forceFinishEvent = false;

        Save();
    }

    public static void OnContinue()
    {
        raceData.isForceShowFinish = false;
        raceData.hasJoined = false;
        //forceFinishEvent = false;

        if (IsEventFinish())
        {
            raceData.isForceShowStart = false;
            Save();
            OnFinishRace?.Invoke();
            OnFinishEvent?.Invoke();
        }
        else
        {
            raceData.isForceShowStart = true;
            Save();
            OnFinishRace?.Invoke();
        }
    }

    private static void ClearRank()
    {
        raceData.ranks.Clear();
        raceData.raceMembers.Clear();

        Save();
    }
}

public class RaceData
{
    public bool isAvailable;
    public bool hasJoined;
    public bool isForceShowStart;
    public bool isForceShowFinish;

    public int winLevelTarget;
    public int nextLevelForce;

    public string timeStart;
    public string nextTimeEvent;

    public List<RaceMember> raceMembers = new();
    public List<int> ranks = new();
}

public class RaceMember
{
    public string name;
    public int level;
    public int id;
    public int avatarId;


    public static RaceMember CreateRandomOther(string name, int id, int level)
    {
        RaceMember res = new RaceMember();
        res.name = name;
        res.level = level;
        res.id = id;
        res.avatarId = id;
        return res;
    }

    public static RaceMember CreateMySelf()
    {
        RaceMember mySelf = new RaceMember();
        mySelf.name = "You";
        mySelf.level = DataManager.Instance.playerData.saveLevelData.currentLevel;
        mySelf.id = RaceEvent.MY_ID;
        mySelf.avatarId = DataManager.Instance.playerData.avatarId;
        return mySelf;
    }
}
