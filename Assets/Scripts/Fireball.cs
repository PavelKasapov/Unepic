using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float speed;
    private float _direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Launch(float direction)
    {
        _direction = direction;
        var localScale = transform.localScale;
        localScale.x = direction;
        transform.localScale = localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.right * speed * _direction;
    }

    private void OnTriggerEnter2D(Collider2D colliders)
    {
        Destroy(gameObject);
    }
}
