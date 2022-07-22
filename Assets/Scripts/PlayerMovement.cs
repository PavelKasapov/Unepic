using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float GroundCheckLenght = 0.1f;

    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private new BoxCollider2D collider;
    [SerializeField] private LayerMask whatIsGround; 
    [SerializeField] private Transform groundLevelPoint;
    [SerializeField] private float speed = 2; 
    
    private PlayerControls _controls;
    private Vector2 _boxCastSize;
    private int _faceDirection = 1;
    private float _moveValue;
    private bool _isGrounded;
    private Coroutine _moveCoroutine;
    private Coroutine _groundCheckCoroutine;

    private void Awake()
    {
        _controls = new PlayerControls();
        _boxCastSize = new Vector2(collider.size.x * 0.5f, GroundCheckLenght);
        
        _controls.CommonMovement.HorisontalMovement.performed += ctx => Move(ctx.ReadValue<float>());
        _controls.CommonMovement.HorisontalMovement.canceled += ctx => Move(ctx.ReadValue<float>());
        _controls.CommonMovement.Jump.performed += ctx => Jump(ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        _controls.Enable();
        _groundCheckCoroutine ??= StartCoroutine(GroundCheckRoutine());
    }
    
    private void OnDisable()
    {
        _controls.Disable();
        if (_groundCheckCoroutine != null)
        {
            StopCoroutine(_groundCheckCoroutine);
            _groundCheckCoroutine = null;
        }
    }

    private void Move(float value)
    {
        if (value != 0f)
        {
            if (Math.Sign(value) != _faceDirection)
            {
                _faceDirection *= -1;
                var thisTransform = transform;
                var localScale = thisTransform.localScale;
                localScale.x *= -1;
                thisTransform.localScale = localScale;
            }
        }
        
        _moveValue = value;
        
        if (IsGrounded())
        {
            _moveCoroutine ??= StartCoroutine(MoveRoutine());
        }
    }
    
    private IEnumerator MoveRoutine()
    {
        var moveValue = _moveValue;
        while (_moveValue != 0f || !_isGrounded)
        {
            if ( moveValue != _moveValue && _isGrounded)
            {
                moveValue = _moveValue;
            }
            rigidbody.velocity = new Vector2(moveValue * speed, rigidbody.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        
        rigidbody.velocity = new Vector2(_moveValue, rigidbody.velocity.y);
        _moveCoroutine = null;
    }
    
    private void Jump(float value)
    {
        if (IsGrounded())
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, value * 5);
            
        }
    }

    private IEnumerator GroundCheckRoutine()
    {
        while (true)
        {
            var value = IsGrounded();
            Debug.Log($"!! IsGrounded {value}");
            if (_isGrounded != value)
            {
                _isGrounded = value;
                
                if (value)
                    _moveCoroutine ??= StartCoroutine(MoveRoutine());
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
    
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(groundLevelPoint.position, _boxCastSize, 0, Vector2.down, GroundCheckLenght, whatIsGround).collider != null;
    }
}
