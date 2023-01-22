using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EnvironmentTouchChecker : MonoBehaviour
{
	public BoolReactiveProperty IsTouchingGround { get; } = new(false);
	public IntReactiveProperty TouchingWallsDirection { get; } = new(0);
    
	private Coroutine _contactsCheckCoroutine;
	private readonly List<ContactPoint2D> _contactPoints = new();
	private ContactFilter2D _contactFilter;
	[SerializeField] private new BoxCollider2D collider;

	private void Awake()
	{
		_contactFilter.SetLayerMask(LayerMask.NameToLayer("Ground"));
	}

	private void OnEnable()
	{
		_contactsCheckCoroutine ??= StartCoroutine(ContactsCheckRoutine());
	}

	private void OnDisable()
	{
		if (_contactsCheckCoroutine != null)
		{
			StopCoroutine(_contactsCheckCoroutine);
			_contactsCheckCoroutine = null;
		}
	}
    

	private IEnumerator ContactsCheckRoutine()
	{
		while (true)
		{
			if (collider.GetContacts(/*_contactFilter,*/ _contactPoints) == 0)
			{
				TouchingWallsDirection.Value = 0;
				IsTouchingGround.Value = false;
			}
			else
			{
				var touchingWallNormal = _contactPoints.FirstOrDefault(point => point.normal.y == 0 && point.normal.x != 0).normal;
				TouchingWallsDirection.Value = touchingWallNormal != default 
					? -Math.Sign(touchingWallNormal.x) 
					: 0;

				IsTouchingGround.Value = _contactPoints.Exists(point => point.normal.y > 0);
			}
            
			yield return new WaitForFixedUpdate();
		}
	}
}