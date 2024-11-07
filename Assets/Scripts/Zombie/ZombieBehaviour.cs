using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows.WebCam;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _player; // Ссылка на игрока
    [SerializeField] private float _fleeDistance = 10f; // Расстояние, на которое зомби будет убегать
    [SerializeField] private float _healDistance;
    [SerializeField] private float _roamRadius = 10f;
    [SerializeField] private float _damage;

    [SerializeField] private GameBalance _gameBalance;

    [SerializeField] private Animator _animator;

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

        _animator.SetFloat("IdleIndex", Random.Range(0f, 1f));
    }

    private void FixedUpdate()
    {
        if (!LevelController.Instance.IsPlay())
        {
            _agent.isStopped = true;

            return;
        }

        _agent.isStopped = false;

        if (_state == ZombieState.Healing || _state == ZombieState.Biting)
        {
            return;
        }

        // Расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer < _fleeDistance)
        {
            _agent.speed = 3.5f;

            if (_player.CanDamage())
            {
                if (_state != ZombieState.PursuitPlayer)
                {
                    _animator.SetTrigger("IsRun");
                }

                _agent.SetDestination(_player.transform.position);

                _state = ZombieState.PursuitPlayer;

                return;
            }
            // Направление от игрока
            Vector3 fleeDirection = (transform.position - _player.transform.position).normalized;

            // Вычисляем новую точку, куда будет убегать зомби
            Vector3 fleeTarget = transform.position + fleeDirection * _fleeDistance * 1.5f;

            // Установка целевой точки для NavMeshAgent
            _agent.SetDestination(fleeTarget);

            Debug.DrawLine(transform.position, fleeTarget, Color.blue);

            if (_state != ZombieState.Runaway)
            {
                _state = ZombieState.Runaway;

                print("run");
                _animator.ResetTrigger("IsIdle");
                _animator.SetTrigger("IsRun");
            }

            if (_roamCoroutine != null )
            {
                StopCoroutine( _roamCoroutine );

                _roamCoroutine = null;
            }
        }
        else
        {
            if (_state == ZombieState.Zombification)
            {
                _agent.ResetPath();
                _animator.SetTrigger("IsAttack");
                return;
            }

            if (_state == ZombieState.Pursuit)
            {
                _agent.SetDestination(_pursuitHuman.transform.position);

                _agent.speed = 3f;
            }

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
                    foreach (var item in dangerHumans)
                    {
                        if (Vector3.Distance(item.transform.position, transform.position) <= _roamRadius)
                        {
                            _pursuitHuman = item;
                        }
                    }

                    if (_pursuitHuman == null) return;

                    _state = ZombieState.Pursuit;

                    _pursuitHuman.OnDangerExit.AddListener(ClearPursuit);
                    _pursuitHuman.OnZombify.AddListener(ClearPursuit);

                    _animator.SetTrigger("IsRun");

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

        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
        _agent.speed = 0.5f;

        _animator.SetTrigger("IsIdle");

        _roamCoroutine = StartCoroutine(RoamCor());
    }

    private IEnumerator RoamCor()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        _animator.ResetTrigger("IsIdle");
        _animator.SetTrigger("IsWalk");

        // Определяем случайную позицию в пределах радиуса
        Vector3 randomDirection = Random.insideUnitSphere * _roamRadius;
        randomDirection += transform.position;

        // Проверяем, что позиция на NavMesh
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _roamRadius, NavMesh.AllAreas);

        // Перемещаем зомби к новой позиции
        _agent.SetDestination(hit.position);
        Debug.DrawLine(transform.position, hit.position, Color.blue);

        
        yield return new WaitUntil(() => Vector3.Distance(hit.position, transform.position) <= 1.1f);

        _animator.SetFloat("IdleIndex", Random.Range(0f, 1f));
        _animator.SetTrigger("IsIdle");

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
        _animator.SetTrigger("IsIdle");
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
            _state = ZombieState.Stay;
        }
        else
        {
            _agent.areaMask = 13;
            _agent.speed = 2f;
            _skinChange.SwitchState(false);
            _state = ZombieState.Stay;
        }

        _animator.ResetTrigger("IsIdle");
    }

    private void OnTriggerEnter(Collider other)
    {
        HumanBehaviour human = null;

        if (other.TryGetComponent<HumanBehaviour>(out human))
        {
            if (human == _pursuitHuman)
            {
                _pursuitHuman.OnDangerExit.RemoveListener(ClearPursuit);

                human.Zombification();
                
                StartCoroutine(ZombificationCor());

                _animator.SetTrigger("IsAttack");

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
                ClearPursuit();

                human.StopZombification(false);
            }
        }

        PlayerController player = null;

        if (other.TryGetComponent<PlayerController>(out player))
        {
            if (player.CanDamage())
            {
                _state = ZombieState.Stay;

                _animator.ResetTrigger("IsIdle");

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

        _animator.SetTrigger("IsAttack");

        yield return new WaitForSeconds(_gameBalance.BiteCooldown);

        _biteCoroutine = StartCoroutine(BiteCor());
    }

    public void BitePlayer()
    {
        _player.Damage(_damage);
    }

    public void ClearPursuit()
    {
        if (_state != ZombieState.Pursuit && _state != ZombieState.Zombification) return;

        print("cleare purs");

        _animator.ResetTrigger("IsAttack");

        _pursuitHuman.OnDangerExit.RemoveListener(ClearPursuit);
        _pursuitHuman.OnZombify.RemoveListener(ClearPursuit);

        _pursuitHuman = null;

        _state = ZombieState.Stay;
    }

    public IEnumerator ZombificationCor()
    {
        yield return new WaitUntil(() => Vector3.Distance(_pursuitHuman.transform.position, transform.position) <= 1.1f);

        _state = ZombieState.Zombification;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
    }

    public void SelectState()
    {
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