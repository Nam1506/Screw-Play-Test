using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class RaceRoad : MonoBehaviour
{
    [SerializeField] private RaceItem raceItem;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private SkeletonGraphic racerAnim;
    [SerializeField] private RectTransform raceGround;

    [SerializeField] private ParticleSystem digEffect;

    private const float DEFAULT_WIDTH_GROUND = 250f;

    private float _race = 0f;

    public void SetData(RaceMember member, int rank)
    {
        Reset();

        raceItem.SetData(member, rank);

        StartCoroutine(IEMove(member));
    }

    private IEnumerator IEMove(RaceMember member)
    {
        yield return new WaitForSeconds(0.15f);

        float race = Mathf.Min(member.level - (RaceEvent.raceData.winLevelTarget - RaceEvent.RACE_RANGE), RaceEvent.RACE_RANGE) / 15f;

        if (race == 1)
        {
            race = 2f;
        }

        float s = rectTransform.rect.width * race;

        raceItem.transform.DOLocalMoveX(s, 0.667f * 2f);
        raceGround.DOSizeDelta(new Vector2(DEFAULT_WIDTH_GROUND + s, raceGround.sizeDelta.y), 0.667f * 2f);

        racerAnim.Initialize(true);
        racerAnim.AnimationState.SetAnimation(0, "Run", false);
        racerAnim.AnimationState.AddAnimation(0, "Run", false, 0);
        racerAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        if (_race != race)
        {
            _race = race;

            digEffect.Play();

            DOVirtual.DelayedCall(0.667f * 1.5f, () =>
            {
                digEffect.Stop();
            });
        }

    }

    public void Reset()
    {
        raceItem.transform.SetLocalPositionX(0);
        raceGround.SetSizeDeltaX(DEFAULT_WIDTH_GROUND);
        _race = 0f;
    }
}
