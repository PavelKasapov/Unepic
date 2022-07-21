using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{
    private const float GroundCheckLenght = 0.1f;
    
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private new Collider2D collider;
    [SerializeField] private LayerMask whatIsGround = 2; 
    [SerializeField] private Transform groundLevelPoint;
    [SerializeField] private float speed = 2; 
    
    private PlayerControls _controls;
    private int _faceDirection = 1;
    private float _moveValue = 0f;
    private Coroutine _moveCoroutine;
    private Coroutine _groundCheckCoroutine;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.CommonMovement.HorisontalMovement.performed += ctx => Move(ctx.ReadValue<float>());
        _controls.CommonMovement.HorisontalMovement.canceled += ctx => Move(ctx.ReadValue<float>());
        _controls.CommonMovement.Jump.performed += ctx => Jump(ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        _controls.Enable();
    }
    
    private void OnDisable()
    {
        _controls.Disable();
    }

    /*private void FixedUpdate()
    {
        Jump(_controls.CommonMovement.Jump.ReadValue<float>());
        Move(_controls.CommonMovement.HorisontalMovement.ReadValue<float>());
    }*/

    void Move(float value)
    {
        Debug.Log($"!! Move {value}");
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

    IEnumerator MoveRoutine()
    {
        var moveValue = _moveValue;
        while (_moveValue != 0f || _groundCheckCoroutine != null)
        {
            if ( moveValue != _moveValue && _groundCheckCoroutine == null)
            {
                moveValue = _moveValue;
            }
            Debug.Log($"!! {_moveValue} {_groundCheckCoroutine != null}");
            rigidbody.velocity = new Vector2(moveValue * speed, rigidbody.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        
        rigidbody.velocity = new Vector2(_moveValue, rigidbody.velocity.y);
        _moveCoroutine = null;
    }
    
    IEnumerator DelayedMoveRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        
        while (!IsGrounded())
        {
            yield return new WaitForFixedUpdate();
        }

        Move(_moveValue);

        _groundCheckCoroutine = null;
    }

    void Jump(float value)
    {
        Debug.Log($"!! Jump {value}");
        if (IsGrounded())
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, value * 3);
            _groundCheckCoroutine ??= StartCoroutine(DelayedMoveRoutine());
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundLevelPoint.position, Vector2.down, GroundCheckLenght, whatIsGround).collider != null;
    }
}
