using UnityEngine;

public class PlayerSuitController : MonoBehaviour
{
    [SerializeField] private float _decreaseSpeed;
    [SerializeField] private float _startSuitValue;
    [SerializeField] private CircleIndicator _circleIndicator;
    private float _suitValue;

    public void Init(float startValue)
    {
        _startSuitValue = startValue;

        _suitValue = _startSuitValue;
    }

    private void FixedUpdate()
    {
        if (_startSuitValue <= 0)
        {
            LevelController.Instance.GetResult(true);

            enabled = false;

            return;
        }
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

    public void DamageSuit(float damage)
    {
        _suitValue -= damage;
        _suitValue = Mathf.Clamp(_suitValue, 0, _startSuitValue);
    }

    public bool InDanger()
    {
        if (_suitValue <= _startSuitValue / 2) return true;

        return false;
    }

    public bool IsGone()
    {
        if (_suitValue == 0) return true;

        return false;
    }
}
