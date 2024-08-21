using System.Collections.Generic;
using DarkTonic.PoolBoss;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    [SerializeField] private Image boxImage;
    [SerializeField] private Transform holeBoxPrefab;
    [SerializeField] private Transform holeBoxParent;
    [SerializeField] private Image glass;
    [SerializeField] private ParticleSystem packedEffect;
    [SerializeField] private SkeletonGraphic tutAnim;

    [SerializeField] private HoleSO holeSO;

    public List<BoxHole> holes = new List<BoxHole>();

    public EScrewColor eColor;

    public int holeCount = 3;

    public RectTransform rectTransform;

    private float holeSpace = DEFAULT_HOLE_SPACE;
    private float holeWidth = 43;

    // to check if hole is visual filled after screw is put in
    private bool[] isVisualFilled;
    [SerializeField] private bool isMoving = false;

    private const float DEFAULT_HOLE_SPACE = 35f;
    private const float GLASS_OFFSET_Y = 250f;

    public bool IsMoving => isMoving;
    [field: SerializeField] public bool IsReady { get; set; }
    public bool IsPacked { get; set; }

    public bool IsComplete()
    {
        foreach (BoxHole hole in holes)
        {
            if (hole.IsEmpty)
                return false;
        }

        return true;
    }

    public void Init(EScrewColor eColor, int holeCount)
    {
        this.eColor = eColor;
        this.holeCount = holeCount;

        holes.Clear();

        var boxSO = BoxController.Instance.GetCurSkin();

        boxImage.sprite = boxSO.GetSprite(eColor, holeCount);
        boxImage.SetNativeSize();

        glass.sprite = boxSO.GetGlassSprite(holeCount);
        glass.SetNativeSize();

        holeBoxParent.GetComponent<RectTransform>().anchoredPosition = boxSO.GetOffset();

        //glass.color = boxSO.GetGlassColor(eColor);

        SetupHoles();

        isVisualFilled = new bool[holeCount];
        for (int i = 0; i < isVisualFilled.Length; i++)
        {
            isVisualFilled[i] = false;
        }

        glass.gameObject.SetActive(false);

        IsReady = false;
        IsPacked = false;
    }

    private void SetupHoles()
    {
        SetHolePosition();
    }

    private void SetHolePosition()
    {
        bool isHoleCountEvenNumber = holeCount % 2 == 0;

        for (int i = 0; i < holeCount; i++)
        {
            BoxHole hole = PoolBoss.Spawn(holeBoxPrefab, transform.position, Quaternion.identity, holeBoxParent).GetComponent<BoxHole>();

            hole.transform.localScale = Vector3.one;

            RectTransform rect = hole.GetComponent<RectTransform>();

            if (isHoleCountEvenNumber)
            {
                float posX = (holeSpace + holeWidth) / 2 + (holeSpace + holeWidth) * (holeCount / 2 - i - 1);
                rect.anchoredPosition = new Vector2(posX, 0);
            }
            else
            {
                if (i == (holeCount - 1) / 2)
                {
                    rect.anchoredPosition = new Vector2(0, 0);
                }
                else
                {
                    rect.anchoredPosition = new Vector2((holeWidth + holeSpace) * (i - (holeCount - 1) / 2), 0);
                }
            }

            holes.Add(hole);
        }
    }

    public void SetHoleType(int holeIndex)
    {
        if (holeIndex < 0 || holeIndex >= holes.Count) return;

        EHoleType holeType = holes[holeIndex].type;

        holes[holeIndex].SetHoleImage(holeSO.GetSprite(holeType, eColor));

        RectTransform rect = holes[holeIndex].GetComponent<RectTransform>();

        if (holeType == EHoleType.Star)
        {
            rect.pivot = new Vector2(0.5f, 0.45f);
        }
        else
        {
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
    }

    public void PackingBox()
    {
        BoosterManager.Instance.BlockBooster(EBoosterType.AddBox, true);

        RectTransform glassRect = glass.GetComponent<RectTransform>();

        glassRect.anchoredPosition = new Vector2(0, GLASS_OFFSET_Y);

        glass.gameObject.SetActive(true);

        IsPacked = true;

        SoundManager.Instance.PlaySound(KeySound.BoxDone);

        glassRect.DOAnchorPos(Vector2.zero, 0.15f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            packedEffect.Play();

            VibrateManager.Instance.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);

            if (GameplayManager.Instance.AreAllCurrentBoxesPacked())
            {
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    GameplayManager.Instance.MoveAllCurrentBoxes();

                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        BoxController.Instance.OnNext();
                    });
                });
            }
            else
            {
                if (!IsLeftAciveBox())
                {
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        MoveBoxAway();

                        if (GameplayManager.Instance.IsAddingBox)
                        {
                            GameplayManager.Instance.boxes[0].isMoving = true;
                        }

                        DOVirtual.DelayedCall(0.2f, () =>
                        {
                            BoxController.Instance.OnNext();
                        });
                    });
                }
            }
        });

        transform.DOScaleY(0.7f, 0.085f).SetDelay(0.1f).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
    }

    public void MoveBoxAway()
    {
        GameplayManager.Instance.boxes.Remove(this);

        isMoving = true;

        Vector2 endPos = new Vector2(Screen.width / 2 + rectTransform.sizeDelta.x + Mathf.Abs(rectTransform.anchoredPosition.x),
            rectTransform.anchoredPosition.y);

        float distance = Vector2.Distance(endPos, rectTransform.anchoredPosition);
        float speed = Screen.width * 2f;

        rectTransform.DOAnchorPos(endPos, distance / speed).SetEase(Ease.OutSine).OnComplete(() =>
        {
            isMoving = false;

            ResetData();

            PoolBoss.Despawn(transform);
        });
    }

    public void MoveToPosition(Vector2 anchorPos, bool isReady)
    {
        isMoving = true;

        rectTransform.DOAnchorPos(anchorPos, 0.36f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);

            isMoving = false;
            IsReady = isReady;

            if (isReady)
            {
                GameplayManager.Instance.CheckPutScrewInQueueToBox();
            }
        });
    }

    public bool IsAllVisualFilled()
    {
        for (int i = 0; i < isVisualFilled.Length; i++)
        {
            if (!isVisualFilled[i])
            {
                return false;
            }
        }
        return true;
    }

    public void SetNextVisualFilled()
    {
        for (int i = 0; i < isVisualFilled.Length; i++)
        {
            if (!isVisualFilled[i])
            {
                isVisualFilled[i] = true;
                break;
            }
        }
    }

    public void ResetData()
    {
        glass.gameObject.SetActive(false);

        foreach (var hole in holes)
        {
            if (hole.screw != null)
            {
                GameplayManager.Instance.screws.Remove(hole.screw);

                PoolBoss.Despawn(hole.screw.transform);
            }

            PoolBoss.Despawn(hole.transform);
        }

        holes.Clear();
    }

    public void PlayTutAnim()
    {
        tutAnim.gameObject.SetActive(true);
        tutAnim.Initialize(true);
        tutAnim.AnimationState.SetAnimation(0, "Box", true);
    }

    public void StopTutAnim()
    {
        tutAnim.gameObject.SetActive(false);
    }

    private bool IsLeftAciveBox()
    {
        List<Box> activeBoxes = GameplayManager.Instance.GetActiveBoxes();

        if (activeBoxes.Count < 2 || !activeBoxes.Contains(this)) return false;

        if (activeBoxes.IndexOf(this) > 0)
        {
            return true;
        }

        return false;
    }
}
