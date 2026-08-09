using TMPro;
using UniRx;
using UnityEngine;

public class ElectronicDisplayBoard4 : MonoBehaviour
{
    private TextMeshProUGUI _team1ScoreText;
    private TextMeshProUGUI _team2ScoreText;

    [SerializeField] private NonStaticScoreSystem2 _system;
    
    public void Start()
    {
        _system.SamsungScore.Subscribe(score => OnTeamScoreChanged(ETeam.Samsung, score)).AddTo(this);
        _system.HanwhaScore.Subscribe(score => OnTeamScoreChanged(ETeam.Hanwha, score)).AddTo(this);
    }
    

    private void OnTeamScoreChanged(ETeam team, int score)
    {
        if (team == ETeam.Samsung)
        {
            _team1ScoreText.text = score.ToString();
        }
        else if (team == ETeam.Hanwha)
        {
            _team2ScoreText.text = score.ToString();
        }
    }
}
