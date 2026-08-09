using TMPro;
using UnityEngine;

public class ElectronicDisplayBoard3 : MonoBehaviour
{
    private TextMeshProUGUI _team1ScoreText;
    private TextMeshProUGUI _team2ScoreText;

    [SerializeField] private NonStaticScoreSystem _system;
    
    public void Start()
    {
        _system.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        _system.OnTeam1ScoreChanged += OnTeamScoreChanged;

        _system.OnTeam2ScoreChanged -= OnTeamScoreChanged;
        _system.OnTeam2ScoreChanged += OnTeamScoreChanged;
    }


    private void OnDestroy()
    {
        _system.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        _system.OnTeam2ScoreChanged -= OnTeamScoreChanged;    
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
