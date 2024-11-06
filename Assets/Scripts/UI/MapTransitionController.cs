using UnityEngine;

public class MapTransitionController : Singletone<MapTransitionController>
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _newTarget;
    [SerializeField] private CameraFollow _cameraFollow;
    [SerializeField] private GameObject _backButton;

    public void OpenMap()
    {
        _cameraFollow.player = _newTarget;

        _player.gameObject.SetActive(false);
        _newTarget.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(true);
    }

    public void CloseMap()
    {
        _cameraFollow.player = _player;

        _player.gameObject.SetActive(true);
        _newTarget.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);
    }
}
