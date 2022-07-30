using System;
using System.Collections;
using UnityEngine;

public class RangedAttack: MonoBehaviour
{
    private const float MaxLifeTime = 2f;
    
    [SerializeField] private float speed;
    [SerializeField] private RangedAttackType attackType;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private Vector3 _direction;
    private Coroutine _disableOverTimeCoroutine;
    public Action OnAttackEnd { get; set; }
    public RangedAttackType AttackType => attackType;
    public OriginType OriginType { get; set; }

    public void Launch(Vector2 direction)
    {
        _direction = direction;
        _spriteRenderer.flipX = direction.x < 0;
        _disableOverTimeCoroutine = StartCoroutine(DisableOverTime());
    }

    void FixedUpdate()
    {
        transform.position += speed * _direction;
    }

    private void OnTriggerEnter2D(Collider2D colliders)
    {
        DisableProjectile();
    }

    private void DisableProjectile()
    {
        _disableOverTimeCoroutine = null;
        OnAttackEnd?.Invoke();
        gameObject.SetActive(false);
        OnAttackEnd = null;
    }

    IEnumerator DisableOverTime()
    {
        yield return new WaitForSeconds(MaxLifeTime);
        DisableProjectile();
    }
}