using UnityEngine;
using UnityEngine.AI;

public class HumanBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    private ZombieHumanChange _skinChange;

    private NavMeshAgent _agent; // NavMeshAgent для перемещения зомби

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
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
}
