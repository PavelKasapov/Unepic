using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    private PlayerControls _controls;
    // Start is called before the first frame update
    void Start()
    {
        _controls = new PlayerControls();
        _controls.CommonMovement.Jump.performed += Jump;
        _controls.CommonMovement.HorisontalMovement.performed += (ctx => Move(ctx.ReadValue<float>()));
    }

    // Update is called once per frame
    void Move(float value)
    {
        Debug.Log(value);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx);
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 1f);
    }
}
