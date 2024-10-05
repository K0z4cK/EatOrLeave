using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    private List<Transform> _food;

    private int _currentFoodIndex = 0;

    private int _currentFoodStage = 0;

    [SerializeField]
    private ArrowsMinigame _arrowsMinigame;
    [SerializeField]
    private ArrowMinigame _pointerMinigame;
    [SerializeField]
    private KeysMinigame _keysMinigame;

    [SerializeField]
    private Transform _winPanel;
    [SerializeField]
    private Transform _losePanel;

    [SerializeField]
    private MidGame _midGame;

    [SerializeField] private TextMeshProUGUI weightIndicator;
    [SerializeField] private TextMeshProUGUI moneyIndicator;
    [SerializeField] private TextMeshProUGUI timeIndicator;

    private int _currentDifficultyValue = 1;
    private int _currentTimeInSeconds = 60;
    private const int _timerChangeDelay = 1;
    private bool _isRoundStillGoing = true;

    private int _currentMoney = 50;
    private int _currentWeight = 70;


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
            string textSeconds = TimerInSeconds.ToString();
            if(TimerInSeconds < 10)
                textSeconds = "0"+ TimerInSeconds.ToString();

            timeIndicator.text = "0:" + textSeconds;
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
        weightIndicator.text = _currentWeight + "KG";
        moneyIndicator.text = _currentMoney + "$";

        StartCoroutine(StartTimer());
        Shuffle(_food);
        StartLevel();
    }


    private void ShuffleMinigames()
    {
        Shuffle(_minigamesTypes);
        _currentMinigame = 0;
    }

    private void SetFood()
    {
        _food.ForEach(x => x.gameObject.SetActive(false));
        _food[_currentFoodIndex].gameObject.SetActive(true);
    }

    public void NextFoodStage()
    {
        _currentFoodStage++;
        foreach (Transform item in _food[_currentFoodIndex])
        {
            item.gameObject.SetActive(false);
        }
        if (_currentFoodStage < 9)
            _food[_currentFoodIndex].GetChild(_currentFoodStage).gameObject.SetActive(true);
    }

    private void StartLevel()
    {
        if (_currentFoodIndex >= _food.Count)
        {
            StopGame();
            _winPanel.gameObject.SetActive(true);
            return;
        }

        _currentFoodStage = 0;
        SetFood();
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

        if (_currentMinigame >= _minigamesTypes.Count)
        {
            _currentFoodIndex++;
            StartLevel();           
            return;
        }

        _midGame.Init();
    }

    private IEnumerator StartTimer()
    {
        while (_isRoundStillGoing)
        {
            yield return new WaitForSeconds(_timerChangeDelay);
            TimerInSeconds--;
            CheckLoseCondition();
        }
    }

    private void CheckLoseCondition()
    {
        if (Mathf.Approximately(TimerInSeconds, 0))
        {
            StopGame();
            _losePanel.gameObject.SetActive(true);
            Time.timeScale = 0;
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

    public void IncreaseWeight(int x) {
        _currentWeight += x;
        weightIndicator.text = _currentWeight + "KG";
    }

    public void DecreaseWeight(int x)
    {
        _currentWeight -= x;
        weightIndicator.text = _currentWeight + "KG";
    }

    public void IncreaseMoney(int x)
    {
        _currentMoney += x;
        moneyIndicator.text = _currentMoney + "$";
    }

    public void DecreaseMoney(int x)
    {
        _currentMoney -= x;
        moneyIndicator.text = _currentMoney + "$";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("_MainMenu");
    }
}
