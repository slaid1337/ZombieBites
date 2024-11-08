using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransitionController : Singletone<MapTransitionController>
{
    public void OpenLevel(int lvl)
    {
        SceneTransition.Instance.OpenCanvas();

        if (lvl == 0)
        {
            StartCoroutine(OpenCor("MainScene"));
        }
        else if (lvl == 1)
        {
            StartCoroutine(OpenCor("TrainingLocation"));
        }
        else if ( lvl == 2) { }
        {
            StartCoroutine(OpenCor("LevelScene"));
        }
    }


    public void OpenMap()
    {
        SceneTransition.Instance.OpenCanvas();

        StartCoroutine(OpenCor("MapScene"));
    }

    private IEnumerator OpenCor(string sceneName)
    {
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(sceneName);
    }
}
