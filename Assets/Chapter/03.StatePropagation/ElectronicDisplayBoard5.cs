using TMPro;
using UniRx;
using UnityEngine;

public class ElectronicDisplayBoard5 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _samsungScoreText;
    [SerializeField] private TextMeshProUGUI _hanwhaScoreText;

    [SerializeField] private ReactiveScoreSystemAsset _system;

    private void Start()
    {
        _system.SamsungScore.Subscribe(score => OnTeamScoreChanged(ETeam.Samsung, score)).AddTo(this);
        _system.HanwhaScore.Subscribe(score => OnTeamScoreChanged(ETeam.Hanwha, score)).AddTo(this);
    }

    private void OnTeamScoreChanged(ETeam team, int score)
    {
        if (team == ETeam.Samsung)
        {
            _samsungScoreText.text = score.ToString();
        }
        else if (team == ETeam.Hanwha)
        {
            _hanwhaScoreText.text = score.ToString();
        }
    }
}
