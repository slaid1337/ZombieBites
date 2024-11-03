using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBehaviour : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    [SerializeField] private float _fleeDistance = 10f; // Расстояние, на которое зомби будет убегать
    [SerializeField] private float _healDistance;
    [SerializeField] private float _roamRadius = 10f;
    private NavMeshAgent agent; // NavMeshAgent для перемещения зомби

    private ZombieState _state;

    private Coroutine _roamCoroutine;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        _state = ZombieState.Stay;
        Invoke("Roam", Random.Range(0.2f, 6f));
    }

    private void FixedUpdate()
    {
        // Расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (_state == ZombieState.Healing)
        {
            

            return;
        }

        if (distanceToPlayer < _fleeDistance)
        {
            // Направление от игрока
            Vector3 fleeDirection = (transform.position - player.position).normalized;

            // Вычисляем новую точку, куда будет убегать зомби
            Vector3 fleeTarget = transform.position + fleeDirection * _fleeDistance;

            // Установка целевой точки для NavMeshAgent
            agent.SetDestination(fleeTarget);

            Debug.DrawLine(transform.position, fleeTarget, Color.blue);

            _state = ZombieState.Runaway;

            if (_roamCoroutine != null )
            {
                StopCoroutine( _roamCoroutine );
            }
        }
        else
        {
            // Если игрок далеко, можно остановить зомби или добавить другую логику
            if (_state != ZombieState.Roam && _state != ZombieState.Stay)
            {
                _state = ZombieState.Stay;

                Invoke("Roam", Random.Range(0.2f, 6f));
            }
        }
    }

    private void Roam()
    {
        _state = ZombieState.Roam;

        // Определяем случайную позицию в пределах радиуса
        Vector3 randomDirection = Random.insideUnitSphere * _roamRadius;
        randomDirection += transform.position;

        // Проверяем, что позиция на NavMesh
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _roamRadius, NavMesh.AllAreas);

        // Перемещаем зомби к новой позиции
        agent.SetDestination(hit.position);
        Debug.DrawLine(transform.position, hit.position, Color.blue);

        _roamCoroutine = StartCoroutine(RoamCor());
    }

    private IEnumerator RoamCor()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0.3f);

        _state = ZombieState.Stay;

        // Вызываем метод снова через случайный интервал
        Invoke("Roam", Random.Range(3f, 6f));
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

        agent.ResetPath();
    }

    public void StopHeal(bool isHealed)
    {
        if (!isHealed)
        {
            _state = ZombieState.Runaway;
        }
    }
}

public enum ZombieState
{
    Roam,
    Stay,
    Runaway, 
    Healing
}