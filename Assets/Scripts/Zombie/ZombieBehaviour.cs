using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBehaviour : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    [SerializeField] private float _fleeDistance = 10f; // Расстояние, на которое зомби будет убегать
    [SerializeField] private float _healDistance;
    private NavMeshAgent agent; // NavMeshAgent для перемещения зомби

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        // Расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < _fleeDistance)
        {
            // Направление от игрока
            Vector3 fleeDirection = (transform.position - player.position).normalized;

            // Вычисляем новую точку, куда будет убегать зомби
            Vector3 fleeTarget = transform.position + fleeDirection * _fleeDistance;

            // Установка целевой точки для NavMeshAgent
            agent.SetDestination(fleeTarget);

            Debug.DrawLine(transform.position, fleeTarget, Color.blue);
        }
        else
        {
            // Если игрок далеко, можно остановить зомби или добавить другую логику
            agent.ResetPath();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _fleeDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _healDistance);
    }
#endif
}