using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _player; // Ссылка на игрока
    [SerializeField] private float _fleeDistance = 10f; // Расстояние, на которое зомби будет убегать
    [SerializeField] private float _healDistance;
    [SerializeField] private float _roamRadius = 10f;
    [SerializeField] private float _damage;

    [SerializeField] private GameBalance _gameBalance;

    private NavMeshAgent _agent; // NavMeshAgent для перемещения зомби
    private ZombieHumanChange _skinChange;

    /*private ZombieState state;

    private ZombieState _state
    {
        get { return state; }
        set { state = value;  print(value); }
    }*/

    private ZombieState _state;

    private HumanBehaviour _pursuitHuman;

    private Coroutine _roamCoroutine;
    private Coroutine _biteCoroutine;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _skinChange = GetComponent<ZombieHumanChange>();

        _damage = _gameBalance.Damage;

        _state = ZombieState.Stay;
    }

    private void FixedUpdate()
    {
        if (_state == ZombieState.Healing || _state == ZombieState.Biting)
        {
            return;
        }

        // Расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer < _fleeDistance)
        {
            if (_player.CanDamage())
            {
                _agent.SetDestination(_player.transform.position);

                _state = ZombieState.PursuitPlayer;

                return;
            }
            // Направление от игрока
            Vector3 fleeDirection = (transform.position - _player.transform.position).normalized;

            // Вычисляем новую точку, куда будет убегать зомби
            Vector3 fleeTarget = transform.position + fleeDirection * _fleeDistance;

            // Установка целевой точки для NavMeshAgent
            _agent.SetDestination(fleeTarget);

            Debug.DrawLine(transform.position, fleeTarget, Color.blue);

            _state = ZombieState.Runaway;

            if (_roamCoroutine != null )
            {
                StopCoroutine( _roamCoroutine );
            }
        }
        else
        {
            if (_state == ZombieState.Zombification)
            {
                _agent.ResetPath();

                return;
            }

            if (_state == ZombieState.Pursuit)
            {
                _agent.SetDestination(_pursuitHuman.transform.position);
            }

            // Если игрок далеко, можно остановить зомби или добавить другую логику
            if (_state == ZombieState.Stay || _state == ZombieState.Runaway || _state == ZombieState.PursuitPlayer)
            {
                _state = ZombieState.Roam;

                Roam();
            }
            else if ( _state != ZombieState.Pursuit && _state != ZombieState.PursuitPlayer)
            {
                List<HumanBehaviour> dangerHumans = HumanPool.Instance.GetDangerHumans();

                if (dangerHumans.Count > 0 )
                {
                    _state = ZombieState.Pursuit;

                    HumanBehaviour pursuitHuman = dangerHumans[Random.Range(0, dangerHumans.Count)];

                    _pursuitHuman = pursuitHuman;

                    _pursuitHuman.OnDangerExit.AddListener(ClearPursuit);
                    _pursuitHuman.OnZombify.AddListener(ClearPursuit);

                    _agent.SetDestination(_pursuitHuman.transform.position);

                    if (_roamCoroutine != null)
                    {
                        StopCoroutine(_roamCoroutine);

                        _roamCoroutine = null;
                    }
                }
            }
        }
    }

    private void Roam()
    {
        if (_roamCoroutine != null) return;
        _roamCoroutine = StartCoroutine(RoamCor());
    }

    private IEnumerator RoamCor()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 6f));

        // Определяем случайную позицию в пределах радиуса
        Vector3 randomDirection = Random.insideUnitSphere * _roamRadius;
        randomDirection += transform.position;

        // Проверяем, что позиция на NavMesh
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _roamRadius, NavMesh.AllAreas);

        // Перемещаем зомби к новой позиции
        _agent.SetDestination(hit.position);
        Debug.DrawLine(transform.position, hit.position, Color.blue);

        yield return new WaitUntil(() => _agent.remainingDistance <= 0.3f);

        _state = ZombieState.Stay;

        // Вызываем метод снова через случайный интервал
        _roamCoroutine = StartCoroutine(RoamCor());
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _fleeDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _healDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _roamRadius);
    }
#endif

    public void Heal()
    {
        _state = ZombieState.Healing;

        _agent.ResetPath();
        _agent.velocity = Vector3.zero;

        if (_roamCoroutine != null) 
        {
            StopCoroutine( _roamCoroutine );

            _roamCoroutine = null;
        }
    }

    public void StopHeal(bool isHealed)
    {
        if (!isHealed)
        {
            _state = ZombieState.Runaway;
        }
        else
        {
            _agent.areaMask = 13;
            _agent.speed = 2f;
            _skinChange.SwitchState(false);
            _state = ZombieState.Runaway;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HumanBehaviour human = null;

        if (other.TryGetComponent<HumanBehaviour>(out human))
        {
            if (human == _pursuitHuman)
            {
                _pursuitHuman.OnDangerExit.RemoveListener(ClearPursuit);
                _pursuitHuman.OnZombify.RemoveListener(ClearPursuit);

                human.Zombification();
                _agent.ResetPath();
                _agent.velocity = Vector3.zero;
                _state = ZombieState.Zombification;

                if (_roamCoroutine != null)
                {
                    StopCoroutine(_roamCoroutine );

                    _roamCoroutine = null;
                }
            }
        }

        PlayerController player = null;

        if (other.TryGetComponent<PlayerController>(out player))
        {
            if (player.CanDamage())
            {
                _state = ZombieState.Biting;
                _agent.ResetPath();
                _agent.velocity = Vector3.zero;

                _biteCoroutine = StartCoroutine(BiteCor());

                if (_roamCoroutine != null)
                {
                    StopCoroutine(_roamCoroutine);

                    _roamCoroutine = null;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HumanBehaviour human = null;

        if (other.TryGetComponent<HumanBehaviour>(out human))
        {
            if (human == _pursuitHuman)
            {
                _pursuitHuman.OnDangerExit.AddListener(ClearPursuit);
                _pursuitHuman.OnZombify.AddListener(ClearPursuit);
                human.StopZombification(false);
            }
        }

        PlayerController player = null;

        if (other.TryGetComponent<PlayerController>(out player))
        {
            if (player.CanDamage())
            {
                _state = ZombieState.Stay;

                if (_biteCoroutine != null)
                {
                    StopCoroutine(_biteCoroutine);

                    _biteCoroutine = null;
                }
            }
        }
    }

    public IEnumerator BiteCor()
    {
        BitePlayer();

        yield return new WaitForSeconds(_gameBalance.BiteCooldown);

        _biteCoroutine = StartCoroutine(BiteCor());
    }

    public void BitePlayer()
    {
        _player.Damage(_damage);
    }

    public void ClearPursuit()
    {
        _pursuitHuman.OnDangerExit.RemoveListener(ClearPursuit);
        _pursuitHuman.OnZombify.RemoveListener(ClearPursuit);

        _pursuitHuman = null;

        _state = ZombieState.Stay;
    }
}

public enum ZombieState
{
    Roam,
    Stay,
    Runaway, 
    Healing,
    Pursuit,
    PursuitPlayer,
    Zombification,
    Biting
}