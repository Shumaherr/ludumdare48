using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _timerPeriod;
    private bool _isActive;

    private float _timeRemaining;
    private TextMeshProUGUI _timerText;

    // Start is called before the first frame update
    void Start()
    {
        _timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public float TimerPeriod
    {
        get => _timerPeriod;
        set => _timerPeriod = value;
    }

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
        }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            _timerText.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(_timeRemaining / 60), Mathf.FloorToInt(_timeRemaining % 60));
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
            }

            if (_timeRemaining <= 0)
            {
                _timeRemaining = _timerPeriod;
            }
            
        }
        
    }
}
