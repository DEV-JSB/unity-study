using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ETeam
{
    Samsung,
    Hanwha
}

public class ElectronicDisplayBoard1 : MonoBehaviour
{
    private int _team2Score;
    private int _team1Score;

    public event Action<ETeam, int> OnTeam1ScoreChanged;
    public event Action<ETeam, int> OnTeam2ScoreChanged;

    [SerializeField] private List<Tourist1> _tourist;

    private TextMeshProUGUI _team1ScoreText;
    private TextMeshProUGUI _team2ScoreText;

    
    private void Start()
    {
        foreach (var tourist in _tourist)
        {
            tourist.Init(this);
        }
        
        OnTeam1ScoreChanged += OnTeamScoreChanged;
        OnTeam2ScoreChanged += OnTeamScoreChanged;
    }

    private void OnDestroy()
    {
        OnTeam1ScoreChanged -= OnTeamScoreChanged;
        OnTeam2ScoreChanged -= OnTeamScoreChanged;
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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ++_team1Score;
            if (OnTeam1ScoreChanged != null)
            {
                OnTeam1ScoreChanged.Invoke(ETeam.Samsung, _team1Score);
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            ++_team2Score;
            if (OnTeam2ScoreChanged != null)
            {
                OnTeam2ScoreChanged.Invoke(ETeam.Hanwha, _team2Score);
            }
        }
    }
}
