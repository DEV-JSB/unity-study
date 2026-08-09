using System;
using UnityEngine;
using UniRx;


public class NonStaticScoreSystem2 : MonoBehaviour
{
    private readonly ReactiveProperty<int> _hanwhaScore = new ReactiveProperty<int>();
    private readonly ReactiveProperty<int> _samsungScore = new ReactiveProperty<int>();
    
    
    public IReadOnlyReactiveProperty<int> HanwhaScore => _hanwhaScore;
    public IReadOnlyReactiveProperty<int> SamsungScore => _samsungScore;

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
}
