using DarkTonic.PoolBoss;
using Obi;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;

public class ObstacleController : SingletonBase<ObstacleController>
{
    [SerializeField] private Transform ropePrefab;
    [SerializeField] private Transform chainPrefab;
    [SerializeField] private ObiSolver obiSolver;

    public List<Rope> ropes = new();
    public List<Screw> lockedScrews = new();

    private ObstacleData data;

    public void SetupObstacles()
    {
        foreach (var obstacle in DataManager.Instance.levelData.obstacles)
        {
            data = obstacle;

            switch (obstacle.eObstacleType)
            {
                case EObstacleType.Rope:
                    SetupRope();
                    break;
                case EObstacleType.Ice:
                    SetupIce();
                    break;
                case EObstacleType.Gate:
                    SetupGate();
                    break;
                case EObstacleType.Bomb:
                    SetupBomb();
                    break;
                case EObstacleType.Chain:
                    SetupChain();
                    break;
                case EObstacleType.Key:
                    SetupKey();
                    break;
                case EObstacleType.Lock:
                    SetupLock();
                    break;
            }
        }
    }

    public void Clear()
    {
        ClearRope();
        lockedScrews.Clear();
    }

    private void ClearRope()
    {
        foreach (Rope rope in ropes)
        {
            rope.DestroyImmediate();
        }
        ropes.Clear();
    }

    private void SetupRope()
    {
        Screw topScrew = GameplayManager.Instance.screws.Find(s => s.identify == data.identifies[0]);
        Screw botScrew = GameplayManager.Instance.screws.Find(s => s.identify == data.identifies[1]);

        //Transform ropeTrf = PoolBoss.Spawn(ropePrefab, Vector3.zero, Quaternion.identity, obiSolver.transform);
        Transform ropeTrf = Instantiate(ropePrefab, obiSolver.transform);

        Rope rope = ropeTrf.GetComponent<Rope>();

        rope.Init(topScrew, botScrew);

        topScrew.rope = rope;
        botScrew.rope = rope;

        ropes.Add(rope);
    }

    private void SetupIce()
    {
        foreach (int id in data.identifies)
        {
            Screw screw = GameplayManager.Instance.screws.Find(s => s.identify == id);

            if (screw != null)
            {
                screw.SetIce();
            }
        }
    }

    private void SetupGate()
    {
        foreach (int id in data.identifies)
        {
            Screw screw = GameplayManager.Instance.screws.Find(s => s.identify == id);

            if (screw != null)
            {
                screw.SetGate(data.isOpen);
            }
        }
    }

    private void SetupBomb()
    {
        foreach (int id in data.identifies)
        {
            Screw screw = GameplayManager.Instance.screws.Find(s => s.identify == id);

            if (screw != null)
            {
                screw.SetBomb(data.ValueBoom);
            }
        }
    }

    private void SetupChain()
    {
        Screw centerScrew = null;

        List<int> identifies = data.identifies.ToHashSet().ToList();

        for (int i = 0; i < identifies.Count; i++)
        {
            int identify = identifies[i];

            Screw screw = GameplayManager.Instance.screws.Find(s => s.identify == identify);

            if (screw == null)
            {
                Debug.LogError("id screw doesn't match with chain");
                return;
            }

            if (i == 0)
            {
                centerScrew = screw;
            }
            else
            {
                var chain = PoolBoss.Spawn(chainPrefab, Vector3.zero, Quaternion.identity, null).GetComponent<Chain>();

                chain.Set(centerScrew, screw);

                ChainGenerator.Instance.chains.Add(chain);

                centerScrew.linkedScrews.Add(screw);
                screw.centerScrew = centerScrew;
            }
        }
    }

    private void SetupKey()
    {
        foreach (int id in data.identifies)
        {
            Screw screw = GameplayManager.Instance.screws.Find(s => s.identify == id);

            if (screw != null)
            {
                screw.SetKey(true);
            }
        }
    }

    private void SetupLock()
    {
        foreach (int id in data.identifies)
        {
            Screw screw = GameplayManager.Instance.screws.Find(s => s.identify == id);

            if (screw != null)
            {
                screw.SetLock(true);
                lockedScrews.Add(screw);
            }
        }
    }

    public Screw GetLockedSrew()
    {
        if (lockedScrews.Count == 0) return null;

        return lockedScrews[0];
    }

    public void RemoveLockedScrew(Screw screw)
    {
        lockedScrews.Remove(screw);
    }
}
