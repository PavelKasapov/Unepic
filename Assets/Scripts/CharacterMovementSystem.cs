using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class CharacterMovementSystem : MonoBehaviour
{
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private float speed = 2;
    [SerializeField] private EnvironmentTouchChecker environmentTouchChecker;
    [SerializeField] private KnightAnimatorAdapter animationAdapter;

    [Inject] private RangedAttackManager _rangedAttackManager;
    
    private int _faceDirection = 1;
    private float _moveValue;
    private bool _isAnimated;
    private Coroutine _moveCoroutine;
    private Coroutine _wallSlideCheckCoroutine;
    private DateTime _comboExpireTime;
    private bool _usedSlideJump;
    private bool _isWallSliding;
    private IDisposable afterWallJumpMovementSubscription;

    private int FaceDirection
    {
        set
        {
            
            if (value != _faceDirection)
            {
                _faceDirection *= -1;
                var thisTransform = transform;
                var localScale = thisTransform.localScale;
                localScale.x = _faceDirection;
                thisTransform.localScale = localScale;
            }
        }
    }

    private void Awake()
    {
        environmentTouchChecker.TouchingWallsDirection.Subscribe(WallSlideCheck);
        environmentTouchChecker.IsTouchingGround.Subscribe(_ => _usedSlideJump = false);
    }

    /*public void ShootAnimation ()
    {
        return;
        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spellcast")) 
            playerAnimator.SetTrigger("spellcast");
    }
    
    public void Shoot ()
    {
        _rangedAttackManager.Launch(RangedAttackType.Fireball, OriginType.Player, transform.position, new Vector2(_faceDirection, 0));
    }*/

    public void Move(float value)
    {
        if (value != 0f && !_usedSlideJump)
        {
            FaceDirection = Math.Sign(value);
        }
        
        _moveValue = value;
        animationAdapter.SetMovingState(value);
        _moveCoroutine ??= StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (_moveValue != 0f || Math.Abs(rigidbody.velocity.x) > 0.5f)
        {
            if (!_usedSlideJump && (_moveValue > 0 && rigidbody.velocity.x < speed 
                               || _moveValue < 0 && rigidbody.velocity.x > -speed))
            {
                rigidbody.velocity += new Vector2(_moveValue * 1, 0);
            }
            else if (environmentTouchChecker.IsTouchingGround.Value)
            {
                rigidbody.velocity -= new Vector2(Math.Sign(rigidbody.velocity.x) * 0.4f, 0);
            }
            yield return new WaitForFixedUpdate();
        }
        
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        _moveCoroutine = null;
    }
    
    public void Jump(float value)
    {
        if (environmentTouchChecker.IsTouchingGround.Value)
        {
            animationAdapter.TriggerJumpingState();
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, value * 14);
        }
        else if (_isWallSliding 
                 && environmentTouchChecker.TouchingWallsDirection.Value == Math.Sign(_moveValue) 
                 && !_usedSlideJump)
        {
            animationAdapter.TriggerJumpingState();
            rigidbody.velocity = new Vector2(-_moveValue * 8, value * 16);
            
            FaceDirection = -Math.Sign(_moveValue);
            afterWallJumpMovementSubscription = environmentTouchChecker.IsTouchingGround.Where(val => val).Subscribe(_ =>
            {
                Move(_moveValue);
                afterWallJumpMovementSubscription.Dispose();
            });
            _usedSlideJump = true;
        }
    }

    private void WallSlideCheck(int touchingWallsDirection)
    {
        if (touchingWallsDirection != 0)
        {
            _wallSlideCheckCoroutine ??= StartCoroutine(WallSlideCheckRoutine(touchingWallsDirection));
        }
        else
        {
            if (_wallSlideCheckCoroutine != null)
            {
                StopCoroutine(_wallSlideCheckCoroutine);
                _wallSlideCheckCoroutine = null;
                
                animationAdapter.SetWallSlideState(false);
                rigidbody.drag =  1f;
            }
        }
    }

    private IEnumerator WallSlideCheckRoutine(int touchingWallsDirection)
    {
        while (true)
        {
            var isWallSliding = _moveValue != 0 
                                && !environmentTouchChecker.IsTouchingGround.Value
                                && touchingWallsDirection == Math.Sign(_moveValue)
                                && rigidbody.velocity.y < 0;

            if (_isWallSliding != isWallSliding)
            {
                animationAdapter.SetWallSlideState(isWallSliding);
                rigidbody.drag = isWallSliding ? 10f : 1f;
                _isWallSliding = isWallSliding;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void Attack()
    {
        animationAdapter.TriggerAttackState();
    }
}