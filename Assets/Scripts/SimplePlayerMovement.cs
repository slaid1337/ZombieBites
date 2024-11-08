using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody rb;
    private InputSystemControlls.PlayerActions _input;
    private bool _isMoving = false;

    [SerializeField] private GameLoader _loader;

    [SerializeField] private Transform _playerModel;

    [SerializeField] private Animator _animator;

    private bool _canMove;

    public Vector3 MoveDirection;

    private void Awake()
    {
        _loader.OnLoad.AddListener(OnLoad);
    }

    private void Start()
    {
        _input = new InputSystemControlls().Player;
        _input.Enable();

        rb = GetComponent<Rigidbody>();
    }

    private void OnLoad()
    {
        _canMove = true;

        _loader.OnLoad.RemoveListener(OnLoad);
    }

    private void Update()
    {
        if (!_canMove) return;

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

        _playerModel.LookAt(_playerModel.transform.position - movement);

        if (MoveDirection == Vector3.zero)
        {
            _animator.SetTrigger("IsIdle");

            _isMoving = false;
        }
    }
}
