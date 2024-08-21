using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceItem : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private PlayerSO playerSO;

    public void SetData(RaceMember member, int rank)
    {
        if (rankText)
            rankText.text = $"#{rank}";

        if (nameText)
            nameText.text = member.name;

        if (avatar)
        {
            if (rank == 3)
            {
                avatar.sprite = DataManager.Instance.GetMyAvt();
            }
            else
            {
                avatar.sprite = playerSO.GetAvatar(member.avatarId);

            }

        }

        if (levelText)
            levelText.text = Mathf.Min(member.level - (RaceEvent.raceData.winLevelTarget - RaceEvent.RACE_RANGE), RaceEvent.RACE_RANGE).ToString();
    }
}
