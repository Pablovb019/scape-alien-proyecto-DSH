using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerControlAuthorative : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 3.5f;

    [SerializeField]
    private float runSpeedOffset = 2.0f;

    [SerializeField]
    private float rotationSpeed = 2f;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlaneX = new Vector2(-177.74f, -174.83f);

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlaneZ = new Vector2(46.4f, 49.3f);

    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    [SerializeField]
    public NetworkVariable<float> networkPlayerHealth = new NetworkVariable<float>(4);

    [SerializeField]
    public static Image BoostRepararAct;

    [SerializeField]
    public static Image BoostRepararDes;

    private CharacterController characterController;
    private bool groundedPlayer;
    private Vector3 playerVelocity;
    private float gravityValue = -9.8f;
    private Animator animator;

    public static bool PowerUpReparacion = false;

    // client caches animation states
    private PlayerState oldPlayerState = PlayerState.Idle;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlaneX.x, defaultInitialPositionOnPlaneX.y), 8.7f,
                   Random.Range(defaultInitialPositionOnPlaneZ.x, defaultInitialPositionOnPlaneZ.y));

            PlayerCameraFollow.Instance.FollowPlayer(transform.Find("PlayerCameraRoot"));

            BoostRepararAct = gameObject.transform.GetChild(5).transform.GetChild(2).GetComponent<Image>();
            BoostRepararDes = gameObject.transform.GetChild(5).transform.GetChild(3).GetComponent<Image>();

            BoostRepararAct.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientVisuals();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }
    }


    private void ClientVisuals()
    {
        if (oldPlayerState != networkPlayerState.Value)
        {
            oldPlayerState = networkPlayerState.Value;
            animator.SetTrigger($"{networkPlayerState.Value}");
        }
    }

    private void ClientInput()
    {
        // y axis client rotation
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        // forward & backward direction
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        // change animation states
        if (forwardInput == 0)
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
        {
            inputPosition = direction * runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Run);
        }
        else if (forwardInput < 0)
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);

        // client is responsible for moving itself
        characterController.SimpleMove(inputPosition * walkSpeed);
        transform.Rotate(inputRotation * rotationSpeed, Space.World);
    }
    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    [ServerRpc]
    public void UpdateHealthServerRpc(int takeAwayPoint, ulong clientId)
    {
        var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.GetComponent<PlayerControlAuthorative>();

        if (clientWithDamaged != null && clientWithDamaged.networkPlayerHealth.Value > 0)
        {
            clientWithDamaged.networkPlayerHealth.Value -= takeAwayPoint;
        }

        if (clientWithDamaged != null && clientWithDamaged.networkPlayerHealth.Value <= 0)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            SceneManager.LoadScene("HasPerdido");
        }

        // execute method on a client getting punch
        clientWithDamaged.NotifyHealthChangedClientRpc(clientWithDamaged.networkPlayerHealth.Value, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    public void NotifyHealthChangedClientRpc(float life, ClientRpcParams clientRpcParams = default)
    {
        if (life > 1)
        {
            Logger.Instance.LogWarning($"Te quedan {life} vidas");
        }

        if (life == 1)
        {
            Logger.Instance.LogWarning($"Te queda {life} vida");
        }

        if (life <= 0)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("HasPerdido");
        }
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }
}