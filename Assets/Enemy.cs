using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D booms_enemy)
    {
        if (booms_enemy.gameObject.tag == "Player")
        {
           

        }

    }
}
