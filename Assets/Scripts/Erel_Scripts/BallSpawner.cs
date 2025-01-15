using Fusion;
using UnityEngine;

public class BallSpawner : NetworkBehaviour
{
    [Networked]
    private TickTimer spawnTimer { get; set; }

    [SerializeField]
    private float timeBetweenSpawns = 1f;

    [SerializeField]
    private NetworkObject prefabToSpawn;

    public override void Spawned()
    {
        // Initialization logic if needed
    }

    public override void FixedUpdateNetwork()
    {
        if (spawnTimer.ExpiredOrNotRunning(Runner))
        {
            Runner.Spawn(
                prefabToSpawn,
                transform.position,
                transform.rotation,
                Object.InputAuthority
            );

            spawnTimer = TickTimer.CreateFromSeconds(Runner, timeBetweenSpawns);
        }
    }
}
