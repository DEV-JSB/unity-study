using System;
using UniRx;
using UnityEngine;


[CreateAssetMenu(fileName = "ScoreSystem", menuName = "Study/ScoreSystem")]
public class ReactiveScoreSystemAsset : ScriptableObject
{
    private ReactiveProperty<int> _hanwhaScore;
    private ReactiveProperty<int> _samsungScore;
    
    
    public IReadOnlyReactiveProperty<int> HanwhaScore => _hanwhaScore;
    public IReadOnlyReactiveProperty<int> SamsungScore => _samsungScore;

    private void OnEnable()
    {
        // SO 로 쓰면 에셋이 로드될 때 , 
        _hanwhaScore = new ReactiveProperty<int>(0);
        _samsungScore = new ReactiveProperty<int>(0);
    }

    public void AddScore(ETeam team, int score)
    {
        if (team == ETeam.Samsung)
        {
            _samsungScore.Value += score;
        }
        else
        {
            _hanwhaScore.Value += score;
        }
    }

    private void OnDestroy()
    {
        _hanwhaScore.Dispose();
        _samsungScore.Dispose();
    }

    private void OnDisable()
    {
        if(_hanwhaScore != null)
        {
            _hanwhaScore.Dispose();
        }

        if (_samsungScore != null)
        {
            _samsungScore.Dispose();
        }
    }
}
