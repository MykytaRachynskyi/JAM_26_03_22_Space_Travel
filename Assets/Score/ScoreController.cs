using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    [SerializeField] int maxScore;
    [SerializeField] UnityEvent onWin;
    [SerializeField] TMPro.TMP_Text scoreLabel;

    int currentScore = 0;

    void Start()
    {
        scoreLabel.text = $"{currentScore}/{maxScore}";
    }

    public void AddScore()
    {
        currentScore++;
        scoreLabel.text = $"{currentScore}/{maxScore}";
        if (currentScore >= maxScore)
        {
            Win();
        }
    }

    void Win()
    {
        onWin?.Invoke();
    }
}
