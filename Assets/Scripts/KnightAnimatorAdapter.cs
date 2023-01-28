using System;
using System.Collections;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

public class KnightAnimatorAdapter : MonoBehaviour, IAnimationAdapter
{
	[SerializeField] private float speed = 2;
	[SerializeField] private EnvironmentTouchChecker environmentTouchChecker;
	[SerializeField] private Animator playerAnimator;
	[SerializeField] private new Rigidbody2D rigidbody;
	[SerializeField] private GameObject slideDust;

	private static readonly int IsGrounded = Animator.StringToHash("Grounded");
	private static readonly int CanWallSlide = Animator.StringToHash("WallSlide");
	//private static readonly int MoveSpeedMultiplier = Animator.StringToHash("MoveSpeedMultiplier");
	private static readonly int IsMoving = Animator.StringToHash("Move");
	private static readonly int VerticalSpeed = Animator.StringToHash("AirSpeedY");
	private static readonly int Jump = Animator.StringToHash("Jump");
	private static readonly int Combo = Animator.StringToHash("Combo");
	private static readonly int ComboCounter = Animator.StringToHash("ComboCounter");
	private static readonly int Attack = Animator.StringToHash("Attack");
	private static readonly int InterruptionForAttack = Animator.StringToHash("InterruptionForAttack");
	private static TimeSpan OneSecond = TimeSpan.FromSeconds(1);
	
	private Coroutine _velocityApplyCoroutine;
	private Coroutine _attackComboCoroutine;
	private DateTime _comboExpireTime;
	private int _moveValueSign;
	private int _currentAttackCounter = 0;
	private bool _isAttackAnimation;
	
	private bool IsAttackAnimation
	{
		get => _isAttackAnimation;
		set
		{
			Debug.Log($"!! _isAttackAnimation {_isAttackAnimation} => {value}");
			_isAttackAnimation = value;
			Debug.Log($"!! {_isAttackAnimation}");
		}
	}

	private int CurrentAttackCounter
	{
		get => _currentAttackCounter;
		set
		{
			if (_currentAttackCounter != value)
			{
				_currentAttackCounter = value;
				playerAnimator.SetInteger(ComboCounter, value);
			}
		}
	}

	private void Awake()
	{
		environmentTouchChecker.IsTouchingGround.Subscribe(value => playerAnimator.SetBool(IsGrounded, value));
		/*environmentTouchChecker.TouchingWallsDirection.Where(value => environmentTouchChecker.IsTouchingGround.Value && value == Math.Sign(rigidbody.velocity.x))
			.Subscribe(value => SetMovingState(0));*/
	}
	
	private void OnEnable()
	{
		_velocityApplyCoroutine ??= StartCoroutine(VelocityApply());
	}
    
	private void OnDisable()
	{
		if (_velocityApplyCoroutine != null)
		{
			StopCoroutine(_velocityApplyCoroutine);
			_velocityApplyCoroutine = null;
		}
	}
	
	private IEnumerator VelocityApply()
	{
		while (true)
		{
			var velocity = rigidbody.velocity;
			//playerAnimator.SetFloat(MoveSpeedMultiplier, velocity.x/speed);
			playerAnimator.SetFloat(VerticalSpeed, rigidbody.velocity.y);
            
			yield return new WaitForFixedUpdate();
		}
	}

	public void SetMovingState(float moveValue)
	{
		_moveValueSign = moveValue != 0 ? Math.Sign(moveValue) : 0;
		playerAnimator.SetBool(IsMoving, moveValue != 0 /*&& environmentTouchChecker.TouchingWallsDirection.Value != _moveValueSign*/);
	}
	
	public void TriggerJumpingState()
	{
		playerAnimator.SetTrigger(Jump);
	}
	
	public void SetWallSlideState(bool isWallSliding)
	{
		playerAnimator.SetBool(CanWallSlide, isWallSliding);
	}

	public void TriggerAttackState(int faceDirection)
	{
		if (IsAttackAnimation)
		{
			return;
		}
		IsAttackAnimation = true;
		
		// Temporary. Not supposed to be here. Will proceed in CharacterMovementSystem soon.
		if (environmentTouchChecker.IsTouchingGround.Value)
		{
			rigidbody.velocity = new Vector2(5 * faceDirection, 0);
		}//
		
		_comboExpireTime = DateTime.Now.Add(OneSecond);
		playerAnimator.SetTrigger(Attack);
		playerAnimator.SetTrigger(InterruptionForAttack);
		
		if (CurrentAttackCounter == 0 && _attackComboCoroutine == null)
		{
			_attackComboCoroutine = StartCoroutine(AttackComboTimer());
			
		}

		CurrentAttackCounter++;

		if (CurrentAttackCounter > 3)
		{
			CurrentAttackCounter = 1;
		}
	}
	
	private IEnumerator AttackComboTimer()
	{
		yield return new WaitForFixedUpdate();
		
		while (DateTime.Now < _comboExpireTime)
		{
			yield return new WaitForFixedUpdate();
		}
		
		CurrentAttackCounter = 0;
		_attackComboCoroutine = null;
	}
	
	private void AE_SlideDust()
	{
		Vector3 spawnPosition;

		/*if (_moveValueSign == 1)
			spawnPosition = m_wallSensorR2.transform.position;
		else
			spawnPosition = m_wallSensorL2.transform.position;*/

		if (slideDust != null)
		{
			// Set correct arrow spawn position
			GameObject dust = Instantiate(slideDust, transform.position, gameObject.transform.localRotation) as GameObject;
			// Turn arrow in correct direction
			dust.transform.localScale = new Vector3(_moveValueSign, 1, 1);
		}
	}

	[UsedImplicitly(ImplicitUseKindFlags.Access)]
	private void OnAttackEnd()
	{
		IsAttackAnimation = false;
	}
}