using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerControl : NetworkBehaviour
{

    [SerializeField]
    private float walkSpeed = 3.5f;

    [SerializeField]
    private float runSpeedOffset = 2.0f;

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlaneX = new Vector2(8, 18);
    private Vector2 defaultInitialPositionOnPlaneZ = new Vector2(-18, 0);

    [SerializeField]
    private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();

    [SerializeField]
    private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();

    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    public Camera camara;
    private CharacterController CharacterController;
    private Animator animator;

    // client cashing
    private Vector3 oldInputPosition = Vector3.zero;
    private Vector3 oldInputRotation = Vector3.zero;
    private PlayerState oldPlayerState = PlayerState.Idle;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {

        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlaneX.x, defaultInitialPositionOnPlaneX.y), 0, Random.Range(defaultInitialPositionOnPlaneZ.x, defaultInitialPositionOnPlaneZ.y));
            PlayerCameraFollow.Instance.FollowPlayer(transform.Find("PlayerCameraRoot"));
        }
    }

    private void Update()
    {
        if(IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientInput()
    {
        Vector3 InputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float ForwardInput = Input.GetAxis("Vertical");
        Vector3 InputPosition = direction * ForwardInput;

        if (ForwardInput == 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }

        else if (!ActiveRunningActionKey() && ForwardInput > 0 && ForwardInput <= 1)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }

        else if (ActiveRunningActionKey() && ForwardInput > 0 && ForwardInput <= 1)
        {
            InputPosition = direction * runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Run);
        }

        else if(ForwardInput < 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);
        }

        // let server know about position and rotation client changes
        if(oldInputPosition != InputPosition || oldInputRotation != InputRotation || oldPlayerState != networkPlayerState.Value)
        {
            oldInputPosition = InputPosition;
            UpdateClientPositionAndRotationServerRpc(InputPosition * walkSpeed, InputRotation * rotationSpeed);
        }
    }

    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void ClientMoveAndRotate()
    {
        if(networkPositionDirection.Value != Vector3.zero)
        {
            CharacterController.SimpleMove(networkPositionDirection.Value);
        }

        if(networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value);
        }
    }

    private void ClientVisuals()
    {
        if(oldPlayerState != networkPlayerState.Value)
        {
            oldPlayerState = networkPlayerState.Value;
            animator.SetTrigger($"{networkPlayerState.Value}");
        }
    }

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPositionDirection, Vector3 newRotationDirection)
    {
        networkPositionDirection.Value = newPositionDirection;
        networkRotationDirection.Value = newRotationDirection;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        networkPlayerState.Value = newState;
    }
}
