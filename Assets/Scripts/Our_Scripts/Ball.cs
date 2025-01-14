using Fusion;
using UnityEngine;

/**
 * This component represents a ball moving at a constant speed.
 */
public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer lifeTimer { get; set; }

    [SerializeField] private float lifeTime = 5.0f; // Time before the ball despawns
    [SerializeField] private float speed = 5.0f; // Speed of the ball
    [SerializeField] private int damagePerHit = 1; // Damage dealt on hit
    [SerializeField] private int addedScore = 1; // Damage dealt on hit



    private Player owner; // Reference to the player who shot the ball

    // Assign the owner of the ball
    public void SetOwner(Player player)
    {
        owner = player;
    }

    public override void Spawned()
    {
        // Initialize the ball's lifetime
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        // Check if the ball's lifetime has expired
        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            // Move the ball forward
            transform.position += speed * transform.forward * Runner.DeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the ball hits an object with a Health component
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            // Check if the player being hit is shielded
            Player hitPlayer = other.GetComponent<Player>();
            if (hitPlayer != null)
            {
                var playerController = hitPlayer.GetComponent<PlayerController>(); // PlayerController manages the shield
                if (playerController != null && playerController.IsShielded)
                {
                    Debug.Log("Player is shielded! No damage taken, and no points awarded.");
                    Runner.Despawn(Object); // Destroy the ball
                    return; // Exit without awarding points or applying damage
                }
            }

            // Apply damage if the player is not shielded
            health.DealDamageRpc(damagePerHit);

            // Award points to the owner if the player is not shielded
            if (owner != null && hitPlayer != null && hitPlayer != owner)
            {
                owner.AddScore(addedScore); // Add 1 point to the owner for a successful hit
                Debug.Log($"Player {owner.name} scored a point!");
            }

            Runner.Despawn(Object); // Despawn the ball
        }
    }
}
