using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

// From Fusion tutorial: https://doc.photonengine.com/fusion/current/tutorials/shared-mode-basics/5-remote-procedure-calls
public class RaycastAttack : NetworkBehaviour
{
    [SerializeField]
    private int Damage;

    [SerializeField]
    private InputAction attack;

    [SerializeField]
    private InputAction attackLocation;

    [SerializeField]
    private float shootDistance = 5f;

    private const int ZERO = 0;
    private const float ONE = 1f;

    private bool _attackPressed;

    private void OnEnable()
    {
        attack.Enable();
        attackLocation.Enable();
    }

    private void OnDisable()
    {
        attack.Disable();
        attackLocation.Disable();
    }

    private void OnValidate()
    {
        // Provide default bindings for the input actions. Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (attack == null)
        {
            attack = new InputAction(type: InputActionType.Button);
        }

        if (attack.bindings.Count == ZERO)
        {
            attack.AddBinding("<Mouse>/leftButton");
        }

        if (attackLocation == null)
        {
            attackLocation = new InputAction(type: InputActionType.Value, expectedControlType: "Vector2");
        }

        if (attackLocation.bindings.Count == ZERO)
        {
            attackLocation.AddBinding("<Mouse>/position");
        }
    }

    private void Update()
    {
        // We have to read the button status in Update, because FixedNetworkUpdate might miss it.
        if (!HasStateAuthority)
        {
            return;
        }

        if (attack.WasPerformedThisFrame())
        {
            _attackPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (_attackPressed)
        {
            Vector2 attackLocationInScreenCoordinates = attackLocation.ReadValue<Vector2>();

            var camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(attackLocationInScreenCoordinates);
            ray.origin += camera.transform.forward;

            Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, duration: ONE);

            if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction * shootDistance, out var hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Debug.Log($"Raycast hit: name={hitObject.name} tag={hitObject.tag} collider={hit.collider}");

                if (hitObject.TryGetComponent<Health>(out var health))
                {
                    Debug.Log("Dealing damage");
                    health.DealDamageRpc(Damage);
                }
            }

            _attackPressed = false;
        }
    }
}
