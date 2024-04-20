using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowMinigame : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject safeZoneObj;
    [SerializeField] private Transform safeZoneParent;

    private int _moveDirection;
    private const float _moveDelay = 0.05f;
    private const float _moveStep = 0.05f;

    private float _safeZoneWidth = 30f;

    private enum MoveDirection
    {
        Left = -1,
        Right = 1
    }

    private void Start()
    {
        _moveDirection = (int)MoveDirection.Right;

        SpawnSafeZone();
        StartCoroutine(MoveArrow());
    }

    private IEnumerator MoveArrow()
    {
        while (true)
        {
            yield return new WaitForSeconds(_moveDelay);

            if (Mathf.Approximately(slider.value, 1))
                _moveDirection = (int)MoveDirection.Left;
            if (Mathf.Approximately(slider.value, 0))
                _moveDirection = (int)MoveDirection.Right;

            slider.value += _moveStep * _moveDirection;
        } 
    }

    private void SpawnSafeZone()
    {
        RectTransform handleRect = slider.handleRect;

        float minX = handleRect.position.x - handleRect.rect.width * 0.5f;
        float maxX = handleRect.position.x + handleRect.rect.width * 0.5f;
        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPosition = new(randomX, handleRect.position.y, handleRect.position.z);

        if (IsWithinSliderBounds(spawnPosition))
        {
            GameObject safeZoneGO = Instantiate(safeZoneObj, spawnPosition, Quaternion.identity, safeZoneParent);

            float parentHeight = safeZoneParent.GetComponent<RectTransform>().sizeDelta.y;
            float width = _safeZoneWidth;
            safeZoneGO.GetComponent<RectTransform>().sizeDelta = new Vector2(width, parentHeight);
        }
        else
        {
            Debug.LogError("Safe zone obj is out of bounds");
        }

        bool IsWithinSliderBounds(Vector3 position)
        {
            RectTransform sliderRect = slider.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            sliderRect.GetWorldCorners(corners);

            // Check if position is within the rectangle formed by the corners
            return position.x >= corners[0].x && position.x <= corners[2].x &&
                    position.y >= corners[0].y && position.y <= corners[1].y;
        }
    }
}
