using DarkTonic.PoolBoss;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HolesQueueController : SingletonBase<HolesQueueController>
{
    [SerializeField] private Transform holePrefab;
    [SerializeField] private Transform holeQueue;
    [SerializeField] private Button watchAds;

    public List<HoleQueue> holesQueue = new List<HoleQueue>();

    private int holeNum = 5;
    private int maxHole = 7;
    private int maxHoleLose = 9;

    private readonly Vector3 DEFAULT_HOLE_SCALE = Vector3.one;

    private void Awake()
    {
        watchAds.onClick.AddListener(() =>
        {
            AddOneMoreHole();

            BoosterManager.Instance.boosterHole.CheckOutOfBooster();

            watchAds.gameObject.SetActive(false);

            SonatTracking.LogEarnCurrency("add_hole_ads", "booster", 1, "Ingame", "non_iap",
            DataManager.Instance.playerData.saveLevelData.currentLevel);

        });
    }

    public void SetupHoleQueue()
    {
        holeNum = DataManager.Instance.levelData.holeQueue;

        if (holeNum == 0) { holeNum = 5; }

        for (int i = 0; i < holeNum; i++)
        {
            Transform hole = PoolBoss.Spawn(holePrefab, holeQueue.position, Quaternion.identity, holeQueue);

            hole.localScale = DEFAULT_HOLE_SCALE;

            HoleQueue holeScript = hole.GetComponent<HoleQueue>();

            holeScript.StopWarning();

            holeScript.screw = null;

            holesQueue.Add(holeScript);
        }

        watchAds.transform.SetAsLastSibling();
        watchAds.gameObject.SetActive(DataManager.Instance.playerData.saveLevelData.currentLevel
            >= 7);
    }

    public void AddOneMoreHole()
    {
        holeNum++;

        Transform hole = PoolBoss.Spawn(holePrefab, holeQueue.position, Quaternion.identity, holeQueue);

        hole.localScale = DEFAULT_HOLE_SCALE * 0.5f;
        hole.DOScale(DEFAULT_HOLE_SCALE, 0.2f);

        HoleQueue holeScript = hole.GetComponent<HoleQueue>();

        holeScript.screw = null;

        holesQueue.Add(holeScript);

        holeScript.PlayAddHoleEffect();

        SoundManager.Instance.PlaySound(KeySound.AddHole);

        if (holeNum >= maxHole)
        {
            watchAds.gameObject.SetActive(false);
        }
        else
        {
            watchAds.transform.SetAsLastSibling();
        }

        CheckWarning();
    }

    public void ActiveWatchAds(bool state)
    {
        watchAds.gameObject.SetActive(state);
    }

    public void ResetData()
    {
        foreach (HoleQueue hole in holesQueue)
        {
            if (!hole.IsEmpty)
            {
                PoolBoss.Despawn(hole.screw.transform);
            }

            PoolBoss.Despawn(hole.transform);
        }
        holesQueue.Clear();
    }

    public bool HasMovingScrewInQueue()
    {
        foreach (HoleQueue hole in holesQueue)
        {
            if (!hole.IsEmpty)
            {
                if (hole.screw.IsMoving)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsQueueFilled()
    {
        foreach (HoleQueue hole in holesQueue)
        {
            if (hole.IsEmpty)
                return false;
        }
        return true;
    }

    public int GetHoleQueueEmptyIndex()
    {
        for (int i = 0; i < holesQueue.Count; i++)
        {
            if (holesQueue[i].IsEmpty)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetRemainEmptyHole()
    {
        int count = 0;

        for (int i = 0; i < holesQueue.Count; i++)
        {
            if (holesQueue[i].IsEmpty)
            {
                count++;
            }
        }

        return count;
    }

    public void StopWarningAll()
    {
        for (int i = 0; i < holesQueue.Count; i++)
        {
            holesQueue[i].StopWarning();
        }
    }

    public void CheckWarning()
    {
        var remain = GetRemainEmptyHole();

        if (remain == 1)
        {
            Debug.Log("Namtt Warning");
            holesQueue[GetHoleQueueEmptyIndex()].Warning();
        }
        else
        {
            Debug.Log("Namtt Stop Warning");
            StopWarningAll();
        }
    }

    public bool IsMaxHole()
    {
        return holeNum >= maxHole;
    }

    public bool IsMaxHoleLose()
    {
        return holeNum >= maxHoleLose;
    }

    public void Revive()
    {
        if (holeNum < maxHoleLose)
        {
            AddOneMoreHole();

            DOVirtual.DelayedCall(0.2f, () =>
            {
                AddOneMoreHole();

                GameplayManager.Instance.GameplayState = GameplayState.Playing;

                BoosterManager.Instance.boosterHole.CheckOutOfBooster();
            });
        }
    }
}
