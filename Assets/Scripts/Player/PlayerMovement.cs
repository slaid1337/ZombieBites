using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody rb;
    private InputSystemControlls.PlayerActions _input;

    public Vector3 MoveDirection;

    private void Start()
    {
        _input = new InputSystemControlls().Player;
        _input.Enable();

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 move = _input.Move.ReadValue<Vector2>();

        float moveHorizontal = move.x;
        float moveVertical = move.y;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        MoveDirection = movement;

        rb.linearVelocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);
    }
}