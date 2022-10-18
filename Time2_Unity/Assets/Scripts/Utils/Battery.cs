using System;
using System.Collections;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] private int maxTime = 60;
    private int _currentTime;
    private bool _isEnabled = false;
    
    public delegate void EndTime();
    public event EndTime OnEndTime;
    public delegate void ChangeTime(float currentTime);
    public event ChangeTime OnChangeTime;

    private void Awake()
    {
        _currentTime = maxTime;
    }

    private IEnumerator BatteryCounter()
    {
        while(_currentTime != 0)
        {

            --_currentTime;
            OnChangeTime?.Invoke((float)_currentTime/maxTime);
            yield return new WaitForSeconds(1);
        }

        _isEnabled = false;
        OnEndTime?.Invoke();
    }

    public void SwitchPower()
    {
        if (_isEnabled)
        {
            StopAllCoroutines();
        }
        else
        {
            StartCoroutine(BatteryCounter());
        }
        _isEnabled = !_isEnabled;
    }

    public void Recharge(int amount)
    {
        var temp = _currentTime + amount;
        _currentTime = Math.Max(temp, maxTime);
        if(!_isEnabled) SwitchPower();
    }
}
