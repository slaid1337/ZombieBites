using UnityEngine;

public class PlayerController : Singletone<PlayerController>
{
    private PlayerSuitController _suitController;
    private PlayerMovement _moveController;
    [SerializeField] private GameBalance _gameBalance;
    [SerializeField] private Canvas _statusCanvas;
    [SerializeField] private CircleIndicator _circleIndicator;
    [SerializeField] private float _followRadius;

    private bool _isHealing;
    private float _healValue;

    private ZombieBehaviour _healingZombie;

    public override void Awake()
    {
        base.Awake();

        _suitController = GetComponent<PlayerSuitController>();
        _moveController = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        if (_isHealing)
        {
            _healValue += Time.fixedDeltaTime * _gameBalance.HealSpeed;

            if (_healValue >= 2f)
            {
                StopHeal(true);
            }

            _circleIndicator.UpdateStatus(Serializer.Normalize(_healValue, 0, 2));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZombieBehaviour zombie = null;

        if (other.TryGetComponent<ZombieBehaviour>(out zombie))
        {
            if (!zombie.isActiveAndEnabled) return;
            _healingZombie = zombie;

            Heal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ZombieBehaviour zombie = null;

        if (other.TryGetComponent<ZombieBehaviour>(out zombie))
        {
            if (!zombie.isActiveAndEnabled) return;
            StopHeal(false);

            _healingZombie = null;
        }
    }

    public void Heal()
    {
        _isHealing = true;

        _healValue = 0;

        _healingZombie.Heal();

        _statusCanvas.gameObject.SetActive(true);
    }

    public void StopHeal(bool isHealed)
    {
        _isHealing = false;

        _healValue = 0;

        _healingZombie.StopHeal(isHealed);

        _statusCanvas.gameObject.SetActive(false);
    }

    public float GetFollowRadius()
    {
        return _followRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _followRadius);
    }
}
