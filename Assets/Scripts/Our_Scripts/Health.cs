using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private NumberField HealthDisplay;

    [Networked]
    public int NetworkedHealth { get; set; } = 100;

    private const int ZERO = 0;

    // Migration from Fusion 1: https://doc.photonengine.com/fusion/current/getting-started/migration/coming-from-fusion-v1
    private ChangeDetector _changes;

    private PlayerController playerController;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        HealthDisplay.SetNumber(NetworkedHealth);

        // Linked to PlayerController to check the shield state
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController is missing on the object!");
        }
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(NetworkedHealth):
                    HealthDisplay.SetNumber(NetworkedHealth);
                    break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(int damage)
    {
        // Check if the player is shielded
        if (playerController != null && playerController.IsShielded)
        {
            Debug.Log("Player is shielded! No damage taken.");
            return; // If player is shielded, no damage
        }

        // If the player is not shielded, apply damage
        Debug.Log($"Received DealDamageRpc on StateAuthority, modifying Networked variable. Damage: {damage}");
        NetworkedHealth -= damage;

        // Check if the player is dead
        if (NetworkedHealth <= ZERO)
        {
            Die();
        }
    }

    // New method to add health (e.g., from picking up the bonus)
    public void AddHealth(int healthAmount)
    {
        NetworkedHealth += healthAmount; // Add health
        Debug.Log($"Added {healthAmount} health. Current Health: {NetworkedHealth}");
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        Destroy(gameObject);
    }
}
