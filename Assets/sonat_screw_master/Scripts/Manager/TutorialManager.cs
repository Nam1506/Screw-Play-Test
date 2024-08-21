using Spine.Unity;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : SingletonBase<TutorialManager>
{
    [SerializeField] private TextMeshProUGUI tutText;
    [SerializeField] private SkeletonAnimation handAnim;
    [SerializeField] private SkeletonAnimation nutAnim;

    [SerializeField] private ObstacleSO obstacleSO;

    private Screw screwTut;
    private bool isShowingTut;

    public bool IsShowingTut => isShowingTut;

    public void CheckTutorial()
    {
        screwTut = null;
        isShowingTut = false;

        int currentLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

        foreach (var obstacleData in obstacleSO.obstacles)
        {
            if (currentLevel == 1)
            {
                screwTut = FindScrew(EObstacleType.None);
                tutText.text = "Collect screws <br> matching the toolbox color";
                tutText.color = Helper.GetColorFromScrew(screwTut.color);

                ShowTut();
                return;
            }
            else if (currentLevel == obstacleData.levelUnlock + 1)
            {
                //screwTut = FindScrew(obstacleData.type);
                //ShowTut();

                //PopupManager.Instance.ForceShowPopupActions.Add(() =>
                //{
                //    PopupManager.Instance.ShowUnlockObstacle(obstacleData.type);
                //});

                return;
            }
        }

        HideTut();
    }

    public bool IsLevelUnlockObs()
    {
        int currentLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

        foreach (var obstacleData in obstacleSO.obstacles)
        {
            if (currentLevel == obstacleData.levelUnlock + 1)
            {
                return true;
            }
        }

        return false;
    }

    public void ShowTutLevel1()
    {
        screwTut = FindScrew(EObstacleType.None);
        tutText.text = "Collect screws <br> matching the toolbox color";
        tutText.color = Helper.GetColorFromScrew(screwTut.color);

        ShowTut();
    }

    public bool IsLevelShowTut()
    {
        int currentLevel = DataManager.Instance.playerData.saveLevelData.currentLevel;

        foreach (var obstacleData in obstacleSO.obstacles)
        {
            if (currentLevel == 1 || currentLevel == obstacleData.levelUnlock + 1)
            {
                return true;
            }
        }

        return false;
    }

    private Screw FindScrew(EObstacleType eObstacleType)
    {
        switch (eObstacleType)
        {
            case EObstacleType.None:
                if (GameplayManager.Instance.screws.Count > 0)
                {
                    foreach (Screw screw in GameplayManager.Instance.screws)
                    {
                        if (!screw.IsOverlap() && screw.hole is ShapeHole)
                        {
                            return screw;
                        }
                    }
                }
                break;
            case EObstacleType.Star:
                Box box = GameplayManager.Instance.boxes[0];

                foreach (Screw screw in GameplayManager.Instance.screws)
                {
                    if (screw.color == box.eColor && screw.type == EHoleType.Star)
                    {
                        tutText.text = "Collect the star <br> screws as required";
                        tutText.color = Helper.GetColorFromScrew(screw.color);
                        return screw;
                    }
                }
                break;
            case EObstacleType.Rope:
                tutText.text = "Screws linked together <br> move collectively";
                tutText.color = Helper.GetColorFromScrew(EScrewColor.OldGreen);
                break;
            case EObstacleType.Ice:
                tutText.text = "Collect 3 screws <br> to break the ice";
                tutText.color = new Color(0.2941177f, 0.9058824f, 0.9137256f);
                break;
            case EObstacleType.Gate:
                tutText.text = "The gate opens and <br> closes with each move";
                tutText.color = new Color(0.7254902f, 0.7294118f, 0.7098039f);
                break;
            case EObstacleType.Bomb:
                tutText.text = "Detach the bomb screw <br> before running out of moves";
                tutText.color = Helper.GetColorFromScrew(EScrewColor.Orange);
                break;
            case EObstacleType.Chain:
                tutText.text = "Release the chained screw <br> by removing the chains";
                tutText.color = new Color(0.7254902f, 0.7294118f, 0.7098039f);
                break;
            case EObstacleType.Lock:
                tutText.text = "Collect the key <br> to open the locked screw";
                tutText.color = Helper.GetColorFromScrew(EScrewColor.Yellow);
                break;
        }

        return null;
    }

    private void ShowTut()
    {
        isShowingTut = true;
        StartCoroutine(IeShowTut());
    }

    private IEnumerator IeShowTut()
    {
        tutText.gameObject.SetActive(true);

        if (screwTut == null)
            yield break;

        nutAnim.transform.position = screwTut.transform.position;
        handAnim.transform.position = screwTut.transform.position + new Vector3(1.45f, -0.9f, 0);

        handAnim.gameObject.SetActive(true);
        handAnim.Initialize(true);
        handAnim.AnimationState.SetAnimation(0, "Hand", true);

        GameplayManager.Instance.boxes[0].PlayTutAnim();

        yield return new WaitForSeconds(0.5f);

        nutAnim.gameObject.SetActive(true);
        nutAnim.Initialize(true);
        nutAnim.AnimationState.SetAnimation(0, "Nut", true);
    }

    public void HideTut()
    {
        //if (!isShowingTut) return;

        tutText.gameObject.SetActive(false);
        handAnim.gameObject.SetActive(false);
        nutAnim.gameObject.SetActive(false);

        if (GameplayManager.Instance.boxes.Count > 0)
            GameplayManager.Instance.boxes[0].StopTutAnim();

        isShowingTut = false;
    }
}
