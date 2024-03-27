using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerCombat))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _knightRenderer;

    [Header("Sideways movement")]
    [SerializeField] private float _shieldSpeed = 4;
    [SerializeField] private float _walkSpeed = 7;
    [SerializeField] private float _runSpeed = 11;
    [SerializeField] private float _horizontalLerpFactor;

    [Header("Jump")]
    [SerializeField] private float _shieldJumpForce;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _maxJumpTime;
    [SerializeField] private float _gravityForce;
    private float _currentJumpTime;
    private bool _jumping;

    private Vector2 _inputDir = Vector2.zero;
    private Rigidbody2D _rb;

    [Header("Grounded")]
    [SerializeField] private Vector2 _groundedOffset;
    [SerializeField] private float _groundedRadius;

    [HideInInspector] public UnityEvent OnRunStart;
    [HideInInspector] public UnityEvent OnRunEnd;
    [HideInInspector] public bool Running;
    [HideInInspector] public bool Stunned;
    [SerializeField, ReadOnly] public bool InDangerZone;

    [SerializeField] private Animator _animator;

    private PlayerCombat _playerCombat;
    public bool movingRight { get; private set; }
    private Vector3 _safePosition;

    [Header("Sounds")]
    [SerializeField] private Sound _jumpSound;
    [SerializeField] private Sound _landSound;

    private void Start()
    {
        _jumpSound = Instantiate(_jumpSound);
        _landSound = Instantiate(_landSound);

        _rb = GetComponent<Rigidbody2D>();  
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        HandleAnimations();

        if (Stunned) return;
        handleHorizontalMovement();
        handleJumping();
    }

    private void HandleAnimations()
    {
        _animator.SetBool("running", Running && Mathf.Abs(_rb.velocity.x) > 0.01f);
        _animator.SetBool("walking", !Running && Mathf.Abs(_rb.velocity.x) > 0.05f);
        _animator.SetFloat("verticalVel", _rb.velocity.y);

        bool grounded = IsGrounded();
        if (!_animator.GetBool("grounded") && grounded) _landSound.Play();
        _animator.SetBool("grounded", grounded);
    }

    private void handleJumping()
    {
        bool pressingJump = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);
        bool jumpDown = (Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.W)) || (Input.GetKeyDown(KeyCode.W) && !Input.GetKey(KeyCode.Space));
        bool jumpUp = (Input.GetKeyUp(KeyCode.Space) && !Input.GetKey(KeyCode.W)) || (Input.GetKeyUp(KeyCode.W) && !Input.GetKey(KeyCode.Space));

        if (jumpDown && IsGrounded()) {
            _jumping = true;
            _jumpSound.Play();
        }
        if (jumpUp) _jumping = false;

        if (pressingJump && _currentJumpTime < _maxJumpTime && _jumping) {
            _currentJumpTime += Time.deltaTime;
            float currentJump = _playerCombat.ShieldUp ? _shieldJumpForce : _jumpForce;
            if (_rb.velocity.y < currentJump) _rb.velocity += Vector2.up * currentJump * Time.deltaTime;
        }
        else {
            _jumping = false;
            _currentJumpTime = 0;
            _inputDir.y = 0;
        }
    }

    private void handleHorizontalMovement()
    {
        if (Input.GetKey(KeyCode.A)) {
            _inputDir.x = -1;
            movingRight = false;
            _knightRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.D)) {
            _inputDir.x = 1;
            movingRight = true;
            _knightRenderer.flipX = false;
        }
        else {
            _inputDir.x = Mathf.Lerp(_inputDir.x, 0, _horizontalLerpFactor * Time.deltaTime);
        }

        Running = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.LeftShift)) OnRunStart.Invoke();
        else if (Input.GetKeyUp(KeyCode.LeftShift)) OnRunEnd.Invoke();
    }

    private void FixedUpdate()
    {
        if (Stunned) {
            _inputDir = Vector2.zero;
            return;
        }

        var vertical = _rb.velocity.y;
        if (_inputDir.y > 0) vertical = Mathf.Max(vertical, _inputDir.y * _jumpForce);
        var gravityForce = _jumping ? Vector2.zero : Vector2.down * _gravityForce;

        float speed = Running ? _runSpeed : _playerCombat.ShieldUp ? _shieldSpeed : _walkSpeed;
        _rb.velocity = new Vector2(_inputDir.x * speed, vertical) + gravityForce;
    }

    private bool IsGrounded()
    {
        bool grounded = false;
        var colliders = Physics2D.OverlapCircleAll(transform.position + (Vector3)_groundedOffset, _groundedRadius);
        foreach (var c in colliders) if (c.CompareTag("Ground")) grounded = true;
        if (grounded && !InDangerZone) _safePosition = transform.position;

        return grounded;
    }

    public void ResetToSafePosition()
    {
        transform.position = _safePosition;
        _rb.velocity = Vector2.zero;
        _inputDir = Vector2.zero;
        Stunned = true;
        Invoke(nameof(UnStun), 0.2f);
    }

    private void UnStun() => Stunned = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3) _groundedOffset, _groundedRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_safePosition, 0.1f);
        Debug.DrawLine(transform.position, _safePosition, Color.green);
    }
}
