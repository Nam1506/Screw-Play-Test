using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINodeLevel : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject ropeLeft;
    [SerializeField] private GameObject ropeRight;

    [SerializeField] private GameObject inActiveNode;
    [SerializeField] private SkeletonGraphic activeNode;

    [SerializeField] private TMP_Text[] levelText;
    [SerializeField] private GameObject[] panelDiff;
    [SerializeField] private GameObject[] hardText;
    [SerializeField] private GameObject[] superHardText;

    [SerializeField] private SkeletonUtilityBone bone;

    private const int INACTIVE = 0;
    private const int ACTIVE = 1;

    public void UpdateData(int level, UILevelHomeController.ENodePos eNodePos)
    {
        EDifficulty eDifficulty = DataManager.Instance.GetLevelDiff(level);

        if (eNodePos == UILevelHomeController.ENodePos.First)
        {
            inActiveNode.SetActive(false);
            activeNode.gameObject.SetActive(true);

            if (eDifficulty == EDifficulty.Normal)
            {
                activeNode.initialSkinName = "normal";
                panelDiff[ACTIVE].gameObject.SetActive(false);
            }
            else
            {
                panelDiff[ACTIVE].gameObject.SetActive(true);

                if (eDifficulty == EDifficulty.Hard)
                {
                    activeNode.initialSkinName = "hard";

                    hardText[ACTIVE].SetActive(true);
                    superHardText[ACTIVE].SetActive(false);
                }
                else
                {
                    activeNode.initialSkinName = "superhard";

                    hardText[ACTIVE].SetActive(false);
                    superHardText[ACTIVE].SetActive(true);
                }
            }

            activeNode.Initialize(true);

            bone.Reset();

            if (CanvasManager.Instance.IsPlayingTransiton())
            {
                Helper.WaitForTransition(() =>
                {
                    activeNode.AnimationState.SetAnimation(0, "Change", false).Complete += (track) =>
                    {
                        activeNode.AnimationState.SetAnimation(0, "Idle", true);
                    };
                });
            }
            else
            {
                activeNode.AnimationState.SetAnimation(0, "Idle", true);
            }

            levelText[ACTIVE].text = level.ToString();
        }
        else
        {
            levelText[INACTIVE].text = level.ToString();

            inActiveNode.SetActive(true);
            activeNode.gameObject.SetActive(false);

            if (eDifficulty == EDifficulty.Normal)
            {
                panelDiff[INACTIVE].gameObject.SetActive(false);
            }
            else
            {
                panelDiff[INACTIVE].gameObject.SetActive(true);

                if (eDifficulty == EDifficulty.Hard)
                {
                    hardText[INACTIVE].SetActive(true);
                    superHardText[INACTIVE].SetActive(false);
                }
                else
                {
                    hardText[INACTIVE].SetActive(false);
                    superHardText[INACTIVE].SetActive(true);
                }
            }
        }
    }

    public void SetTypeRope(UILevelHomeController.ERope eRope)
    {
        TurnRope(eRope);
        RePosition(eRope);
    }

    public void TurnRope(UILevelHomeController.ERope eRope)
    {
        ropeLeft.SetActive(eRope == UILevelHomeController.ERope.Left);
        ropeRight.SetActive(eRope == UILevelHomeController.ERope.Right);
    }

    public void RePosition(UILevelHomeController.ERope eRope)
    {
        container.transform.localPosition = eRope == UILevelHomeController.ERope.Left ? new Vector2(75f, 0f) : new Vector2(-75f, 0f);
    }

}
