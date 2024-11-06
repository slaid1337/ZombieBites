using System.Collections;
using UnityEngine;

public class MainDesk : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mapCanvas;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OpenMap();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CloseMap();
        }
    }

    public void OpenMap()
    {
        StopAllCoroutines();

        StartCoroutine(ToggleCoroutine(true));
    }

    public void CloseMap()
    {
        StopAllCoroutines();

        StartCoroutine(ToggleCoroutine(false));
    }

    private IEnumerator ToggleCoroutine(bool isOpen)
    {
        if (isOpen)
        {
            _mapCanvas.blocksRaycasts = true;

            while(_mapCanvas.alpha != 1)
            {
                _mapCanvas.alpha += Time.fixedDeltaTime * 2;

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            _mapCanvas.blocksRaycasts = false;

            while (_mapCanvas.alpha != 0)
            {
                _mapCanvas.alpha -= Time.fixedDeltaTime * 2;

                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void OpenMapScene()
    {
        MapTransitionController.Instance.OpenMap();
    }
}
