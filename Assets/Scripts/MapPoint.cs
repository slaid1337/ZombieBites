using System.Collections;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private int _lvlIndex;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;

        Vector3 directionToCamera = _camera.transform.forward;
        
        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
        _canvas.transform.rotation = lookRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OpenDescription();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CloseMap();
        }
    }

    public void OpenDescription()
    {
        StopAllCoroutines();

        StartCoroutine(ToggleCoroutine(true));
    }

    public void CloseMap()
    {
        StopAllCoroutines();

        StartCoroutine(ToggleCoroutine(false));
    }

    public void OpenLevel()
    {
        MapTransitionController.Instance.OpenLevel(_lvlIndex);
    }

    private IEnumerator ToggleCoroutine(bool isOpen)
    {
        if (isOpen)
        {
            _canvas.blocksRaycasts = true;

            while (_canvas.alpha != 1)
            {
                _canvas.alpha += Time.fixedDeltaTime * 2;

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            _canvas.blocksRaycasts = false;

            while (_canvas.alpha != 0)
            {
                _canvas.alpha -= Time.fixedDeltaTime * 2;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
