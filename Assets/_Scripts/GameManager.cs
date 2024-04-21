using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private enum MinigamesType 
    {
        Pointer, Arrows, Keys
    }

    private enum DifficultyValue
    {
        Easy = 1,
        Hard = 5,
        Suicide = 10
    }

    public static GameManager Instance;

    [SerializeField]
    private List<MinigamesType> _minigamesTypes;

    private int _currentMinigame = 0;

    [SerializeField]
    private ArrowsMinigame _arrowsMinigame;
    [SerializeField]
    private ArrowMinigame _pointerMinigame;
    [SerializeField]
    private KeysMinigame _keysMinigame;

    [SerializeField]
    private MidGame _midGame;

    [SerializeField] private TextMeshProUGUI weightIndicator;
    [SerializeField] private TextMeshProUGUI moneyIndicator;
    [SerializeField] private TextMeshProUGUI timeIndicator;

    private int _currentDifficultyValue = 1;
    private int _currentTimeInSeconds = 60;
    private const int _timerChangeDelay = 1;
    private bool _isRoundStillGoing = true;
    
    public bool IsGameLosed { get; set; }

    public int CurrentDifficultyValue { 
        get => _currentDifficultyValue;
        private set => _currentDifficultyValue = Mathf.Clamp(value, (int)DifficultyValue.Easy, (int)DifficultyValue.Suicide);
    }

    public int TimerInSeconds
    {
        get => _currentTimeInSeconds;
        private set
        {
            _currentTimeInSeconds = Mathf.Clamp(value, 0, 60);
            timeIndicator.text = _currentTimeInSeconds.ToString();
        }
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
        StartLevel();
    }


    private void ShuffleMinigames()
    {
        Shuffle(_minigamesTypes);
        _currentMinigame = 0;
    }

    private void StartLevel()
    {
        ShuffleMinigames();
        NextMinigame();
    }

    public void NextMinigame()
    {
        switch (_minigamesTypes[_currentMinigame])
        {
            case MinigamesType.Pointer:
                _pointerMinigame.gameObject.SetActive(true);
                _pointerMinigame.Init();
                break;
            case MinigamesType.Arrows:
                _arrowsMinigame.gameObject.SetActive(true);
                _arrowsMinigame.Init();
                break;
            case MinigamesType.Keys:
                _keysMinigame.gameObject.SetActive(true);
                _keysMinigame.Init();
                break;
        }

    }

    public void StartMidgame()
    {
        _currentMinigame++;
        _pointerMinigame.gameObject.SetActive(false);
        _arrowsMinigame.gameObject.SetActive(false);
        _keysMinigame.gameObject.SetActive(false);

        _midGame.Init();
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

    public void IncreaseTime(int time) => TimerInSeconds += time;
    public void DecreaseTime(int time) => TimerInSeconds -= time;

    public void StopGame() => _isRoundStillGoing = false;

    static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void AddToTime(int x) => TimerInSeconds += x;
}
