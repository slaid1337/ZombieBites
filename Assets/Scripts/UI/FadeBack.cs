using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FadeBack : Singletone<FadeBack>
{
    private CanvasGroup _canvas;

    public override void Awake()
    {
        base.Awake();

        _canvas = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        _canvas.DOFade(0.8f, 1f);
        _canvas.blocksRaycasts = true;
    }

    public void Close()
    {
        _canvas.DOFade(0f, 1f).OnComplete(() => _canvas.blocksRaycasts = false);
    }
}
