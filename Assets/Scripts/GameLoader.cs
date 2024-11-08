using UnityEngine;
using UnityEngine.Events;

public class GameLoader : Singletone<GameLoader>
{
    public UnityEvent OnLoad;

    private void Start()
    {
        if (CrazyGamesController.Instance.IsLoaded)
        {
            OnLoaded();
        }
        else
        {
            CrazyGamesController.Instance.OnLoad.AddListener(OnLoaded);
        }
    }

    public void OnLoaded()
    {
        SceneTransition.Instance.CloseCanvas();

        OnLoad?.Invoke();
    }
}
