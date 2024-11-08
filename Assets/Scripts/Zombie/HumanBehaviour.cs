using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HumanBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Canvas _statusCanvas;
    [SerializeField] private CircleIndicator _circleIndicator;
    private ZombieHumanChange _skinChange;

    [SerializeField] private Animator _animator;

    private float _zombifictionValue;
    private bool _isZombifying;

    private bool _isCollected;

    private NavMeshAgent _agent;

    private bool _isRunPlayer;

    public UnityEvent OnDangerExit;
    public UnityEvent OnZombify;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _skinChange = GetComponent<ZombieHumanChange>();
    }

    private void OnEnable()
    {
        _player.OnHumanSave.AddListener(OnSave);
    }

    private void OnDisable()
    {
        _player.OnHumanSave.RemoveListener(OnSave);
    }

    private void FixedUpdate()
    {
        if (_isCollected)
        {
            return;
        }

        if (!LevelController.Instance.IsPlay())
        {
            _agent.isStopped = true;

            return;
        }

        _agent.isStopped = false;

        if (_isZombifying)
        {
            _zombifictionValue += Time.fixedDeltaTime;

            if (_zombifictionValue >= 5f)
            {
                StopZombification(true);
            }

            _circleIndicator.UpdateStatus(Serializer.Normalize(_zombifictionValue, 0, 5));

            return;
        }

        // Расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer > _player.GetFollowRadius())
        {
            // Установка целевой точки для NavMeshAgent
            _agent.SetDestination(_player.transform.position);

            if (!_isRunPlayer)
            {
                _animator.SetTrigger("IsRun");
            }

            _isRunPlayer = true;
        }
        else
        {
            _agent.ResetPath();

            if (_isRunPlayer)
            {
                _animator.SetTrigger("IsIdle");
            }

            _isRunPlayer = false;
        }
    }

    public bool isDanger()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_player.CanDamage()) return true;

        if (distanceToPlayer > _player.GetSaveRadius())
        {
            return true;
        }

        return false;
    }

    public void OnSave(SaveZone zone)
    {
        if (_isCollected) return;

        StartCoroutine(SaveCor(zone));
    }

    private IEnumerator SaveCor(SaveZone zone)
    {
        _animator.SetTrigger("IsWalk");

        Vector3 point = zone.GetRandomPoint();

        _agent.SetDestination(point);

        _isCollected = true;

        HumanPool.Instance.RemoveHuman(this);
        OnDangerExit?.Invoke();

        LevelController.Instance.AddHuman();

        print("Collect");

        _player.OnHumanSave.RemoveListener(OnSave);

        yield return new WaitUntil(() => Vector3.Distance(point, transform.position) <= 2f);

        _animator.SetTrigger("IsIdle");
    }

    public void Zombification()
    {
        _animator.SetTrigger("IsIdle");

        _isZombifying = true;

        _agent.ResetPath();

        _statusCanvas.gameObject.SetActive(true);

        _zombifictionValue = 0;

        HumanPool.Instance.RemoveHuman(this);
        OnDangerExit?.Invoke();
    }

    public void StopZombification(bool isDead)
    {
        _statusCanvas.gameObject.SetActive(false);

        _zombifictionValue = 0;

        if (!isDead)
        {
            _isZombifying = false;

            HumanPool.Instance.AddHuman(this);

            _animator.SetTrigger("IsRun");
        }
        else
        {
            _agent.areaMask = 5;
            OnZombify?.Invoke();
            _agent.speed = 3.5f;
            _isZombifying = false;
            _skinChange.SwitchState(true);
        }
    }
}
