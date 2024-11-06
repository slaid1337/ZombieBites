using System;
using TMPro;
using UnityEngine;

public class LevelController : Singletone<LevelController>
{
    [SerializeField] private int _zombieCount;
    [SerializeField] private float _lvlTime;

    private int _humanCount;
    private float _lvlTimer;

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timerText;

    private bool _isPlaying;

    private void Start()
    {
        _scoreText.text = "0 / " + _zombieCount;
        _timerText.text = TimeSpan.FromSeconds(_lvlTime).ToString("mm\\:ss");

        _lvlTimer = _lvlTime;

        _isPlaying = true;
    }

    private void FixedUpdate()
    {
        if (!_isPlaying)
        {
            return;
        }

        if (_lvlTimer <= 0)
        {
            _isPlaying = false;

            print("time gone");

            return;
        }

        _lvlTimer -= Time.fixedDeltaTime;

        _timerText.text = TimeSpan.FromSeconds(_lvlTimer).ToString("mm\\:ss");
    }

    public void AddHuman()
    {
        _humanCount++;

        _scoreText.text = _humanCount + " / " + _zombieCount;

        if (_humanCount == _zombieCount)
        {
            _scoreText.text = "WIN";
        }
    }
}
