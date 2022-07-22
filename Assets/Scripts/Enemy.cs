using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [Inject(Id = "Player")] private Transform playerTransform;
    [SerializeField] private Rigidbody2D selfRb;
    [SerializeField] private float speed;

    [SerializeField] private int life = 1;

    private int Life
    {
        get 
        { 
            return life; 
        }
        set 
        { 
            life = value;
            if (value <= 0)
                Destroy(gameObject);
        } 
    }


    void Start()
    {
        
    }

    void Update()
    {
        var direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
        selfRb.velocity = new Vector2(direction * speed, selfRb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("!!h1t hero");


        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            Debug.Log("!!!!take shit");

            Life--;

        }
    }
}
