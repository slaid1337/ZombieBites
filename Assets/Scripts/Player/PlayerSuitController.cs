using UnityEngine;

public class PlayerSuitController : MonoBehaviour
{
    [SerializeField] private float _decreaseSpeed;
    [SerializeField] private float _startSuitValue;
    [SerializeField] private CircleIndicator _circleIndicator;
    private float _suitValue;

    private void Awake()
    {
        _suitValue = _startSuitValue;
    }

    private void FixedUpdate()
    {
        if (_startSuitValue <= 0) return;
        _suitValue -= _decreaseSpeed * Time.fixedDeltaTime;
        _circleIndicator.UpdateStatus(Serializer.Normalize(_suitValue, 0, _startSuitValue));
    }

    private void OnTriggerEnter(Collider other)
    {
        SuitCharger suitCharger = null;

        if (other.TryGetComponent<SuitCharger>(out suitCharger))
        {
            _suitValue += suitCharger.GetCharge();

            _suitValue = Mathf.Clamp(_suitValue, 0, _startSuitValue);
        }
    }
}
