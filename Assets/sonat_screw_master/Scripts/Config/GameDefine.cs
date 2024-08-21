using UnityEngine;

public static class GameDefine
{
    public const string BANNER_ID = "2f510eaaa5aa9625";
    public const string BANNER_ID_IOS = "85dd9d5703961ddd";

    public static int MAX_LEVEL = 370;

    public const int LEVEL_GO_HOME = 11;
    public const int LEVEL_SHOW_PREBOOSTER = 16;

    public static int[] LEVEL_SHOW_RATE = { 19, 39, 55, 79 };

    public const int DEFAULT_COIN = 5;
    public const int COIN_WIN_DEFAULT = 10;
    public const int COIN_REVIVE = 255;

    public const int LIVE_COST = 50;

    public const float TRANSITION_DURATION = 0.7f;
    public const float TRANSITION_DURATION_GAP = 0.3f;

    public const float TRANSITION_DURATION_IOS = TRANSITION_DURATION_IN + TRANSITION_DURATION_IDLE + TRANSITION_DURATION_OUT;

    //public const float TRANSITION_DURATION_IN = 0.8333334f;

    //public const float TRANSITION_DURATION_IDLE = 1f;

    //public const float TRANSITION_DURATION_OUT = 0.4666667f;

    public const float TRANSITION_DURATION_IN = 0.3f;

    public const float TRANSITION_DURATION_IDLE = 0.2f;

    public const float TRANSITION_DURATION_OUT = 0.5f;


    public const int MAX_LIVES = 5;
    public const int RESTORELIVES_DURATION = 1800;

    public static readonly Vector3 DEFAULT_SCREW_SCALE = Vector3.one * 1.3f;

    public static int[] LOG_LEVEL_TRACKING = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 40, 50, 60, 70, 80, 90, 100 };

    public static int[] LOG_COMPLETE_REWARD_ADS = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

    public static int[] LOG_SHOW_ADS = { 1, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
}
