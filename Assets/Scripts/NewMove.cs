using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NewMove : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public float jumpForce = 10f;

    float velo;
    public bool faceToRight = true;
    private Rigidbody2D _rb;

    private bool isGrounded = false;
    public Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");

        if (isGrounded && Input.GetAxis("Jump") > 0)
        {
            Debug.Log("jump");
            _rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }

        if (move > 0 && !faceToRight)
            Flip();
        else if (move < 0 && faceToRight)
            Flip();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        if (!isGrounded)
            return;

        _rb.velocity = new Vector2(move * speed, _rb.velocity.y);
    }

    void Flip()
    {
        faceToRight = !faceToRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
