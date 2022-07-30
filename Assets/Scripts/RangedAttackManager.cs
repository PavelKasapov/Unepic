using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedAttackManager : MonoBehaviour
{
    private List<RangedAttack> _rangedAttackPool = new List<RangedAttack>();
    private List<RangedAttack> _launchedRangedAttackPool = new List<RangedAttack>();
    [SerializeField] private List<RangedAttack> _attackPrefabs;

    public void Launch(RangedAttackType type, OriginType originType, Vector2 startPos, Vector2 direction)
    {
        var attack = _rangedAttackPool.FirstOrDefault(attack => attack.AttackType == type);
        if (attack == null)
        {
            attack = Create(type, startPos, direction);
            if (attack == null)
            {
                Debug.LogError($"Unable to create attack with type {type}. Please check connected prefabs.");
                return;
            }
        }
        else
        {
            _rangedAttackPool.Remove(attack);
            attack.transform.position = startPos;
            attack.gameObject.SetActive(true);
        }
        
        _launchedRangedAttackPool.Add(attack);
        attack.OnAttackEnd += () => MoveToAvailable(attack);
        attack.OriginType = originType;
        attack.Launch(direction);
    }
    
    private RangedAttack Create(RangedAttackType type, Vector2 startPos, Vector2 direction)
    {
        return Instantiate(_attackPrefabs.FirstOrDefault(attack => attack.AttackType == type), 
                startPos, 
                Quaternion.Euler(direction.x, direction.y, 0),
                transform)
            .GetComponent<Fireball>();
    }

    private void MoveToAvailable(RangedAttack attack)
    {
        _launchedRangedAttackPool.Remove(attack);
        _rangedAttackPool.Add(attack);
    }
}