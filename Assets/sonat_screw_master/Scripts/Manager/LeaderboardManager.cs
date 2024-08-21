using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LeaderboardManager
{
    [Serializable]
    public class Leaderboard
    {
        public List<PlayerRanking> players = new();
    }

    [Serializable]
    public class PlayerRanking
    {
        public string name;
        public int level;
        public int avtId;
    }

    public class ListName
    {
        public List<string> names = new();
    }

    public static Leaderboard LeaderboardData
    {
        get
        {
            if (PlayerPrefs.HasKey("LeaderboardData"))
                return JsonConvert.DeserializeObject<Leaderboard>(PlayerPrefs.GetString("LeaderboardData"));
            else
            {
                var defaultLeaderboard = new Leaderboard();
                var defaultListName = new ListName();

                JsonUtility.FromJsonOverwrite(Helper.LoadTextFileFromResource("leaderboard-default.json"), defaultListName);

                int curLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

                defaultLeaderboard.players.Add(new PlayerRanking
                {
                    name = "YOU",
                    level = curLevel,
                    avtId = UnityEngine.Random.Range(0, 10)
                });

                System.Random random = new System.Random();

                for (int i = 0; i < defaultListName.names.Count; i++)
                {
                    int generatedLevel = curLevel + random.Next(-50, 200);

                    if (generatedLevel < 1)
                        generatedLevel = 1;

                    defaultLeaderboard.players.Add(new PlayerRanking
                    {
                        name = defaultListName.names[i],
                        level = generatedLevel,
                        avtId = UnityEngine.Random.Range(0, 10)
                    });
                }

                defaultLeaderboard.players = defaultLeaderboard.players.OrderByDescending(player => player.level).ToList();

                PlayerPrefs.SetString("LeaderboardData", JsonConvert.SerializeObject(defaultLeaderboard));

                return defaultLeaderboard;
            }
        }
        set
        {
            PlayerPrefs.SetString("LeaderboardData", JsonConvert.SerializeObject(value));
        }
    }

    public static string SaveLastDay
    {
        get
        {
            return PlayerPrefs.GetString("SaveLastDayLeaderboard", Helper.GetUnixTimeSecStr());
        }
        set
        {
            PlayerPrefs.SetString("SaveLastDayLeaderboard", value);
        }
    }


    public static Leaderboard saveLeaderboard;

    public static void Load()
    {
        saveLeaderboard = LeaderboardData;

        var date = Helper.GetUnixTimeSec();

        if (long.Parse(SaveLastDay) < date)
        {
            UpdateAllLeaderboard();

            SaveLastDay = date.ToString();
        }
    }

    public static void Save()
    {
        LeaderboardData = saveLeaderboard;
    }

    public static int GetIndexYou()
    {
        return saveLeaderboard.players.FindIndex(x => x.name == "YOU");
    }

    public static void UpdateData()
    {
        var myDataIndex = saveLeaderboard.players.FindIndex(x => x.name == "YOU");

        var myData = saveLeaderboard.players[myDataIndex];

        if (myData.level == DataManager.Instance.playerData.saveLevelData.currentLevel) return;

        saveLeaderboard.players[myDataIndex].level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        UpdateAllLeaderboard(false);

        saveLeaderboard.players[myDataIndex].level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        OrderLeaderboard();

        Save();
    }

    public static void OrderLeaderboard()
    {
        saveLeaderboard.players = saveLeaderboard.players.OrderByDescending((player) => player.level).ToList();
    }

    public static void UpdateAllLeaderboard(bool isByDay = true)
    {
        Debug.Log("Update All Leaderboard");

        for (int i = 0; i < saveLeaderboard.players.Count; i++)
        {
            var player = saveLeaderboard.players[i];

            if (player.name == "YOU") continue;

            System.Random random = new System.Random();

            var (minn, maxx) = isByDay ? RandomIncreaseLevelByDay() : RandomIncreaseLevelByRuntime();

            saveLeaderboard.players[i].level += random.Next(minn, maxx);
        }

        OrderLeaderboard();

        Save();
    }

    private static (int, int) RandomIncreaseLevelByDay()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 101);

        int minIncrease, maxIncrease;

        if (randomNumber <= 1)
        {
            minIncrease = 150;
            maxIncrease = 200;
        }
        else if (randomNumber <= 3)
        {
            minIncrease = 100;
            maxIncrease = 150;
        }
        else if (randomNumber <= 6)
        {
            minIncrease = 80;
            maxIncrease = 100;
        }
        else if (randomNumber <= 10)
        {
            minIncrease = 60;
            maxIncrease = 80;
        }
        else if (randomNumber <= 30)
        {
            minIncrease = 40;
            maxIncrease = 60;
        }
        else if (randomNumber <= 60)
        {
            minIncrease = 20;
            maxIncrease = 40;
        }
        else
        {
            minIncrease = 1;
            maxIncrease = 20;
        }

        return (minIncrease, maxIncrease);
    }

    private static (int, int) RandomIncreaseLevelByRuntime()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 101);

        int minIncrease, maxIncrease;

        if (randomNumber <= 40)
        {
            minIncrease = 1;
            maxIncrease = 2;
        }
        else
        {
            minIncrease = 0;
            maxIncrease = 0;
        }

        return (minIncrease, maxIncrease);
    }
}