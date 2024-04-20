using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowMinigame : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private int _moveDirection;
    private const float _moveDelay = 0.05f;
    private const float _moveStep = 0.05f;

    private enum MoveDirection
    {
        Left = -1,
        Right = 1
    }

    private void Start()
    {
        _moveDirection = (int)MoveDirection.Right;

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
}
