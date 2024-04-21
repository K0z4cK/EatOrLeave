using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowMinigame : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject safeZoneObj;
    [SerializeField] private Transform safeZoneParent;
    [SerializeField] RawImage targetImage;
    [SerializeField] private Image handle;
    [SerializeField, Range(0.5f, 50f)] private float _moveStep = 20f;

    private int _moveDirection;

    private GameObject _spawnedSafeZone;
    private GameManager _instance;

    private int _timesGameWasCompleted = 0;
    private const int _gamesCompletedLimit = 3;
    private const float _difficultyCoef = 0.05f;
    private bool _isMinigameFinished = false;
    private readonly float[] _safeZoneWidths = { 1f, 0.7f, 0.4f };

    private enum MoveDirection
    {
        Left = -1,
        Right = 1
    }

    public void Init()
    {
        _difficultyCoef = 0.05f;
        _timesGameWasCompleted = 0;
        _isMinigameFinished = false;
        _moveDirection = (int)MoveDirection.Right;
        _instance = GameManager.Instance;
        UpdateDifficulty();
        //SpawnSafeZone(_safeZoneWidths[_timesGameWasCompleted]);
        StartCoroutine(MoveArrow());
    }

    private IEnumerator MoveArrow()
    {
        while (!_isMinigameFinished)
        {
            yield return null;

            if (Mathf.Approximately(slider.value, 1))
                _moveDirection = (int)MoveDirection.Left;
            if (Mathf.Approximately(slider.value, 0))
                _moveDirection = (int)MoveDirection.Right;

            slider.value += _moveStep * _moveDirection * Time.deltaTime * _difficultyCoef * _instance.CurrentDifficultyValue;
        } 
    }

    private void SpawnSafeZone(float safeZoneWidth)
    {
        if (_spawnedSafeZone != null)
            Destroy(_spawnedSafeZone);

        RectTransform parentRect = targetImage.transform.parent.GetComponent<RectTransform>();

        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        _spawnedSafeZone = Instantiate(safeZoneObj);
        _spawnedSafeZone.transform.SetParent(targetImage.transform);

        _spawnedSafeZone.transform.localScale = new Vector3(safeZoneWidth, parentHeight / _spawnedSafeZone.GetComponent<RectTransform>().rect.height, 1f);

        float randomX = Random.Range(0f, parentWidth);

        _spawnedSafeZone.transform.localPosition = new Vector3(randomX - parentWidth / 2f, 0, 0f);
    }

    private void Update()
    {
        if (_isMinigameFinished)
            return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            GameManager.Instance.NextFoodStage();
            if (IsArrowInSafeZone())
            {
                ProgressMinigame();
            }
            else
            {
                ProgressMinigame();
                _instance.IncreaseDifficulty();
            }
        }
    }

    private void ProgressMinigame()
    {
        _timesGameWasCompleted++;
        CheckWinCondition();

        if (!_isMinigameFinished)
            UpdateDifficulty();
    }

    private bool IsArrowInSafeZone()
    {
        if (handle.transform.position.x < _spawnedSafeZone.transform.position.x + _spawnedSafeZone.transform.localScale.x
            && handle.transform.position.x > _spawnedSafeZone.transform.position.x - _spawnedSafeZone.transform.localScale.x)
        {
            return true;
        }
        return false;
    }

    private void UpdateDifficulty()
    {
        ResetArrow();
        SpawnSafeZone(_safeZoneWidths[_timesGameWasCompleted]);
    }

    private void ResetArrow()
    {
        slider.value = 0;
        _moveDirection = (int)MoveDirection.Right;
    }

    private void CheckWinCondition()
    {
        if (Mathf.Approximately(_timesGameWasCompleted, _gamesCompletedLimit))
        {
            print("Current minigame complete");
            _isMinigameFinished = true;
            GameManager.Instance.StartMidgame();
        }
    }
}
