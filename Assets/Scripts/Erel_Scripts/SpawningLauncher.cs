// From the Fusion 2 Tutorial: https://doc.photonengine.com/fusion/current/tutorials/host-mode-basics/2-setting-up-a-scene#launching-fusion
using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// This class launches Fusion NetworkRunner, and also spawns a new avatar whenever a player joins.
public class SpawningLauncher : EmptyLauncher
{
    [SerializeField]
    private NetworkPrefabRef _playerPrefab;
    [SerializeField]
    private Transform[] spawnPoints;

    private const int ZERO = 0;

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined");

        bool isAllowedToSpawn = runner.GameMode == GameMode.Shared
            ? player == runner.LocalPlayer // In Shared mode, the local player is allowed to spawn.
            : runner.IsServer; // In Host or Server mode, only the server is allowed to spawn.

        if (isAllowedToSpawn)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = spawnPoints[player.AsIndex % spawnPoints.Length].position;

            NetworkObject networkPlayerObject = runner.Spawn(
                _playerPrefab,
                spawnPosition,
                Quaternion.identity,
                player // Input authority
            );

            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left");

        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    [SerializeField]
    private InputAction moveAction = new InputAction(type: InputActionType.Button);
    [SerializeField]
    private InputAction shootAction = new InputAction(type: InputActionType.Button);
    [SerializeField]
    private InputAction colorAction = new InputAction(type: InputActionType.Button);

    private void OnEnable()
    {
        moveAction.Enable();
        shootAction.Enable();
        colorAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        shootAction.Disable();
        colorAction.Disable();
    }

    private void OnValidate()
    {
        // Provide default bindings for the input actions. Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (moveAction.bindings.Count == ZERO)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
        }

        if (shootAction.bindings.Count == ZERO)
        {
            shootAction.AddBinding("<Keyboard>/space");
        }

        if (colorAction.bindings.Count == ZERO)
        {
            colorAction.AddBinding("<Keyboard>/C");
        }
    }

    private NetworkInputData inputData = new NetworkInputData();

    private void Update()
    {
        if (shootAction.WasPressedThisFrame())
        {
            inputData.shootActionValue = true;
        }

        if (colorAction.WasPressedThisFrame())
        {
            inputData.colorActionValue = true;
        }
    }

    public override void OnInput(NetworkRunner runner, NetworkInput input)
    {
        inputData.moveActionValue = moveAction.ReadValue<Vector2>();
        input.Set(inputData); // Pass inputData by value

        inputData.shootActionValue = false; // Clear shoot flag
        inputData.colorActionValue = false; // Clear color flag
    }
}
