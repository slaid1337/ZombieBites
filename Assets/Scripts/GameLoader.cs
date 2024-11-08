using UnityEngine;
using UnityEngine.Events;

public class GameLoader : Singletone<GameLoader>
{
    public UnityEvent OnLoad;

    private void Start()
    {
        OnLoaded();
    }

    public void OnLoaded()
    {
        SceneTransition.Instance.CloseCanvas();

        OnLoad?.Invoke();
    }
}
