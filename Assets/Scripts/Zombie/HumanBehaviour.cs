using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HumanBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Canvas _statusCanvas;
    [SerializeField] private CircleIndicator _circleIndicator;
    private ZombieHumanChange _skinChange;

    private float _zombifictionValue;
    private bool _isZombifying;

    private bool _isCollected;

    private NavMeshAgent _agent; // NavMeshAgent для перемещения зомби

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

        }
        else
        {
            _agent.ResetPath();
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

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnSave(SaveZone zone)
    {
        if (_isCollected) return;

        _agent.SetDestination(zone.GetRandomPoint());

        _isCollected = true;

        HumanPool.Instance.RemoveHuman(this);
        OnDangerExit?.Invoke();

        LevelController.Instance.AddHuman();

        print("Collect");

        _player.OnHumanSave.RemoveListener(OnSave);
    }

    public void Zombification()
    {
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
