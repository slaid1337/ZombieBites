using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения
    public float jumpForce = 5f; // Сила прыжка
    private Rigidbody rb;
    private InputSystemControlls.PlayerActions _input;

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

        // Вектор движения
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        rb.linearVelocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);
    }
}