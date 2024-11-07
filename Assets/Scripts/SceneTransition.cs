using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class SceneTransition : Singletone<SceneTransition>
{
    private CanvasGroup _canvas;

    public override void Awake()
    {
        base.Awake();

        _canvas = GetComponent<CanvasGroup>();
    }

    public void OpenCanvas()
    {
        _canvas.DOFade(1f, 1f);
        _canvas.blocksRaycasts = true;
    }

    public void CloseCanvas()
    {
        _canvas.DOFade(0f, 1f).OnComplete(() => _canvas.blocksRaycasts = false);
    }
}
