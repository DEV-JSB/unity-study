using UniRx;
using UnityEngine;

public class Tourist5 : MonoBehaviour
{
    [SerializeField] private ETeam _team;

    [SerializeField] private ReactiveScoreSystemAsset _system;

    private void Start()
    {
        _system.SamsungScore.Subscribe(score => OnTeamScoreChanged(ETeam.Samsung, score)).AddTo(this);
        _system.HanwhaScore.Subscribe(score => OnTeamScoreChanged(ETeam.Hanwha, score)).AddTo(this);
    }

    private void OnTeamScoreChanged(ETeam team, int score)
    {
        if (team == _team)
        {
            Debug.Log($"그르취 짜란다짜란다 {score} 우효~");
        }
        else
        {
            Debug.Log($"뭐하냐고!!!!!!!!!!!!! {score}가 말이냐고!!");
        }
    }
}
