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

    private int _moveDirection;
    private const float _moveDelay = 0.01f;
    private const float _moveStep = 0.05f;

    private GameObject _spawnedSafeZone;

    private int _timesGameWasCompleted = 0;
    private const int _gamesCompletedLimit = 3;
    private bool _isMinigameFinished = false;
    private readonly float[] _safeZoneWidths = { 1f, 0.7f, 0.3f };

    private enum MoveDirection
    {
        Left = -1,
        Right = 1
    }

    private void Start()
    {
        _moveDirection = (int)MoveDirection.Right;

        SpawnSafeZone(_safeZoneWidths[_timesGameWasCompleted]);
        StartCoroutine(MoveArrow());
    }

    private IEnumerator MoveArrow()
    {
        while (!_isMinigameFinished)
        {
            yield return new WaitForSeconds(_moveDelay);

            if (Mathf.Approximately(slider.value, 1))
                _moveDirection = (int)MoveDirection.Left;
            if (Mathf.Approximately(slider.value, 0))
                _moveDirection = (int)MoveDirection.Right;

            slider.value += _moveStep * _moveDirection;
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
            if (IsArrowInSafeZone())
            {
                _timesGameWasCompleted++;
                CheckWinCondition();

                if (!_isMinigameFinished)
                    UpdateDifficulty();
            }
            else
                print("Minigame lose!"); 
        }
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
        }
    }
}
