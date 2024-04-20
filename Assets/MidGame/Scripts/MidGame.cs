using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidGame : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Transform _line;

    [SerializeField]
    private float _minLineValue;
    [SerializeField]
    private float _maxLineValue;

    private float _lineValue; 

    /*[SerializeField]
    private float _sliderSpeed;*/

    private bool _isEnabled = false;

    private bool _isPouring = false;

    private void Start()
    {
        Init();
    }

    private void Init() 
    {
        _lineValue = Random.Range(_minLineValue, _maxLineValue);
        _slider.handleRect = _line.GetComponent<RectTransform>();
        _slider.value = _lineValue;
        _slider.handleRect = null;
        _slider.value = 0;

       _isEnabled = true;

    }

    private void Update()
    {
        if (!_isEnabled)
            return;

        if(Input.GetKeyDown(KeyCode.Space) && !_isPouring)
        {
            _isPouring = true;
            StartCoroutine(PourCoroutine());
        }
        if(Input.GetKeyUp(KeyCode.Space) && _isPouring)
        {
            _isPouring = false;
            _isEnabled = false;
        }
    }

    private IEnumerator PourCoroutine()
    {
        while (_isPouring)
        {
            _slider.value += 0.005f;
            yield return new WaitForSeconds(0.005f);
        }
        CheckValue();
    }

    private void CheckValue()
    {
        if (_slider.value <= _lineValue + 0.005f && _slider.value >= _lineValue - 0.005f)
        {
            Debug.Log("Perfect");
        }
        else if (_slider.value <= _lineValue + 0.05f && _slider.value >= _lineValue - 0.05f)
        {
            Debug.Log("Good Enough");
        }
        else if (_slider.value <= _lineValue + 0.1f && _slider.value >= _lineValue - 0.1f)
        {
            Debug.Log("Almost False");
        }
        else if (_slider.value <= _lineValue + 0.2f && _slider.value >= _lineValue - 0.2f)
        {
            Debug.Log("Not Good");
        }
        else
            Debug.Log("Bad");
    }
}
