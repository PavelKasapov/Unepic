using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CharacterMovementSystem : MonoBehaviour
{
    private const float GroundCheckLenght = 0.1f;

    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private new BoxCollider2D collider;
    [SerializeField] private LayerMask whatIsGround; 
    [SerializeField] private Transform groundLevelPoint;
    [SerializeField] private float speed = 2; 
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private GameObject FireballPrefab;

    [Inject] private RangedAttackManager _rangedAttackManager;
    
    private Vector2 _boxCastSize;
    private int _faceDirection = 1;
    private float _moveValue;
    private bool _isGrounded;
    private bool _isAnimated;
    private Coroutine _moveCoroutine;
    private Coroutine _groundCheckCoroutine;

    private void Awake()
    {
        _boxCastSize = new Vector2(collider.size.x * 0.5f, GroundCheckLenght);
    }

    private void OnEnable()
    {
        _groundCheckCoroutine ??= StartCoroutine(GroundCheckRoutine());
    }
    
    private void OnDisable()
    {
        if (_groundCheckCoroutine != null)
        {
            StopCoroutine(_groundCheckCoroutine);
            _groundCheckCoroutine = null;
        }
    }

    public void Shoot ()
    {
        _rangedAttackManager.Launch(RangedAttackType.Fireball, OriginType.Player, transform.position, new Vector2(_faceDirection, 0));
        //Instantiate(FireballPrefab, transform.position, Quaternion.identity).GetComponent<Fireball>().Launch(new Vector2(_faceDirection, 0));
    }

    public void Move(float value)
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
        _moveCoroutine ??= StartCoroutine(MoveRoutine());
    }
    
    private IEnumerator MoveRoutine()
    {
        while (_moveValue != 0f || !_isGrounded)
        {
            rigidbody.velocity = new Vector2(_moveValue * speed, rigidbody.velocity.y);
            yield return new WaitForFixedUpdate();
        }
        
        rigidbody.velocity = new Vector2(_moveValue, rigidbody.velocity.y);
        _moveCoroutine = null;
    }
    
    public void Jump(float value)
    {
        if (_isGrounded)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, value * 5);
        }
    }

    private IEnumerator GroundCheckRoutine()
    {
        while (true)
        {
            if (rigidbody.velocity != Vector2.zero)
            {
                _isGrounded = IsGrounded();
            }
            else
            {
                _isGrounded = true;
                _moveCoroutine ??= StartCoroutine(MoveRoutine());
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
    
    private bool IsGrounded()
    {
        if (rigidbody.velocity.y != 0f)
        {
            return false;
        }
        
        return Physics2D.BoxCast(groundLevelPoint.position, _boxCastSize, 0, Vector2.down, GroundCheckLenght, whatIsGround).collider != null;
    }
    
    public void Attack()
    {
        if (!weaponAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swing")) 
            weaponAnimator.SetTrigger("swing");
    }
}
