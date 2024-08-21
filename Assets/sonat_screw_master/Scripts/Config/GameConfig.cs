using System;

public class GameConfig
{
    public const int HOUR_TO_SEC = 3600;
}

public enum EColorShape
{
    None = -1,
    Dark_Pink = 0,
    Pink = 1,
    Orange = 2,
    Yellow = 3,
    Green = 4,
    Cyan = 5,
    Gray = 6,
    Light_Purple = 7,
    Light_Orange = 8,
    Purple = 9
}

public enum EObstacleType
{
    None = -1,
    Star = -2,
    Rope = 0,
    Ice = 1,
    Gate = 2,
    Bomb = 3,
    Key = 4,
    Lock = 5,
    Chain = 6,
}

public enum EHoleType
{
    Normal = -1,
    Star = 0,
}

[Serializable]
public enum EScrewColor
{
    None = -1,
    Blue = 0,
    Green = 1,
    Ocean_Blue = 2,
    OldGreen = 3,
    Orange = 4,
    Pink = 5,
    Purple = 6,
    Red = 7,
    Violet = 8,
    Yellow = 9
}

public enum GameState
{
    None,
    Ingame,
    Home,
    Build
}

public enum GameplayState
{
    Pausing,
    Playing
}

public enum LoseCause
{
    FullHole = 0,
    Ice = 1,
    Gate = 2,
    Bomb = 3,
}

public enum EScreen
{
    Lose,
    Win,
    Shop,
    Ingame,
    Home,
    Build,
    Leaderboard,
}

public enum EDifficulty
{
    Normal = 0,
    Hard = 4,
    Super_Hard = 5
}