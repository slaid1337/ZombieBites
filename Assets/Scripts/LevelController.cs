using CrazyGames;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : Singletone<LevelController>
{
    [SerializeField] private int _zombieCount;
    [SerializeField] private float _lvlTime;

    private int _humanCount;
    private float _lvlTimer;

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timerText;

    private bool _isPlaying;

    public UnityEvent OnStop;
    public UnityEvent OnResume;

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

            GetResult();

            return;
        }

        _lvlTimer -= Time.fixedDeltaTime;

        _timerText.text = TimeSpan.FromSeconds(_lvlTimer).ToString("mm\\:ss");
    }

    public bool IsPlay()
    {
        return _isPlaying;
    }

    public void StopPlay()
    {
        _isPlaying = false;
        OnStop?.Invoke();
    }

    public void ResumePlay()
    {
        _isPlaying = true;
        OnResume?.Invoke();
    }

    public void GetResult()
    {
        StopPlay();

        bool isWin = true;

        if (_zombieCount / 2 > _humanCount) isWin = false;

        EndMenu.Instance.Open(isWin, _humanCount, _zombieCount);
    }

    public void GetResult(bool isDead)
    {
        StopPlay();

        bool isWin = true;

        if (isDead) isWin = false;

        if (_zombieCount / 2 > _humanCount) isWin = false;

        if (isWin)
        {
            CrazySDK.Game.HappyTime();
        }

        EndMenu.Instance.Open(isWin, _humanCount, _zombieCount);
    }

    public void AddHuman()
    {
        _humanCount++;

        _scoreText.text = _humanCount + " / " + _zombieCount;

        if (_humanCount == _zombieCount)
        {
            GetResult();
        }
    }
}
