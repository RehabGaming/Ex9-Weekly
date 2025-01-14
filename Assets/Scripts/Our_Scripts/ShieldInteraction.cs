using Fusion;
using UnityEngine;

public class ShieldInteraction : NetworkBehaviour
{
    [SerializeField] private float shieldDuration = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player picked up the shield!");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null)
            {
                Debug.LogError("PlayerController is missing on the player!");
                return;
            }

            // activate the shield for the player
            player.PickUpShield(shieldDuration);

            // destroy the shield after picking him up
            if (Runner != null && Runner.IsRunning)
            {
                Runner.Despawn(Object); // destroy the shield obj to all the other players
            }
            else
            {
                Debug.LogWarning("Runner is null or not running. Hiding object instead.");
                gameObject.SetActive(false); // to hide it only
            }
        }
    }
}
