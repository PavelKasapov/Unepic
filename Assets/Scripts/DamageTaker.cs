using System;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private LayerMask damageSources;
   
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (damageSources == (damageSources | (1 << collision.gameObject.layer)))
        {
            characterData.Life.Value--;

            if (characterData.Life.Value <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
