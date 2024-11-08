using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using CrazyGames;

public class EndMenu : Singletone<EndMenu>
{
    [SerializeField] private TMP_Text _scoreText;

    [SerializeField] private GameObject _winState;
    [SerializeField] private GameObject _looseState;

    private CanvasGroup _canvas;


    public override void Awake()
    {
        base.Awake();

        _canvas = GetComponent<CanvasGroup>();
    }

    public void Open(bool isWin, int score, int maxScore)
    {
        CrazySDK.Game.GameplayStop();

        FadeBack.Instance.Open();
        _canvas.DOFade(1f, 1f);
        _canvas.blocksRaycasts = true;

        if (isWin)
        {
            _winState.SetActive(true);
            _looseState.SetActive(false);

            _scoreText.text = "Cured <color=\"yellow\">" + score + "/" + maxScore;
        }
        else
        {
            _winState.SetActive(false);
            _looseState.SetActive(true);

            _scoreText.text = "Cured <color=\"red\">" + score + "/" + maxScore;
        }
        
    }

    public void Restart()
    {
        SceneTransition.Instance.OpenCanvas();

        StartCoroutine(RestartCor());
    }

    private IEnumerator RestartCor()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadHome()
    {
        SceneTransition.Instance.OpenCanvas();

        StartCoroutine(LoadHomeCor());
    }

    private IEnumerator LoadHomeCor()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("MainScene");
    }
}
