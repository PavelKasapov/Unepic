using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMove : MonoBehaviour
{
    //���������� ��� ��������� ����. �������� ���������
    public float maxSpeed = 10f;
    //���������� ��� ����������� ����������� ��������� ������/�����
    private bool isFacingRight = true;
    //������ �� ��������� ��������
    private Animator anim;
    //��������� �� �������� �� ����� ��� � ������?
    private bool isGrounded = false;
    //������ �� ��������� Transform �������
    //��� ����������� ��������������� � ������
    public Transform groundCheck;
    //������ ����������� ��������������� � ������
    private float groundRadius = 0.2f;
    //������ �� ����, �������������� �����
    public LayerMask whatIsGround;
    private Rigidbody2D rigidbody2D;

    /// <summary>
    /// ��������� �������������
    /// </summary>
	private void Start()
    {
    }

    /// <summary>
    /// ��������� �������� � ������ FixedUpdate, �. �. � ���������� Animator ���������
    /// ���������� �������� Animate Physics = true � �������� ���������������� � ��������� ������
    /// </summary>
    private void FixedUpdate()
    {
        //����������, �� ����� �� ��������
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        //������������� ��������������� ���������� � ���������
        anim.SetBool("Ground", isGrounded);
        //������������� � ��������� �������� �������� ������/�������
        anim.SetFloat("vSpeed", rigidbody2D.velocity.y);
        //���� �������� � ������ - ����� �� ������, ����� �� ����������� ��������, ��������� � �����
        if (!isGrounded)
            return;
        //���������� Input.GetAxis ��� ��� �. ����� ���������� �������� ��� � �������� �� -1 �� 1.
        //��� ����������� ���������� ������� 
        //-1 ������������ ��� ������� �� ���������� ������� ����� (��� ������� �),
        //1 ������������ ��� ������� �� ���������� ������� ������ (��� ������� D)
        float move = Input.GetAxis("Horizontal");

        //� ���������� �������� �������� �������� ��������� Speed �� �������� ��� �.
        //������� ��� ����� ������ ��������
        anim.SetFloat("Speed", Mathf.Abs(move));

        //���������� � ���������� ��������� RigidBody2D. ������ ��� �������� �� ��� �, 
        //������ �������� ��� � ���������� �� �������� ����. ��������
        rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);

        //���� ������ ������� ��� ����������� ������, � �������� ��������� �����
        if (move > 0 && !isFacingRight)
            //�������� ��������� ������
            Flip();
        //�������� ��������. �������� ��������� �����
        else if (move < 0 && isFacingRight)
            Flip();
    }

    private void Update()
    {
        //���� �������� �� ����� � ����� ������...
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //������������� � ��������� ���������� � false
            anim.SetBool("Ground", false);
            //������������ ���� �����, ����� �������� ����������
            rigidbody2D.AddForce(new Vector2(0, 600));
        }
    }

    /// <summary>
    /// ����� ��� ����� ����������� �������� ��������� � ��� ����������� ���������
    /// </summary>
    private void Flip()
    {
        //������ ����������� �������� ���������
        isFacingRight = !isFacingRight;
        //�������� ������� ���������
        Vector3 theScale = transform.localScale;
        //��������� �������� ��������� �� ��� �
        theScale.x *= -1;
        //������ ����� ������ ���������, ������ �������, �� ��������� ����������
        transform.localScale = theScale;
    }
}