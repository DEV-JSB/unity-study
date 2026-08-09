using UnityEngine;

public class Tourist3 : MonoBehaviour
{
    [SerializeField] private ETeam _team;
    
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
        if (_system == null)
        {
            return;
        }
        _system.OnTeam1ScoreChanged -= OnTeamScoreChanged;
        _system.OnTeam2ScoreChanged -= OnTeamScoreChanged;
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
