using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody rb;
    private InputSystemControlls.PlayerActions _input;

    [SerializeField] private Transform _playerModel;
    private bool _isMoving = false;

    [SerializeField] private Animator _animator;

    [SerializeField] private int _inverse;

    public Vector3 MoveDirection;

    public bool IsStop;

    private void Start()
    {
        _input = new InputSystemControlls().Player;
        _input.Enable();

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsStop)
        {
            rb.linearVelocity = Vector3.zero;
            _isMoving = false;
            return;
        }

        Move();
    }

    private void Move()
    {
        if (!_isMoving && MoveDirection != Vector3.zero)
        {
            _animator.ResetTrigger("IsIdle");
            _animator.SetTrigger("IsRun");

            _isMoving = true;
        }

        Vector2 move = _input.Move.ReadValue<Vector2>();

        float moveHorizontal = move.x;
        float moveVertical = move.y;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        MoveDirection = movement;

        rb.linearVelocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);

        _playerModel.LookAt(_playerModel.transform.position + (movement * _inverse));

        if (MoveDirection == Vector3.zero)
        {
            _animator.SetTrigger("IsIdle");

            _isMoving = false;
        }
    }
}