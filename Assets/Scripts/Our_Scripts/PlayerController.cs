using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsShielded { get; private set; } = false; // is player shielded
    private float shieldEndTime = 0f;

    private void Update()
    {
        //check if the shield time is expired
        if (IsShielded && Time.time >= shieldEndTime)
        {
            DeactivateShield();
        }
    }

    public void PickUpShield(float duration)
    {
        //if he already shielded cant be shielded again
        if (IsShielded)
        {
            Debug.Log("Shield is already active!");
            return;
        }

        ActivateShield(duration);
    }

    private void ActivateShield(float duration)
    {
        //activated shield
        IsShielded = true;
        shieldEndTime = Time.time + duration;
        Debug.Log("Shield activated! Duration: " + duration + " seconds");
    }

    private void DeactivateShield()
    {
        IsShielded = false;
        Debug.Log("Shield deactivated!");
    }
}







