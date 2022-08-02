using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private CharacterMovementSystem playerMovementSystem;
    
    private PlayerControls _controls;
    
    private void Awake()
    {
        _controls = new PlayerControls();
        
        _controls.CommonMovement.HorisontalMovement.performed += ctx => playerMovementSystem.Move(ctx.ReadValue<float>());
        _controls.CommonMovement.HorisontalMovement.canceled += ctx => playerMovementSystem.Move(ctx.ReadValue<float>());
        _controls.CommonMovement.Jump.performed += ctx => playerMovementSystem.Jump(ctx.ReadValue<float>());
        _controls.CommonMovement.Attack.performed += ctx => playerMovementSystem.Attack();
        _controls.CommonMovement.Shoot.performed += ctx => playerMovementSystem.ShootAnimation();
    }
    
    private void OnEnable()
    {
        _controls.Enable();
    }
    
    private void OnDisable()
    {
        _controls.Disable();
    }
}
