using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum DifficultyValue
    {
        Easy = 1,
        Hard = 2,
        Suicide = 3
    }

    public static GameManager Instance;

    private int _currentDifficultyValue = 1;
    private int _currentTimeInSeconds = 60;
    private const int _timerChangeDelay = 1;
    private bool _isRoundStillGoing = true;

    public int CurrentDifficultyValue { 
        get => _currentDifficultyValue;
        private set => _currentDifficultyValue = Mathf.Clamp(value, (int)DifficultyValue.Easy, (int)DifficultyValue.Suicide);
    }

    public int TimerInSeconds
    {
        get => _currentTimeInSeconds;
        private set => _currentTimeInSeconds = Mathf.Clamp(value, 0, 60);
    }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        while (_isRoundStillGoing)
        {
            yield return new WaitForSeconds(_timerChangeDelay);
            TimerInSeconds--;
            print(TimerInSeconds);

            CheckLoseCondition();
        }
    }

    private void CheckLoseCondition()
    {
        if (Mathf.Approximately(TimerInSeconds, 0))
        {
            StopGame();
            print("GAME LOSE");
        }
    }

    public void DecreaseDifficulty() => CurrentDifficultyValue--;
    public void IncreaseDifficulty() => CurrentDifficultyValue++;
    public void StopGame() => _isRoundStillGoing = false;
    public void AddToTime(int x) => TimerInSeconds += x;
}
