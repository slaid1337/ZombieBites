using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    private CanvasGroup _canvas;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        FadeBack.Instance.Open();
        _canvas.DOFade(1f, 1f);
    }

    public void Restart()
    {
        SceneTransition.Instance.OpenCanvas();

        StartCoroutine(RestartCor());
    }

    private IEnumerator RestartCor()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Map");
    }
}
