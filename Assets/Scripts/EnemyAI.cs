using UnityEngine;
using Zenject;

public class EnemyAI : MonoBehaviour
{
    [Inject(Id = "Player")] private Transform _playerTransform;
    
    [SerializeField] private CharacterMovementSystem playerMovementSystem;
    [SerializeField] private Rigidbody2D _rigidbody;

    private float moveDirection;
    
    private void Update()
    {
        var playerDirection =  Mathf.Sign(_playerTransform.position.x - transform.position.x);
        if (moveDirection != playerDirection)
        {
            playerMovementSystem.Move(playerDirection);
            moveDirection = playerDirection;
        }
        
        if (_rigidbody.velocity.x == 0f && _playerTransform.position.y > transform.position.y)
        {
            playerMovementSystem.Move(playerDirection);
            playerMovementSystem.Jump(1f);
        }
    }
}
