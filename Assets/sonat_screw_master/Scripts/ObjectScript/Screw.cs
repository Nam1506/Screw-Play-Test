using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Screw : MonoBehaviour
{
    [SerializeField] private float timeScrewUp = 0.2f;
    [SerializeField] private float timeScrewMove = 0.5f;

    [SerializeField] private SpriteRenderer screwHead;
    [SerializeField] private SpriteRenderer screwTail;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private ParticleSystem onClickEffect;
    [SerializeField] private Transform rotateTransform;
    [SerializeField] private CircleCollider2D circleCollider;

    [Header("Obstacles")]
    [SerializeField] private SkeletonAnimation iceAnim;
    [SerializeField] private SkeletonAnimation gateAnim;
    [SerializeField] private SkeletonAnimation bombAnim;
    [SerializeField] private TextMeshPro bombCountText;
    [SerializeField] private Transform chainParent;
    [SerializeField] private GameObject keyObj;
    [SerializeField] private GameObject lockObj;
    [SerializeField] private ActionMoveParabol actionMove;
    [SerializeField] private ParticleSystem bombEffect;
    [SerializeField] private ParticleSystem colorBombEffect;
    [SerializeField] private ParticleSystem chainEffect;
    [SerializeField] private ParticleSystem unlockEffect;
    [SerializeField] public TMP_Text displayIdentify;

    //[HideInInspector] public int ropeId = -1;

    [Header("Properties")]
    public Shape shape;
    public Hole hole;
    public EScrewColor color;
    public EHoleType type;
    public int identify;

    [Header("Obstacles")]
    public Rope rope;
    public List<Screw> linkedScrews;
    public Screw centerScrew;

    private Sprite normalSprite;

    private bool isMoving = false;
    private bool isMovingFromQueue = false;
    private bool isInBox = false;
    private bool isPulled = false;
    private bool isActiveBomb = false;

    private int iceCount;
    private int bombCount;
    private bool isShowGate;
    private bool isOpenGate;
    private bool isKey;
    private bool isLock;
    private bool isPulledSuccess;

    private float delayIn = 0.06f;
    private float radius;

    private const int MAX_ICE = 3;

    public bool IsMoving => isMoving;
    public bool IsMovingFromQueue => isMovingFromQueue;
    public bool IsFrozen => iceCount > 0;
    public bool IsCloseGate => isShowGate && !isOpenGate;
    public int BombCount => bombCount;
    public bool IsChainLock => linkedScrews.Count > 0;
    public bool IsKey => isKey;
    public bool IsLock => isLock;

    public bool IsBlocking()
    {
        return IsOverlap() || IsCloseGate || IsFrozen || IsLock || IsChainLock;
    }

    public void ShowIndentify()
    {
        displayIdentify.gameObject.SetActive(!displayIdentify.gameObject.activeSelf);
    }

    public void Init(Sprite head, Sprite tail, EScrewColor color, EHoleType type)
    {
        this.transform.localScale = Vector3.one * CheatManager.Instance.GetScaleScrew();

        radius = 0.44f * 1.2f * CheatManager.Instance.GetScalePercent();

        circleCollider.radius = 0.44f * 2f * CheatManager.Instance.GetScalePercent();

        normalSprite = head;

        //screwHead.sprite = head;
        screwTail.sprite = tail;
        this.color = color;
        this.type = type;

        isMoving = false;
        isMovingFromQueue = false;
        isInBox = false;
        sortingGroup.sortingLayerName = "Default";

        rope = null;
        isPulledSuccess = false;

        iceCount = 0;
        iceAnim.gameObject.SetActive(false);

        isShowGate = false;
        gateAnim.gameObject.SetActive(false);

        RemoveBomb(false);

        if (linkedScrews == null)
        {
            linkedScrews = new();
        }
        else
        {
            linkedScrews.Clear();
        }
        centerScrew = null;

        SetKey(false);
        SetLock(false);
    }

    public void SetLayer(int layer)
    {
        //sortingGroup.sortingOrder = layer;
        transform.SetLocalPositionZ(-layer - 0.2f);
    }

    public void SetIce()
    {
        iceCount = MAX_ICE;

        iceAnim.initialSkinName = LevelGenerator.Instance.GetCurScrewSkin().nameSkinIce;

        iceAnim.Initialize(true);

        iceAnim.gameObject.SetActive(true);

        iceAnim.AnimationState.SetAnimation(0, "Idle", false);
    }

    public bool CheckBreakIce()
    {
        if (iceCount == 0) return false;

        iceCount--;
        iceAnim.AnimationState.SetAnimation(0, $"{MAX_ICE - iceCount}", false);

        return true;
    }

    public void BreakAllIce()
    {
        iceCount = 0;
        iceAnim.gameObject.SetActive(false);
    }

    public void SetGate(bool isOpen)
    {
        isShowGate = true;
        isOpenGate = isOpen;

        gateAnim.gameObject.SetActive(true);
        gateAnim.AnimationState.SetAnimation(0, isOpenGate ? "Open" : "Close", false);
    }

    public bool CheckToggleGate()
    {
        if (!isShowGate) return false;

        isOpenGate = !isOpenGate;
        gateAnim.AnimationState.SetAnimation(0, isOpenGate ? "Open" : "Close", false);

        return true;
    }

    public void RemoveGate()
    {
        if (!isShowGate) return;

        isShowGate = false;
        gateAnim.gameObject.SetActive(false);
    }

    public void SetBomb(int count)
    {
        bombCount = count;
        bombCountText.gameObject.SetActive(true);

        UpdateBombCountText();
        screwHead.gameObject.SetActive(false);
        bombAnim.gameObject.SetActive(true);
        SetBombAnim();

        //screwHead.transform.localPosition = BOMB_SPRITE_OFFSET;
        //screwHead.sprite = LevelGenerator.Instance.GetBombSprite(type, color);
    }

    private void UpdateBombCountText()
    {
        bombCountText.text = bombCount.ToString();
    }

    public void CheckBomb()
    {
        if (bombCount == 0) return;

        bombCount--;
        UpdateBombCountText();

        if (bombCount == 0)
        {
            SoundManager.Instance.StopLoopSound();

            SoundManager.Instance.PlaySound(KeySound.Bomb_Explosion);

            var main = colorBombEffect.main;
            main.startColor = Helper.GetColorFromScrew(color);

            bombEffect.Play();

            RemoveBomb(false);

            Debug.Log("bomb exploded");

            GameplayManager.Instance.SetLose(LoseCause.Bomb);
        }
        else if (bombCount == 1)
        {
            bombAnim.AnimationState.SetAnimation(0, "Warning", true);

            if (!GameplayManager.Instance.IsLose)
                SoundManager.Instance.PlaySound(KeySound.Bomb_Count, true);
        }
        else
        {
            bombAnim.AnimationState.SetAnimation(0, "Count_down", false);
            bombAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

            //SoundManager.Instance.PlaySound(KeySound.Bomb_Count);
        }
    }

    public void CheckActiveBomb()
    {
        if (bombCount == 0) return;

        if (!IsOverlap())
        {
            if (!isActiveBomb)
            {
                isActiveBomb = true;

                if (bombCount == 1)
                {
                    bombAnim.AnimationState.SetAnimation(0, "Warning", true);
                    SoundManager.Instance.PlaySound(KeySound.Bomb_Count, true);
                }
                else
                {
                    bombAnim.AnimationState.SetAnimation(0, "Idle", true);

                    Helper.WaitForTransition(() => SoundManager.Instance.PlaySound(KeySound.Bomb_Start));
                }
            }
        }
        else
        {
            isActiveBomb = false;
            bombAnim.AnimationState.SetAnimation(0, "Idle_0", false);
        }
    }

    private void RemoveBomb(bool isPlaySound)
    {
        bombCount = 0;
        bombCountText.gameObject.SetActive(false);

        isActiveBomb = false;

        screwHead.gameObject.SetActive(true);
        bombAnim.gameObject.SetActive(false);

        screwHead.sprite = normalSprite;
        screwHead.transform.localPosition = Vector3.zero;
        screwHead.DOKill();

        if (isPlaySound)
        {
            if (!GameplayManager.Instance.HasOneValueBomb())
            {
                SoundManager.Instance.StopLoopSound();
            }

            SoundManager.Instance.PlaySound(KeySound.Bomb_Remove);
        }
    }

    public void SetChain(int num)
    {
        for (int i = 0; i < chainParent.childCount; i++)
        {
            chainParent.GetChild(i).gameObject.SetActive(i < num);
        }
    }

    public void CheckChain()
    {
        if (centerScrew != null)
        {
            foreach (Screw screw in centerScrew.linkedScrews)
            {
                if (screw == this)
                {
                    SoundManager.Instance.PlaySound(KeySound.Chain_Remove);

                    ChainGenerator.Instance.RemoveChain(screw);
                    centerScrew.linkedScrews.Remove(screw);
                    centerScrew.PlayChainBreakEffect();
                    centerScrew.SetChain(centerScrew.linkedScrews.Count);
                    centerScrew = null;
                    return;
                }
            }
        }
    }

    public void PlayChainBreakEffect()
    {
        chainEffect.Play();
    }

    public void SetKey(bool b)
    {
        isKey = b;
        keyObj.SetActive(b);
    }

    public void SetLock(bool b)
    {
        isLock = b;
        lockObj.SetActive(b);
    }

    private void CheckKey()
    {
        if (!isKey) return;

        Screw lockedScrew = ObstacleController.Instance.GetLockedSrew();

        Debug.Log(lockedScrew);

        if (lockedScrew == null)
        {
            Debug.LogError("key doesn't match any lock");
            return;
        }

        isKey = false;

        ObstacleController.Instance.RemoveLockedScrew(lockedScrew);

        DOVirtual.DelayedCall(timeScrewUp, () =>
        {
            keyObj.transform.parent = null;
            actionMove.SetEndPoint(lockedScrew.transform);
            actionMove.StartAction(() =>
            {
                SoundManager.Instance.PlaySound(KeySound.Key_Lock);

                keyObj.SetActive(false);
                lockedScrew.PlayUnlockEffect();
                lockedScrew.SetLock(false);

                keyObj.transform.parent = transform;
                keyObj.transform.localEulerAngles = Vector3.zero;
                keyObj.transform.localPosition = new Vector3(0, -0.7f, 0);
            });
        });
    }

    public void PlayUnlockEffect()
    {
        unlockEffect.Play();
    }

    public void OnScrewClick()
    {
        if (isMoving) Debug.Log("Moving");
        if (isInBox) Debug.Log("in box");
        if (hole is HoleQueue) Debug.Log("in queue");

        if (isMoving || isInBox || (hole is HoleQueue) || GameplayManager.Instance.IsLose || iceCount > 0 || (isShowGate && !isOpenGate)
            || IsChainLock || isLock)
        {
            VibrateManager.Instance.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);

            return;
        }

        if (BoosterManager.Instance.IsUsingHammer)
        {
            if (!shape.IsBroken)
            {
                shape.Break();
            }
            return;
        }

        SoundManager.Instance.PlaySound(KeySound.ScrewPick);

        VibrateManager.Instance.Vibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);

        onClickEffect.Play();

        bool moveSuccess = MoveSomeWhere();

        if (moveSuccess)
        {
            GameplayManager.Instance.moveCount++;

            CheckRemoveObstacles();

            GameplayManager.Instance.CheckObstacles(this);

            if (isPulledSuccess)
            {
                if (rope.topScrew == this)
                {
                    rope.bottomScrew.MoveSomeWhere(true);
                    rope.bottomScrew.CheckRemoveObstacles();
                }
                else if (rope.bottomScrew == this)
                {
                    rope.topScrew.MoveSomeWhere(true);
                    rope.topScrew.CheckRemoveObstacles();
                }

                SoundManager.Instance.PlaySound(KeySound.Rope);

                isPulledSuccess = false;
            }

            GameplayManager.Instance.CheckObstaclesLose();
        }
        else
        {
            StartCoroutine(IeFakeMove());
        }
    }

    public void CheckRemoveObstacles()
    {
        RemoveGate();

        if (bombCount > 0)
        {
            RemoveBomb(true);
        }

        CheckChain();

        CheckKey();
    }

    public bool MoveSomeWhere(bool isPulled = false)
    {
        this.isPulled = isPulled;

        bool success;

        if (!CheckAvailableHoleInBox(false))
        {
            // no avaible hole, move screw to queue
            success = MoveToQueue();
        }
        else
        {
            success = true;
        }

        return success;
    }

    private bool MoveToQueue()
    {
        Debug.Log("Move to queue");

        int emptyHole = HolesQueueController.Instance.GetHoleQueueEmptyIndex();

        if (emptyHole >= 0)
        {
            if (rope != null && !isPulled)
            {
                if (!CheckRope())
                {
                    return false;
                }
                else
                {
                    if (rope.topScrew == this)
                    {
                        if (rope.bottomScrew.CanMoveToBox() || HolesQueueController.Instance.GetRemainEmptyHole() >= 2)
                        {
                            isPulledSuccess = true;
                        }
                        else return false;
                    }
                    else if (rope.bottomScrew == this)
                    {
                        if (rope.topScrew.CanMoveToBox() || HolesQueueController.Instance.GetRemainEmptyHole() >= 2)
                        {
                            isPulledSuccess = true;
                        }
                        else return false;
                    }
                }
            }

            shape.CheckRemoveHinge(this);
            shape.RemoveScrewFromHole(this);

            HolesQueueController.Instance.holesQueue[emptyHole].screw = this;
            hole = HolesQueueController.Instance.holesQueue[emptyHole];

            transform.SetParent(hole.transform);
            sortingGroup.sortingLayerName = "Screw";

            ScrewOut();

            isMoving = true;

            BoosterManager.Instance.BlockBooster(EBoosterType.AddHole, true);

            if (!GameplayManager.Instance.HasMovingBox())
            {
                Debug.Log("Check lose");
                GameplayManager.Instance.CheckLose();
            }

            var newPos = HolesQueueController.Instance.holesQueue[emptyHole].transform.position;

            newPos.z = -11f;

            StartCoroutine(IeMoveToQueue(newPos));

            HolesQueueController.Instance.CheckWarning();

            return true;
        }
        else
        {
            VibrateManager.Instance.Vibrate(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);

            return false;
        }
    }

    private IEnumerator IeFakeMove()
    {
        isMoving = true;

        transform.DOMoveY(transform.position.y + 0.75f, timeScrewUp).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(timeScrewUp);

        transform.DOMoveY(transform.position.y - 0.75f, timeScrewUp).SetEase(Ease.InOutQuint);

        yield return new WaitForSeconds(timeScrewUp);

        isMoving = false;
    }

    private IEnumerator IeMoveToQueue(Vector3 holePos)
    {
        //CheckRope();

        yield return new WaitForSeconds(timeScrewUp);

        transform.DOMove(holePos, timeScrewMove).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(timeScrewMove + delayIn);

        ScrewIn();

        yield return new WaitForSeconds(timeScrewUp);

        isMoving = false;

        if (rope != null)
        {
            rope.DestroySelf();
            rope = null;
        }

        BoosterManager.Instance.BlockBooster(EBoosterType.AddHole, false);

        GameplayManager.Instance.CheckPutScrewInQueueToBox();

        TutorialManager.Instance.HideTut();
    }

    public bool CheckRope()
    {
        if (rope.topScrew == this && !rope.bottomScrew.IsOverlap())
        {
            if (!rope.bottomScrew.IsCloseGate && !rope.bottomScrew.IsFrozen && !rope.bottomScrew.IsLock)
                return true;
        }
        else if (rope.bottomScrew == this && !rope.topScrew.IsOverlap())
        {
            if (!rope.topScrew.IsCloseGate && !rope.topScrew.IsFrozen && !rope.topScrew.IsLock)
                return true;
        }

        return false;
    }

    public bool CheckAvailableHoleInBox(bool isMovingFromQueue)
    {
        List<Box> activeBoxes = GameplayManager.Instance.GetActiveBoxes();

        foreach (Box box in activeBoxes)
        {
            if (box.eColor != color)
            {
                continue;
            }

            foreach (BoxHole hole in box.holes)
            {
                if (hole.IsEmpty && type == hole.type)
                {
                    // move screw to box

                    if (!isMovingFromQueue)
                    {
                        if (rope != null && !isPulled)
                        {
                            if (!CheckRope())
                            {
                                return false;
                            }
                            else
                            {
                                isPulledSuccess = true;
                            }
                        }

                        shape.CheckRemoveHinge(this);
                        shape.RemoveScrewFromHole(this);
                    }

                    hole.screw = this;

                    this.hole = hole;

                    this.isMovingFromQueue = isMovingFromQueue;

                    var pos = hole.transform.position;
                    pos.z = -11f;

                    StartCoroutine(IePutScrewToBox(box, pos, isPulledSuccess));

                    if (box.IsComplete())
                        GameplayManager.Instance.CheckWin();

                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator IePutScrewToBox(Box box, Vector3 holePosition, bool isPulledSuccess = false)
    {
        //isMovingToBox = true;

        while (isMoving)
        {
            yield return null;
        }

        transform.SetParent(box.transform);
        sortingGroup.sortingLayerName = "Screw";

        ScrewOut();

        isMoving = true;

        BoosterManager.Instance.BlockBooster(EBoosterType.AddBox, true);

        StartCoroutine(IeMoveToBox(box, holePosition));
    }

    private IEnumerator IeMoveToBox(Box box, Vector3 holePosition)
    {
        yield return new WaitForSeconds(timeScrewUp);

        bool isChangeEndPos = false;

        transform.DOMove(holePosition, timeScrewMove).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            if (box.IsMoving && !isChangeEndPos)
            {
                transform.DOKill();

                transform.position = hole.transform.position;

                isChangeEndPos = true;
            }
        });

        yield return new WaitForSeconds(timeScrewMove + delayIn);

        ScrewIn();

        yield return new WaitForSeconds(timeScrewUp);

        isMoving = false;
        isMovingFromQueue = false;
        isInBox = true;

        if (rope != null)
        {
            rope.DestroySelf();
            rope = null;
        }

        if (hole.type != EHoleType.Normal)
        {
            (hole as BoxHole).SetNormalHole();
        }

        BoosterManager.Instance.BlockBooster(EBoosterType.AddBox, false);

        box.SetNextVisualFilled();

        if (box.IsAllVisualFilled())
        {
            box.PackingBox();
        }

        TutorialManager.Instance.HideTut();
    }

    public void ScrewOut()
    {
        sortingGroup.sortingOrder = 1;
        // dotween rotate 90 degrees on z axis of screwHead by clockwise
        screwHead
            .transform.DORotate(new Vector3(0, 0, 180), timeScrewUp)
            .SetEase(Ease.InOutSine);
        // up screwHead 0.75 units on y axis
        screwHead
            .transform.DOMoveY(screwHead.transform.position.y + 0.75f, timeScrewUp)
            .SetEase(Ease.InOutSine);
        // up screwTail 0.25 units on y axis
        screwTail
            .transform.DOMoveY(screwTail.transform.position.y + 0.25f, timeScrewUp)
            .SetEase(Ease.InOutSine);
    }

    public void ScrewIn()
    {
        // dotween rotate 90 degrees on z axis of screwHead by counter clockwise
        screwHead.transform.DORotate(new Vector3(0, 0, 0), timeScrewUp).SetEase(Ease.InOutSine);
        // down screwHead 0.75 units on y axis
        screwHead
            .transform.DOMoveY(screwHead.transform.position.y - 0.75f, timeScrewUp)
            .SetEase(Ease.InOutQuint);
        // down screwTail 0.25 units on y axis
        screwTail
            .transform.DOMoveY(screwTail.transform.position.y - 0.25f, timeScrewUp)
            .SetEase(Ease.InOutQuint);

        sortingGroup.sortingOrder = 0;
    }

    private void LateUpdate()
    {
        transform.localEulerAngles = Vector3.zero;
        rotateTransform.eulerAngles = Vector3.zero;

        CheckActiveBomb();
    }

    public bool IsOverlap()
    {
        List<Collider2D> results = Physics2D.OverlapCircleAll(transform.position, radius).ToList();
        List<Shape> otherShapes = new List<Shape>();

        foreach (Collider2D collider in results)
        {
            if (collider.TryGetComponent(out Shape temp))
            {
                otherShapes.Add(temp);
            }
        }

        foreach (Shape shape in otherShapes)
        {
            if (shape.Layer > this.shape.Layer)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanMoveToBox()
    {
        List<Box> activeBoxes = GameplayManager.Instance.GetActiveBoxes();

        foreach (Box box in activeBoxes)
        {
            if (box.IsComplete() || box.eColor != color) continue;

            foreach (BoxHole hole in box.holes)
            {
                if (hole.IsEmpty && type == hole.type)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetBombAnim()
    {
        switch (color)
        {
            case EScrewColor.Blue:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Blue" : "Blue2";
                break;
            case EScrewColor.Green:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Green" : "Green2";
                break;
            case EScrewColor.Ocean_Blue:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "OceanBlue" : "OceanBlue2";
                break;
            case EScrewColor.OldGreen:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "OldGreen" : "OldGreen2";
                break;
            case EScrewColor.Orange:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Orange" : "Orange2";
                break;
            case EScrewColor.Pink:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Pink" : "Pink2";
                break;
            case EScrewColor.Purple:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Purple" : "Purple2";
                break;
            case EScrewColor.Red:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Red" : "Red2";
                break;
            case EScrewColor.Violet:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Violet" : "Violet2";
                break;
            case EScrewColor.Yellow:
                bombAnim.initialSkinName = type == EHoleType.Normal ? "Yellow" : "Yellow2";
                break;
        }

        bombAnim.initialSkinName += LevelGenerator.Instance.GetCurScrewSkin().nameSkinBomb;

        bombAnim.Initialize(true);
    }
}
