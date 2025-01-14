using UnityEngine;
using Fusion;

public class BonusPickup : NetworkBehaviour
{
    [SerializeField] private int healthBonus = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player picked up the health bonus!");

            // Get the PlayerHealth component of the player who picked up the bonus
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Call the method to add health to the player
                playerHealth.AddHealth(healthBonus);
            }

            // Hide or despawn the bonus prefab after it is picked up
            if (Runner != null && Runner.IsRunning)
            {
                Runner.Despawn(Object); // Despawn the bonus prefab in a networked way
            }
            else
            {
                Debug.LogWarning("Runner is null or not running. Hiding object instead.");
                gameObject.SetActive(false); // Hide the object if not running in the network
            }
        }
    }
}
