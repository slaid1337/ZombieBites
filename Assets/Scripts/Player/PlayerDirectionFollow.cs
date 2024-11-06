using UnityEngine;

public class PlayerDirectionFollow : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMove;

    private void FixedUpdate()
    {
        transform.LookAt(transform.position + _playerMove.MoveDirection);
    }
}
