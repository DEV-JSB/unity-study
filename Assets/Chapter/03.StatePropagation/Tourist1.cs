using System;
using UnityEngine;

public class Tourist1 : MonoBehaviour
{
    [SerializeField] private ETeam _team;
    
    
    private ElectronicDisplayBoard1 _board;
    
    public void Init(ElectronicDisplayBoard1 board)
    {
        _board = board;
        board.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        board.OnTeam1ScoreChanged += OnTeamScoreChanged;

        board.OnTeam2ScoreChanged -= OnTeamScoreChanged;
        board.OnTeam2ScoreChanged += OnTeamScoreChanged;
    }


    private void OnDestroy()
    {
        if (_board == null)
        {
            return;
        }
        _board.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        _board.OnTeam2ScoreChanged -= OnTeamScoreChanged;
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
