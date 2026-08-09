using System;
using UnityEngine;

public class Tourist2 : MonoBehaviour
{
    
    [SerializeField] private ETeam _team;

    private void Start()
    {
        
        ScoreSystem.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        ScoreSystem.OnTeam1ScoreChanged += OnTeamScoreChanged;

        ScoreSystem.OnTeam2ScoreChanged -= OnTeamScoreChanged;
        ScoreSystem.OnTeam2ScoreChanged += OnTeamScoreChanged;
    }
    
    

    private void OnDestroy()
    {
        ScoreSystem.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        ScoreSystem.OnTeam2ScoreChanged -= OnTeamScoreChanged;
    }

    private void OnTeamScoreChanged(ETeam board, int score)
    {
        if (board == _team)
        {
            Debug.Log($"그르취 짜란다짜란다 {score} 우효~");
        }
        else
        {
            Debug.Log($"뭐하냐고!!!!!!!!!!!!! {score}가 말이냐고!!");
        }
    }
    
}
