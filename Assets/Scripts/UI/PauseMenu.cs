using UnityEngine;
using DG.Tweening;

public class PauseMenu : Singletone<PauseMenu>
{
    private CanvasGroup _canvas;


    public override void Awake()
    {
        base.Awake();

        _canvas = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        FadeBack.Instance.Open();
        _canvas.blocksRaycasts = true;
        _canvas.DOFade(1f, 1f);

        LevelController.Instance.StopPlay();
    }

    public void Close()
    {
        FadeBack.Instance.Close();
        _canvas.blocksRaycasts = false;
        _canvas.DOFade(0f, 1f);

        LevelController.Instance.ResumePlay();
    }
}
