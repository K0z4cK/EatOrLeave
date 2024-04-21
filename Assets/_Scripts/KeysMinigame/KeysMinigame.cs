using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeysMinigame : MonoBehaviour
{
    [SerializeField] private GameObject[] keyPrefabs; 
    [SerializeField] private TextMeshProUGUI[] keyTexts; 
    [SerializeField] private float timeLimit;
    [SerializeField] private Image[] timerLines;

    private List<string> _keySequence; 
    private float _timer;
    private bool _isQTEActive;
    private int _sequenceIndex = 0;
    private readonly int[] _currentSequenceKeysCount = { 4, 5, 6 };

    private bool _isMinigameFinished = false;
    private int _timesMinigameWasFinished = 0;
    private int _minigamesFinishedLimit = 3;

    private void Start()
    {
        // Ensure key prefabs and key texts have same length
        if (keyPrefabs.Length != keyTexts.Length)
        {
            Debug.LogError("Number of key prefabs and key texts must be equal!");
            return;
        }
        else
            StartQTE();
    }

    private void StartQTE()
    {
        // Reset timer and flag
        _timer = timeLimit;
        _isQTEActive = true;
        _sequenceIndex = 0;
        ResetTimerImage();
        ResetKeysObjects();

        _keySequence = GenerateRandomKeySequence();

        for (int i = 0; i < _keySequence.Count; i++)
            keyTexts[i].text = _keySequence[i];
    }

    private void Update()
    {
        if (!_isQTEActive)
            return;

        _timer -= Time.deltaTime;
        ChangeTimerImage();

        if (_timer <= 0)
        {
            QTEFailed();
            return;
        }

        if (Input.GetKeyDown(GetKeyByString(_keySequence[0])))
        {
            // Correct key pressed, remove from sequence
            _keySequence.RemoveAt(0);
            keyPrefabs[_sequenceIndex].SetActive(false);
            _sequenceIndex++;

            if (_keySequence.Count == 0)
            {
                QTESucceeded();
                return;
            }
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

    private List<string> GenerateRandomKeySequence()
    {
        List<string> keys = new List<string>() { "↑", "↓", "←", "→" };
        List<string> sequence = new List<string>();

        int numKeys = _currentSequenceKeysCount[_timesMinigameWasFinished];

        for (int i = 0; i < numKeys; i++)
        {
            int randomIndex = Random.Range(0, keys.Count);
            sequence.Add(keys[randomIndex]);
        }

        return sequence;
    }

    private KeyCode GetKeyByString(string keyString)
    {
        return keyString switch
        {
            "↑" => KeyCode.UpArrow,
            "↓" => KeyCode.DownArrow,
            "←" => KeyCode.LeftArrow,
            "→" => KeyCode.RightArrow,
            _ => KeyCode.None,
        };
    }

    private void QTESucceeded()
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

            print("Minigame complete");
        }
    }

    private void ResetKeysObjects()
    {
        for (int i = 0; i < _currentSequenceKeysCount[_timesMinigameWasFinished]; i++)
            keyPrefabs[i].SetActive(true);
    }

    private void QTEFailed()
    {
        Debug.Log("QTE Failed!");
    }
}
