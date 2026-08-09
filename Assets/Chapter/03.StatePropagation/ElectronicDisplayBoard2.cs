using System;
using TMPro;
using UnityEngine;

public class ElectronicDisplayBoard2 : MonoBehaviour
{
    
    private TextMeshProUGUI _team1ScoreText;
    private TextMeshProUGUI _team2ScoreText;

    private void Start()
    {
        ScoreSystem.OnTeam1ScoreChanged += OnTeamScoreChanged;
        ScoreSystem.OnTeam2ScoreChanged += OnTeamScoreChanged;
    }

    private void OnDestroy()
    {

        ScoreSystem.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        ScoreSystem.OnTeam2ScoreChanged -= OnTeamScoreChanged;    
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
