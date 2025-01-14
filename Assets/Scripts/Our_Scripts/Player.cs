using UnityEngine;
using Fusion;
using UnityStandardAssets.Characters.ThirdPerson;
using TMPro; // For TextMeshPro UI

public class Player : NetworkBehaviour
{
    private CharacterController _cc;

    [SerializeField] private float speed = 5f; // Movement speed
    [SerializeField] private GameObject ballPrefab; // Prefab for the ball
    private const int ZERO = 0;

    private Camera firstPersonCamera;

    // Networked score variable to sync across players
    [Networked]
    private int score { get; set; }

    [SerializeField] private TMP_Text scoreText; // UI Text to display the score

    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();

        // Assign camera for the local player
        if (HasStateAuthority)
        {
            firstPersonCamera = Camera.main;
            var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
            if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
            {
                firstPersonCameraComponent.SetTarget(this.transform);
            }
        }

        // Update the score display
        UpdateScoreUI();
    }

    private Vector3 moveDirection;

    public override void FixedUpdateNetwork()
    {
        // Handle player movement and shooting
        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.moveActionValue.magnitude > ZERO)
            {
                inputData.moveActionValue.Normalize(); // Normalize input to prevent cheating
                moveDirection = new Vector3(inputData.moveActionValue.x, ZERO, inputData.moveActionValue.y);
                Vector3 deltaPosition = speed * moveDirection * Runner.DeltaTime;
                _cc.Move(deltaPosition);
            }

            // Handle shooting logic
            if (HasStateAuthority)
            {
                if (inputData.shootActionValue)
                {
                    Debug.Log("SHOOT!");
                    var ball = Runner.Spawn(ballPrefab,
                        transform.position + moveDirection, Quaternion.LookRotation(moveDirection),
                        Object.InputAuthority);

                    // Assign the owner of the ball
                    var ballComponent = ball.GetComponent<Ball>();
                    if (ballComponent != null)
                    {
                        ballComponent.SetOwner(this); // Assign ownership
                    }
                }
            }
        }
    }

    // Public method to add score
    public void AddScore(int points)
    {
        if (HasStateAuthority) // Only the owner can update the score
        {
            score += points;
            RPC_UpdateScoreUI(score); // Call an RPC to update UI for all players
        }
    }

    // RPC to update the score display for all players
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateScoreUI(int updatedScore)
    {
        score = updatedScore; // Sync the score across all players
        UpdateScoreUI();
    }

    // Update the score UI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}"; // Display the updated score
        }
        else
        {
            Debug.LogWarning("[Player] ScoreText is not assigned!");
        }
    }
}
