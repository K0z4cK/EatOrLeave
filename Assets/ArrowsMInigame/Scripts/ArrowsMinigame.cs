using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ArrowsMinigame : MonoBehaviour
{
    [SerializeField]
    private List<Arrow> _arrows;

    [SerializeField]
    private List<float> _stagesTimers;

    [SerializeField]
    private float _generateTime;

    [SerializeField]
    private Image _leftArrowImage;
    [SerializeField]
    private Image _rightArrowImage;

    [SerializeField] 
    private float timeLimit;
    [SerializeField] 
    private Image[] timerLines;

    private Arrow _leftArrow;
    private Arrow _rightArrow;

    private bool _isTimerStart;

    private int _currentStage = 0;

    private Coroutine _timerCoroutine;

    public void Init()
    {
        _currentStage = 0;
        ResetTimerImage();
        StartStage();
    }

    private void StartStage()
    {
        int n = _arrows.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Arrow value = _arrows[k];
            _arrows[k] = _arrows[n];
            _arrows[n] = value;
        }

        _leftArrow = _arrows[0];
        _rightArrow = _arrows[1];

        _timerCoroutine = StartCoroutine(StageCoroutine());
    }

    private void ChangeTimerImage()
    {
        foreach (Image timerPart in timerLines)
            timerPart.fillAmount -= (Time.fixedDeltaTime / _stagesTimers[_currentStage]);
    }

    private void ResetTimerImage()
    {
        foreach (Image timerPart in timerLines)
            timerPart.fillAmount = 1;
    }

    private IEnumerator StageCoroutine()
    {
        float time = 0;
        while (time < _generateTime)
        {
            _leftArrowImage.sprite = _arrows[Random.Range(0, _arrows.Count)].sprite;
            _rightArrowImage.sprite = _arrows[Random.Range(0, _arrows.Count)].sprite;
            yield return new WaitForSeconds(0.01f);
            time += 0.01f;
        }

        _leftArrowImage.sprite = _leftArrow.sprite;
        _rightArrowImage.sprite = _rightArrow.sprite;


        time = _stagesTimers[_currentStage];
        _isTimerStart = true;

        while(time > 0)
        {
            yield return new WaitForSeconds(1f);
            time -= 1f;
            
        }

        //yield return new WaitForSeconds(_stagesTimers[_currentStage]);
        _isTimerStart = false;
        Debug.Log("Penalty");
        _currentStage++;
        GameManager.Instance.NextFoodStage();
        GameManager.Instance.IncreaseWeight(5);
        GameManager.Instance.DecreaseTime(3);

        if (_currentStage == _stagesTimers.Count)
        {
            Debug.Log("End");
            _isTimerStart = false;
            StopCoroutine(_timerCoroutine);
            GameManager.Instance.StartMidgame();
        }
        else
        {
            ResetTimerImage();
            StartStage();
        }
    }

    private void FixedUpdate()
    {
        if (!_isTimerStart)
            return;

        ChangeTimerImage();
    }

    private void Update()
    {
        if (!_isTimerStart)
            return;

        //ChangeTimerImage();
        if (Input.GetKey(_leftArrow.keyCode) && Input.GetKey(_rightArrow.keyCode))
        {
            _currentStage++;
            GameManager.Instance.NextFoodStage();
            GameManager.Instance.IncreaseWeight(2);
            _isTimerStart = false;

            if (_currentStage == _stagesTimers.Count)
            {
                Debug.Log("Win");                
                StopCoroutine(_timerCoroutine);
                GameManager.Instance.StartMidgame();
                return;
            }

            StopCoroutine(_timerCoroutine);
            ResetTimerImage();
            StartStage();
            

            return;
            //win;
        }
    }
}

[Serializable]
public struct Arrow
{
    public KeyCode keyCode;
    public Sprite sprite;
}


