using System;
using UnityEngine;

public class NonStaticScoreSystem : MonoBehaviour
{
    private int _team1Score = 0;
    private int _team2Score = 0;


    public event Action<ETeam, int> OnTeam1ScoreChanged;
    public event Action<ETeam, int> OnTeam2ScoreChanged;


    public void AddTeamScore(ETeam team, int score)
    {
        if (team == ETeam.Samsung)
        {
            _team1Score += score;
            if (OnTeam1ScoreChanged != null)
            {
                OnTeam1ScoreChanged.Invoke(ETeam.Samsung, _team1Score);
            }
        }
        else
        {
            _team2Score += score;
            if (OnTeam2ScoreChanged != null)
            {
                OnTeam2ScoreChanged.Invoke(ETeam.Hanwha, _team2Score);
            }
        }
    }
}
