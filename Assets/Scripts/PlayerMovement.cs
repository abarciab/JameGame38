using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerCombat))]
public class PlayerMovement : MonoBehaviour
{
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

    private PlayerCombat _playerCombat;
    public bool movingRight { get; private set; }


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();  
        _playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (Stunned) return;
        handleHorizontalMovement();
        handleJumping();
    }

    private void handleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) _jumping = true;
        if (Input.GetKeyUp(KeyCode.Space)) _jumping = false;

        if (Input.GetKey(KeyCode.Space) && _currentJumpTime < _maxJumpTime && _jumping) {
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
        }
        else if (Input.GetKey(KeyCode.D)) {
            _inputDir.x = 1;
            movingRight = true;
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
        if (Stunned) return;
        var vertical = _rb.velocity.y;
        if (_inputDir.y > 0) vertical = Mathf.Max(vertical, _inputDir.y * _jumpForce);
        var gravityForce = _jumping ? Vector2.zero : Vector2.down * _gravityForce;

        float speed = Running ? _runSpeed : _playerCombat.ShieldUp ? _shieldSpeed : _walkSpeed;
        _rb.velocity = new Vector2(_inputDir.x * speed, vertical) + gravityForce;
    }

    private bool IsGrounded()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position + (Vector3)_groundedOffset, _groundedRadius);
        foreach (var c in colliders) if (c.CompareTag("Ground")) return true;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3) _groundedOffset, _groundedRadius);
    }
}
