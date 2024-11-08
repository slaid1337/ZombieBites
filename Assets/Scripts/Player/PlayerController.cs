using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Singletone<PlayerController>
{
    private PlayerSuitController _suitController;
    private PlayerMovement _moveController;
    [SerializeField] private GameBalance _gameBalance;
    [SerializeField] private Canvas _statusCanvas;
    [SerializeField] private CircleIndicator _circleIndicator;
    [SerializeField] private float _followRadius;
    [SerializeField] private float _saveRadius;

    [SerializeField] private Animator _animator;

    [SerializeField] private AudioSource _audioSource;

    private bool _isHealing;
    private float _healValue;

    private bool _isSaving;
    private bool _isTrySaving;
    private float _saveValue;

    private ZombieBehaviour _healingZombie;

    public UnityEvent<SaveZone> OnHumanSave;

    public UnityEvent OnDie;

    private SaveZone _zone;

    public override void Awake()
    {
        base.Awake();

        _suitController = GetComponent<PlayerSuitController>();
        _moveController = GetComponent<PlayerMovement>();

        _suitController.Init(_gameBalance.SuitStrength);
    }

    private void Start()
    {
        LevelController.Instance.OnStop.AddListener(delegate
        {
            _moveController.IsStop = true;
        });

        LevelController.Instance.OnResume.AddListener(delegate
        {
            _moveController.IsStop = false;
        });
    }

    private void FixedUpdate()
    {
        if (!LevelController.Instance.IsPlay()) return;

        if (_isTrySaving)
        {
            if (HumanPool.Instance.GetHumansCount() > HumanPool.Instance.GetDangerHumans().Count)
            {
                _isTrySaving = false;
                _statusCanvas.gameObject.SetActive(true);
                _isSaving = true;
            }

            return;
        }

        if (_isSaving)
        {
            _saveValue += Time.fixedDeltaTime * _gameBalance.SavingSpeed;

            if (_saveValue >= 2f)
            {
                CancelSave(true);
            }

            _circleIndicator.UpdateStatus(Serializer.Normalize(_saveValue, 0, 2));

            return;
        }

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
            if (CanDamage()) return;
            if (!zombie.isActiveAndEnabled) return;
            _healingZombie = zombie;

            Heal();
        }

        SaveZone zone = null;

        if (other.TryGetComponent<SaveZone>(out zone))
        {
            _zone = zone;
            SaveHumans();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ZombieBehaviour zombie = null;

        if (other.TryGetComponent<ZombieBehaviour>(out zombie))
        {
            if (_healingZombie == null) return;
            if (!zombie.isActiveAndEnabled) return;
            StopHeal(false);
            _healingZombie = null;
        }

        SaveZone zone = null;

        if (other.TryGetComponent<SaveZone>(out zone))
        {
            CancelSave(false);
        }
    }

    public void Heal()
    {
        _isHealing = true;

        _healValue = 0;

        _healingZombie.Heal();

        _statusCanvas.gameObject.SetActive(true);

        _animator.SetTrigger("IsHit");
        _animator.ResetTrigger("IsRun");

        _moveController.IsStop = true;

        Vector3 pos = _healingZombie.transform.position;
        pos.y = _animator.transform.position.y;

        _animator.transform.LookAt(pos);
    }

    public void StopHeal(bool isHealed)
    {
        _isHealing = false;

        _healValue = 0;

        _healingZombie.StopHeal(isHealed);

        _statusCanvas.gameObject.SetActive(false);

        _moveController.IsStop = false;
    }

    public void SaveHumans()
    {
        if (HumanPool.Instance.GetHumansCount() == HumanPool.Instance.GetDangerHumans().Count)
        {
            _isTrySaving = true;

            _saveValue = 0;
            _statusCanvas.gameObject.SetActive(false);

            return;
        }

        _audioSource.Play();

        print("save");

        _isSaving = true;

        _saveValue = 0;
        _statusCanvas.gameObject.SetActive(true);
    }

    public void CancelSave(bool isSaved)
    {
        if (_isTrySaving && !isSaved)
        {
            _isTrySaving = false;
        }

        if (!_isSaving) return;

        if (isSaved)
        {
            OnHumanSave?.Invoke(_zone);
        }

        _isSaving = false;

        _saveValue = 0;
        _statusCanvas.gameObject.SetActive(false);
    }

    public bool CanDamage()
    {
        return _suitController.InDanger();
    }

    public float GetFollowRadius()
    {
        return _followRadius;
    }

    public float GetSaveRadius()
    {
        return _saveRadius;
    }

    public void Damage(float damage)
    {
        if (!LevelController.Instance.IsPlay()) return;

        _suitController.DamageSuit(damage);
        print("bite");
        if (_suitController.IsGone())
        {
            LevelController.Instance.GetResult(true);
            print("dead");
            OnDie?.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _followRadius);

        Gizmos.DrawWireSphere(transform.position, _saveRadius);
    }
#endif
}
