using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkinSystem
{
    public static readonly string KEY = "SKIN_DATA";

    public static SkinData data;

    private static SkinData Data
    {
        get
        {
            if (PlayerPrefs.HasKey(KEY))
            {
                return JsonConvert.DeserializeObject<SkinData>(PlayerPrefs.GetString(KEY));
            }
            else
            {
                var newSkinData = InitData();

                PlayerPrefs.SetString(KEY, JsonConvert.SerializeObject(newSkinData));

                return newSkinData;
            }

        }
        set
        {
            PlayerPrefs.SetString(KEY, JsonConvert.SerializeObject(value));
        }
    }

    public static void Load()
    {
        data = Data;
    }

    public static void Save()
    {
        Data = data;
    }

    public static bool IsCollectedSkin(ESkin eSkin, int id)
    {
        switch (eSkin)
        {
            case ESkin.Box:
                return data.boxCollection.ids.Contains(id);

            case ESkin.Screw:
                return data.screwCollection.ids.Contains(id);

            default:
                return false;
        }
    }

    public static bool IsEquippedSkin(ESkin eSkin, int id)
    {
        switch (eSkin)
        {
            case ESkin.Box:
                return data.boxCollection.curSkin == id;

            case ESkin.Screw:
                return data.screwCollection.curSkin == id;

            default:
                return false;
        }
    }

    public static bool IsUnlockedSkin(ESkin eSkin, int id)
    {
        switch (eSkin)
        {
            case ESkin.Box:
                return BoxController.Instance.BoxData.GetBoxSO(id).IsUnlocked();

            case ESkin.Screw:
                return LevelGenerator.Instance.ScrewData.GetScrewSO(id).IsUnlocked();

            default:
                return false;
        }
    }

    public static void UnlockSkin(ESkin eSkin, int id)
    {
        switch (eSkin)
        {
            case ESkin.Box:
                if (!data.boxCollection.ids.Contains(id))
                    data.boxCollection.ids.Add(id);

                break;

            case ESkin.Screw:
                if (!data.screwCollection.ids.Contains(id))
                    data.screwCollection.ids.Add(id);
                break;
        }

        Save();
    }

    public static void EquipSkin(ESkin eSkin, int id)
    {
        switch (eSkin)
        {
            case ESkin.Box:
                data.boxCollection.curSkin = id;
                break;

            case ESkin.Screw:
                data.screwCollection.curSkin = id;
                break;
        }

        Save();
    }

    private static SkinData InitData()
    {
        var newSkinData = new SkinData()
        {
            boxCollection = new SkinCollection(),
            screwCollection = new SkinCollection()
        };

        return newSkinData;
    }

    public class SkinData
    {
        public SkinCollection boxCollection;
        public SkinCollection screwCollection;
    }

    public class SkinCollection
    {
        public List<int> ids = new();
        public int curSkin = new();

        public SkinCollection()
        {
            ids.Add(0);
            curSkin = 0;
        }
    }

    public enum ESkin
    {
        Box = 0,
        Screw = 1
    }
}
