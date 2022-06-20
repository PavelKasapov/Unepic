using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    
    private PlayerControls _controls;
    
    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }
    
    private void OnDisable()
    {
        _controls.Disable();
    }

    private void FixedUpdate()
    {
        Jump(_controls.CommonMovement.Jump.ReadValue<float>());
        Move(_controls.CommonMovement.HorisontalMovement.ReadValue<float>());
    }

    // Update is called once per frame
    void Move(float value)
    {
        var localScale = transform.localScale;
        localScale = new Vector3(Mathf.Sign(value), localScale.y, localScale.z);
        _rigidbody.velocity = new Vector2(value * 2, _rigidbody.velocity.y);
    }

    void Jump(float value)
    {
        if (value > 0)
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, value * 5);
    }
}
