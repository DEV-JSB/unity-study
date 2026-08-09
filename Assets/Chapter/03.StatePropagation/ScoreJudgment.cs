using UnityEngine;

public class ScoreJudgment : MonoBehaviour
{
    
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScoreSystem.AddTeamScore(ETeam.Samsung,1);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            ScoreSystem.AddTeamScore(ETeam.Hanwha,1);
        }
    }
}
