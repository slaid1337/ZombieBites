using CrazyGames;
using UnityEngine;
using UnityEngine.Events;

public class CrazyGamesController : Singletone<CrazyGamesController>
{
    public UnityEvent OnLoad;
    public bool IsLoaded
    {
        get;
        private set;
    }

    private void Start()
    {
        if (CrazySDK.IsAvailable)
        {
            CrazySDK.Init(() =>
            {
                OnLoad?.Invoke();
                IsLoaded = true;
            });
        }
        else
        {
            OnLoad?.Invoke();
            IsLoaded = true;
        }
    }
}
