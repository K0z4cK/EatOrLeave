using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeysMinigame : MonoBehaviour
{
    [SerializeField] private GameObject[] keyPrefabs; 
    [SerializeField] private float timeLimit;
    [SerializeField] private Image[] timerLines;
    [SerializeField] private List<Arrow> _arrows;

    private List<Arrow> _keySequence; 
    private float _timer;
    private const float _difficultyCoef = 0.25f;
    private bool _isQTEActive;
    private int _sequenceIndex = 0;
    private readonly int[] _currentSequenceKeysCount = { 4, 5, 6 };

    private bool _isMinigameFinished = false;
    private int _timesMinigameWasFinished = 0;
    private int _minigamesFinishedLimit = 3;

    public void Init()
    {
        _isMinigameFinished = false;
        StartQTE();
    }

    private void StartQTE()
    {
        // Reset timer and flag
        _timer = timeLimit - _difficultyCoef * GameManager.Instance.CurrentDifficultyValue;
        _isQTEActive = true;
        _sequenceIndex = 0;
        ResetTimerImage();
        ResetKeysObjects();

        _keySequence = GenerateRandomKeySequence();
    }

    private void FixedUpdate()
    {
        if (!_isQTEActive)
            return;

        _timer -= Time.fixedDeltaTime;
        ChangeTimerImage();
    }

    private void Update()
    {
        if (!_isQTEActive)
            return;

       /* _timer -= Time.deltaTime;
        ChangeTimerImage();*/

        if (_timer <= 0)
        {
            QTEFailed();
            return;
        }

        if (Input.GetKeyDown(_keySequence[0].keyCode))
        {
            // Correct key pressed, remove from sequence
            _keySequence.RemoveAt(0);
            keyPrefabs[_sequenceIndex].SetActive(false);
            _sequenceIndex++;

            if (_keySequence.Count == 0)
            {
                ProgressQTE();
                return;
            }
        }
        else if (Input.anyKeyDown)
        {
            QTEFailed();
        }
    }

    private void ChangeTimerImage()
    {
        foreach (Image timerPart in timerLines)
            timerPart.fillAmount -= (Time.fixedDeltaTime / timeLimit);
    }

    private void ResetTimerImage()
    {
        foreach (Image timerPart in timerLines)
            timerPart.fillAmount = 1;
    }

    private List<Arrow> GenerateRandomKeySequence()
    {
        List<Arrow> sequence = new List<Arrow>();

        int numKeys = _currentSequenceKeysCount[_timesMinigameWasFinished];

        for (int i = 0; i < numKeys; i++)
        {
            int randomIndex = Random.Range(0, _arrows.Count);
            sequence.Add(_arrows[randomIndex]);
            keyPrefabs[i].GetComponent<Image>().sprite = _arrows[randomIndex].sprite;
        }

        return sequence;
    }

    private void ProgressQTE()
    {
        _timesMinigameWasFinished++;
        CheckWinCondition();

        if (!_isMinigameFinished)
            StartQTE();
    }

    private void CheckWinCondition()
    {
        if (Mathf.Approximately(_timesMinigameWasFinished, _minigamesFinishedLimit))
        {
            _isMinigameFinished = true;
            _isQTEActive = false;

            //GameManager.Instance.StopGame();
            print("Minigame complete");
            GameManager.Instance.StartMidgame();
        }
    }

    private void ResetKeysObjects()
    {
        for (int i = 0; i < _currentSequenceKeysCount[_timesMinigameWasFinished]; i++)
            keyPrefabs[i].SetActive(true);
    }

    private void QTEFailed()
    {
        GameManager.Instance.IncreaseDifficulty();
        print("FAILED");
        ProgressQTE();
    }
}
