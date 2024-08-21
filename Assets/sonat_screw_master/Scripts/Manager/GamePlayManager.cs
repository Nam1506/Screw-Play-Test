using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DarkTonic.PoolBoss;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class GameplayManager : SingletonBase<GameplayManager>
{
    public float damping;

    [field: SerializeField] public GameplayState GameplayState { get; set; }

    [SerializeField] private Camera cam;

    public List<Box> boxes = new List<Box>();
    public List<Shape> shapes = new List<Shape>();
    public List<Screw> screws = new List<Screw>();

    public float timePlay;
    public float freeTime;
    public bool isContinueWith = false;
    public int numContinue = 0;
    public int moveCount = 0;

    public EDifficulty difficulty;

    public bool IsAddingBox { get; set; }
    public bool IsLose { get; set; }
    public LoseCause LoseCause { get; set; }

    public event EventHandler OnStartLevelAction;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
    }

    public void StartLevel()
    {
        ResetData();

        DataManager.Instance.StartNewLevel();

        Debug.Log(DataManager.Instance.playerData.saveLevelData.currentLevel);

        if (!DataManager.Instance.LoadLevel(DataManager.Instance.playerData.saveLevelData.currentLevel))
        {
            DespawnAllObjects();
            return;
        }

        SetupGame();

        UIManager.Instance.UIIngame.UpdateLevelText();

        UIManager.Instance.UIIngame.ShowUI();

        BoosterManager.Instance.LoadData();
        BoosterManager.Instance.boosterHammer.isFree = false;

        TutorialManager.Instance.CheckTutorial();

        GameplayState = GameplayState.Playing;

        OnStartLevelAction?.Invoke(this, EventArgs.Empty);

        //if (!(BoosterManager.Instance.IsLevelUnlockBooster() && DataManager.Instance.IsFirstPlay()) && !TutorialManager.Instance.IsShowingTut ||
        //        DataManager.Instance.playerData.saveLevelData.currentLevel == 1)
        //{
        //    //Helper.WaitForTransition(() =>
        //    //{
        //    //    PopupManager.Instance.ShowHardLevel(difficulty);
        //    //});

        //    return;
        //}

        //GameplayState = GameplayState.Pausing;
    }

    public void ResetData()
    {
        timePlay = 0;
        freeTime = 0;
        numContinue = 0;
        isContinueWith = false;
        moveCount = 0;

        IsAddingBox = false;
        IsLose = false;

        DespawnAllObjects();
    }

    public void DespawnAllObjects()
    {
        foreach (Box box in boxes)
        {
            box.ResetData();
            PoolBoss.Despawn(box.transform);
        }
        boxes.Clear();

        foreach (Shape shape in shapes)
        {
            shape.ResetData();
            PoolBoss.Despawn(shape.transform);
        }
        shapes.Clear();

        HolesQueueController.Instance.ResetData();

        screws.Clear();

        PopupManager.Instance.PrePopups.Clear();
        SoundManager.Instance.StopLoopSound();
        ChainGenerator.Instance.ClearData();

        TutorialManager.Instance.HideTut();

        ObstacleController.Instance.Clear();
    }

    public void SetupGame()
    {
        LevelGenerator.Instance.SetupLevel();
        BoxController.Instance.SetupBoxes();
        HolesQueueController.Instance.SetupHoleQueue();
    }

    string getD(int a)
    {
        if (a < 10) return "0" + a;
        return a.ToString();
    }

    Screw GetScrew(int id)
    {
        foreach (var screw in screws)
        {
            if (screw.identify == id)
            {
                return screw;
            }

        }

        return null;
    }

    float radius = 0.44f * 1.2f;
    public int numCheckPoints = 8;

    public void Calculate()
    {
        var now = System.DateTime.Now;
        var DIR_SHEET = $"{DataManager.Instance.playerData.saveLevelData.currentLevel}_{now.Year}{getD(now.Month)}{getD(now.Day)}_{getD(now.Hour)}{getD(now.Minute)}{getD(now.Second)}.csv";

#if UNITY_EDITOR
        DIR_SHEET = Application.dataPath + "/" + DIR_SHEET;
#endif
        var filePath = DIR_SHEET;

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        //WriteCSV(GetAllDataToExport(), filePath);

        List<Table1> table1s = new();

        foreach (var screw in screws)
        {
            List<Collider2D> results = Physics2D.OverlapCircleAll(screw.transform.position, radius).ToList();
            List<int> fullyCoveredColliders = new List<int>();
            List<int> coveredColliders = new List<int>();

            string cover = "";
            string coverFull = "";

            foreach (var collider in results)
            {
                if (collider.GetComponent<Shape>() != null)
                {
                    if (screw.shape.Layer >= collider.GetComponent<Shape>().Layer) { continue; }

                    if (IsFullyCovered(screw.transform, collider))
                    {
                        coverFull += collider.GetComponent<Shape>().identify + "; ";
                    }

                    cover += collider.GetComponent<Shape>().identify + "; ";
                }

            }

            Table1 data = new Table1
            {
                id_screw = screw.identify,
                id_glass = screw.shape.identify,
                id_glass_cover = cover,
                id_glass_cover_full = coverFull,
                Pos_x = screw.transform.position.x,
                Pos_y = screw.transform.position.y,
                color_id = (int)screw.color
            };

            table1s.Add(data);
        }

        List<Table2> table2s = new();

        foreach (var shape in shapes)
        {
            var newTable = new Table2();

            newTable.id_glass = shape.identify;
            newTable.num_screw = shape.holes.Count;
            newTable.layer = shape.Layer;
            newTable.pos_x = shape.transform.position.x;
            newTable.pos_y = shape.transform.position.y;
            newTable.id_shape = shape.id;
            newTable.rotation = shape.transform.localEulerAngles.z;
            newTable.num_screw_color = shape.GetNumScrewColor();

            table2s.Add(newTable);
        }

        List<Table3> table3s = new();

        Dictionary<int, Table3> keyValuePairs = new Dictionary<int, Table3>();

        var obstacleData = DataManager.Instance.levelData.obstacles;

        foreach (var shape in DataManager.Instance.levelData.shapes)
        {
            if (!keyValuePairs.TryGetValue(shape.layer, out var data))
            {
                data = new Table3();
            }
            else
            {
                data = keyValuePairs[shape.layer];
            }

            data.glass += 1;
            data.layer = shape.layer;

            keyValuePairs[shape.layer] = data;
        }

        foreach (var screw in screws)
        {
            if (!keyValuePairs.TryGetValue(screw.shape.Layer, out var data))
            {
                data = new Table3();
            }
            else
            {
                data = keyValuePairs[screw.shape.Layer];
            }

            if (screw.type == EHoleType.Star) data.star++;

            keyValuePairs[screw.shape.Layer] = data;

        }

        foreach (var obstacle in DataManager.Instance.levelData.obstacles)
        {
            var screw = GetScrew(obstacle.identifies[0]);
            var layer = screw.shape.Layer;

            if (!keyValuePairs.TryGetValue(layer, out var data))
            {
                data = new Table3();
            }
            else
            {
                data = keyValuePairs[layer];
            }

            data.layer = layer;

            switch (obstacle.eObstacleType)
            {
                case EObstacleType.Ice:
                    data.ice++;
                    break;

                case EObstacleType.Bomb:
                    data.boom++;
                    break;

                case EObstacleType.Gate:
                    data.gate++;
                    break;

                case EObstacleType.Rope:
                    data.rope++;
                    break;

                case EObstacleType.Chain:
                    data.chain++;
                    break;

                default: break;
            }

            keyValuePairs[layer] = data;
        }

        foreach (var table in keyValuePairs.Values)
        {
            table3s.Add(table);
        }

        List<Table4> table4s = new();

        foreach (var box in DataManager.Instance.levelData.boxes)
        {
            var newData = new Table4();

            newData.tool_box_queue = box.holes.Count;
            newData.color_id = ((int)box.eColor);

            table4s.Add(newData);
        }

        WriteCSV(table1s, table2s, table3s, table4s, filePath);
    }

    public void WriteCSV(List<Table1> d1, List<Table2> d2, List<Table3> d3, List<Table4> d4, string filePath)
    {
        // Define all table headers
        var table1 = "id_screw, id_glass, id_glass_cover, id_glass_cover_full, Pos_X, Pos_y, color_id";
        var table2 = "id_glass, num_screw, layer, Pos_x, Pos_y, id_shape, rotation, num_screw_color";
        var table3 = "Layer Count, Glass count, Star, Ice, Boom, Gate, Rope, Chain";
        var table4 = "Tool_box_queue, color_id";

        var concatenatedHeaders = $"{table1}, ,{table2}, ,{table3}, ,{table4}";

        using (TextWriter tw = new StreamWriter(filePath, false))
        {
            tw.WriteLine(concatenatedHeaders);

            // Iterate through the lists and write the data
            int maxRows = Math.Max(Math.Max(d1.Count, d2.Count), Math.Max(d3.Count, d4.Count));

            for (int i = 0; i < maxRows; i++)
            {
                string row = "";

                if (i < d1.Count)
                {
                    row += $"{d1[i].id_screw},{d1[i].id_glass},{d1[i].id_glass_cover},{d1[i].id_glass_cover_full},{d1[i].Pos_x},{d1[i].Pos_y},{d1[i].color_id}";
                }
                else
                {
                    row += ",,,,,,";
                }

                row += ",,"; // Separator between tables

                if (i < d2.Count)
                {
                    row += $"{d2[i].id_glass},{d2[i].num_screw},{d2[i].layer},{d2[i].pos_x},{d2[i].pos_y},{d2[i].id_shape},{d2[i].rotation},{d2[i].num_screw_color}";
                }
                else
                {
                    row += ",,,,,,,";
                }

                row += ",,"; // Separator between tables

                if (i < d3.Count)
                {
                    row += $"{d3[i].layer},{d3[i].glass},{d3[i].star},{d3[i].ice},{d3[i].boom},{d3[i].gate},{d3[i].rope},{d3[i].chain}";
                }
                else
                {
                    row += ",,,,,,,";
                }

                row += ",,"; // Separator between tables

                if (i < d4.Count)
                {
                    row += $"{d4[i].tool_box_queue},{d4[i].color_id}";
                }
                else
                {
                    row += ",,";
                }

                tw.WriteLine(row);
            }
        }

        Application.OpenURL(filePath);
    }


    public class Table1
    {
        public int id_screw;
        public int id_glass;
        public string id_glass_cover;
        public string id_glass_cover_full;
        public float Pos_x;
        public float Pos_y;
        public int color_id;
    }

    public class Table2
    {
        public int id_glass;
        public int num_screw;
        public int layer;
        public float pos_x;
        public float pos_y;
        public int id_shape;
        public float rotation;
        public int num_screw_color;
    }

    public class Table3
    {
        public int layer;
        public int glass;
        public int star;
        public int ice;
        public int boom;
        public int gate;
        public int rope;
        public int chain;
    }

    public class Table4
    {
        public int tool_box_queue;
        public int color_id;
    }

    bool IsFullyCovered(Transform trans, Collider2D collider)
    {
        for (int i = 0; i < numCheckPoints; i++)
        {
            float angle = i * (2 * Mathf.PI / numCheckPoints);
            Vector2 point = new Vector2(
                trans.position.x + radius * Mathf.Cos(angle),
                trans.position.y + radius * Mathf.Sin(angle)
            );

            if (!collider.OverlapPoint(point))
            {
                return false;
            }
        }
        return true;
    }

    public void SetupObstacle()
    {
        ObstacleController.Instance.SetupObstacles();

        boxes[0].IsReady = true;

        foreach (Screw screw in screws)
        {
            screw.SetChain(screw.linkedScrews.Count);
        }

        if (HasOneValueBomb())
        {
            SoundManager.Instance.PlaySound(KeySound.Bomb_Count, true);
        }
    }

    public bool IsOverlap(Screw screw, List<Shape> shapes)
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            if (shapes[i] != null)
            {
                if (shapes[i].Layer > screw.shape.Layer)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Box> GetActiveBoxes()
    {
        List<Box> result = new List<Box>();

        foreach (Box box in boxes)
        {
            if (box.IsReady && !box.IsMoving)
            {
                result.Add(box);
            }
        }
        return result;
    }

    public bool AreAllCurrentBoxesPacked()
    {
        List<Box> boxes = GetActiveBoxes();

        foreach (Box box in boxes)
        {
            if (!box.IsPacked)
            {
                return false;
            }
        }

        return true;
    }

    public bool AreAllBoxesPacked()
    {
        foreach (Box box in boxes)
        {
            if (!box.IsPacked)
            {
                return false;
            }
        }

        return true;
    }

    public void MoveAllCurrentBoxes()
    {
        List<Box> boxes = GetActiveBoxes();

        foreach (Box box in this.boxes)
        {
            if (box.IsPacked && !boxes.Contains(box))
            {
                boxes.Add(box);
            }
        }

        foreach (Box box in boxes)
        {
            box.MoveBoxAway();
            box.IsReady = false;
        }
    }

    public bool HasMovingBox()
    {
        foreach (Box box in boxes)
        {
            if (box.IsMoving)
            {
                Debug.Log(box.name + " is moving");
                return true;
            }
        }

        return false;
    }

    public bool HasMovingScrew()
    {
        foreach (Screw screw in screws)
        {
            if (screw.IsMovingFromQueue)
            {
                Debug.Log(screw + " " + screw.color + " moving from queue");
                return true;
            }
        }

        return false;
    }

    public bool HasOneValueBomb()
    {
        foreach (Screw screw in screws)
        {
            if (screw.BombCount == 1)
            {
                return true;
            }
        }
        return false;
    }

    public void CheckWin()
    {
        Debug.Log("Check win");
        foreach (Box box in boxes)
        {
            if (!box.IsComplete())
                return;
        }

        Win();
    }

    public void Test_Win()
    {
        //GameplayState = GameplayState.Pausing;

        //PopupManager.OnSetInteractableMask?.Invoke(true);

        //Debug.Log("Win");

        //DataManager.Instance.Win();

        //RaceEvent.OnWinLevel();

        //SoundManager.Instance.PlaySound(KeySound.Win);

        //EffectManager.Instance.PlayWinEffect();

        //PopupManager.Instance.ShowWin();

        DespawnAllObjects();

        Win();
    }

    private void Win()
    {
        GameplayState = GameplayState.Pausing;

        Debug.Log("Win");

        StartCoroutine(IeShowWin());
    }

    private IEnumerator IeShowWin()
    {
        while (boxes.Count > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        PopupManager.Instance.ShowNotiAlert("Win");

        //SoundManager.Instance.PlaySound(KeySound.Win);

        //EffectManager.Instance.PlayWinEffect();

        //PopupManager.Instance.ShowWin();
    }

    public void CheckLose()
    {
        if (HolesQueueController.Instance.IsQueueFilled())
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                if (HasMovingBox() || (HasMovingScrew() && !HolesQueueController.Instance.IsQueueFilled()))
                {
                    Debug.Log("Moving box: " + HasMovingBox());
                    Debug.Log("Moving screw: " + HasMovingScrew());
                    return;
                }

                List<Box> aciveBoxes = GetActiveBoxes();

                foreach (Box box in aciveBoxes)
                {
                    if (!box.IsComplete())
                    {
                        SetLose(LoseCause.FullHole);
                    }
                }
            });
        }
    }

    public void CheckObstaclesLose()
    {
        Debug.Log("check obstacle lose");

        List<Screw> inShapeScrews = new();

        foreach (Screw screw in screws)
        {
            if (screw.hole is ShapeHole)
            {
                inShapeScrews.Add(screw);
            }
        }

        Debug.Log("Shape: " + shapes.Count);

        StartCoroutine(IeCheckObstaclesLose(inShapeScrews));
    }

    private bool isWaitForCheckLose = false;

    private IEnumerator IeCheckObstaclesLose(List<Screw> inShapeScrews)
    {
        Debug.Log("Shape: " + shapes.Count);

        foreach (Shape shape in shapes)
        {
            Debug.Log("Shape: " + shapes.Count);

            if (shape.gameObject.activeInHierarchy)
            {
                if (shape.IsEmpty())
                {
                    isWaitForCheckLose = true;

                    float waitTime = 0;

                    while (shape.gameObject.activeSelf && waitTime < 1.5f)
                    {
                        waitTime += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }

        foreach (Screw screw in inShapeScrews)
        {
            if (!screw.IsBlocking())
            {
                yield break;
            }
        }

        foreach (Screw screw in inShapeScrews)
        {
            if (screw.IsOverlap()) continue;

            if (screw.IsFrozen)
            {
                SetLose(LoseCause.Ice);
            }
            else if (screw.IsCloseGate)
            {
                SetLose(LoseCause.Gate);
            }
        }
    }

    public void SetLose(LoseCause loseCause)
    {
        Debug.Log("Set lose");

        if (IsLose)
        {
            if (LoseCause == loseCause || loseCause == LoseCause.FullHole)
            {
                return;
            }
        }

        IsLose = true;

        LoseCause = loseCause;

        GameplayState = GameplayState.Pausing;

        //PopupManager.OnSetInteractableMask?.Invoke(true);

        //if (loseCause == LoseCause.Bomb)
        //{
        //    Lose();
        //}
        Lose();
    }

    public void Lose()
    {
        GameplayState = GameplayState.Pausing;

        PopupManager.Instance.ShowNotiAlert("Lose");

        //DespawnAllObjects();

        //StartLevel();
    }

    private IEnumerator IeShowLose()
    {
        if (LoseCause == LoseCause.FullHole)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            if (!isWaitForCheckLose)
                yield return new WaitForSeconds(1.2f);
        }

        SoundManager.Instance.PlaySound(KeySound.Lose);

        PopupManager.Instance.ShowLose();
    }

    public void CheckPutScrewInQueueToBox()
    {
        bool canFreeQueue = false;

        foreach (Hole hole in HolesQueueController.Instance.holesQueue)
        {
            if (!hole.IsEmpty/* && !hole.screw.IsMoving*/)
            {
                if (hole.screw.CheckAvailableHoleInBox(true))
                {
                    hole.screw = null;
                    canFreeQueue = true;

                    HolesQueueController.Instance.CheckWarning();
                }
            }
        }

        if (!canFreeQueue)
        {
            Debug.Log("can't free queue");
            CheckLose();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Calculate();
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            foreach (var shape in shapes)
            {
                shape.ShowIndentify();
            }

            foreach (var screw in screws)
            {
                screw.ShowIndentify();
            }
        }

        if (GameplayState != GameplayState.Playing)
        {
            return;
        }

        timePlay += Time.deltaTime;
        freeTime += Time.deltaTime;

        if (freeTime > 3f)
        {
            freeTime = 0;

            if (DataManager.Instance.playerData.saveLevelData.currentLevel == 1)
            {
                TutorialManager.Instance.ShowTutLevel1();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePosition);

            // check hammer
            if (BoosterManager.Instance.IsUsingHammer)
            {
                if (colliders.Length == 0)
                {
                    BoosterManager.Instance.CheckCancelHammer();
                    return;
                }

                List<Shape> clickedShapes = new List<Shape>();

                // Process each collider
                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent(out Shape temp))
                    {
                        clickedShapes.Add(temp);
                    }
                }

                clickedShapes = clickedShapes.OrderByDescending(shape => shape.Layer).ToList();

                if (clickedShapes.Count > 0)
                {
                    clickedShapes[0].Break();
                }

                BoosterManager.Instance.CheckCancelHammer();
                return;
            }

            //get first screw
            if (colliders.Length == 0) return;

            List<Screw> otherScrews = new List<Screw>();

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out Screw otherScrew))
                {
                    otherScrews.Add(otherScrew);
                }
            }

            if (otherScrews.Count == 0) return;

            otherScrews = otherScrews.OrderByDescending(screw => screw.shape.Layer).ToList();

            Screw hitScrew = otherScrews[0];

            if (!hitScrew.IsOverlap())
            {
                hitScrew.OnScrewClick();
            }
            else
            {
                Debug.Log("overlap");
                VibrateManager.Instance.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);
            }

            //// check overlap
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            //RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Screw"));

            //List<Collider2D> overlapObjects = new List<Collider2D>();

            //ContactFilter2D filter = new ContactFilter2D();

            //hit.collider.OverlapCollider(filter.NoFilter(), overlapObjects);

            //List<Shape> shapes = new List<Shape>();

            //foreach (Collider2D collider in overlapObjects)
            //{
            //    if (collider.TryGetComponent(out Shape temp))
            //    {
            //        shapes.Add(temp);
            //    }
            //}

            //if (IsOverlap(hitScrew, shapes))
            //{
            //    Debug.Log("overlap");
            //    return;
            //}

            //hitScrew.OnScrewClick();
        }
    }

    public void CheckObstacles(Screw checkingScrew)
    {
        bool isSoundIce = false;
        bool isSoundGate = false;

        foreach (Screw screw in screws)
        {
            if (screw != checkingScrew && !screw.IsOverlap())
            {
                if (screw.CheckBreakIce())
                {
                    isSoundIce = true;
                }

                if (screw.CheckToggleGate())
                {
                    isSoundGate = true;
                }
                screw.CheckBomb();
            }
        }

        if (isSoundIce)
        {
            SoundManager.Instance.PlaySound(KeySound.Break_Ice);
        }

        if (isSoundGate)
        {
            SoundManager.Instance.PlayGateSound();
        }
    }
}
