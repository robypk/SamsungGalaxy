using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCard : MonoBehaviour
{
    [SerializeField] TMP_Text level;
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text turn;


    public void SetScoreCard(int level, int score, int turn)
    {
        this.level.text =  level.ToString();
        this.score.text =  score.ToString();
        this.turn.text =  turn.ToString();
    }
}
