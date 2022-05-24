using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class KillerControlAuthorative : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 4.5f;

    [SerializeField]
    private float runSpeedOffset = 2.25f;

    [SerializeField]
    private float rotationSpeed = 2f;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlaneX = new Vector2(-12.5f, -8.5f);

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlaneZ = new Vector2(-14.0f, -10.0f);

    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    [SerializeField]
    private NetworkVariable<float> networkPlayerPunchBlend = new NetworkVariable<float>();

    [SerializeField]
    public static int NumGeneradoresHechos = 0;

    [SerializeField]
    private GameObject Punto1;

    [SerializeField]
    private GameObject Punto2;

    [SerializeField]
    private GameObject Punto3;

    [SerializeField]
    private float minPunchDistance = 6f;

    [SerializeField]
    public static Image BoostVelocidadAct;

    [SerializeField]
    public static Image BoostVelocidadDes;

    public int kill = 0;
    public bool hayJugadores = false;
    private int MaxNumberClients = 0;

    private CharacterController characterController;
    private bool groundedPlayer;
    private Vector3 playerVelocity;
    private float gravityValue = -9.8f;
    private bool Golpe = false;
    private Animator animator;
    private int a = 0;

    private Transform PosBoostVelocidad;
    private static bool PowerUpVelocidad = true;
    private bool TiempoEsperaSpawn = false;

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
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlaneX.x, defaultInitialPositionOnPlaneX.y), 0,
                   Random.Range(defaultInitialPositionOnPlaneZ.x, defaultInitialPositionOnPlaneZ.y));

            PlayerCameraFollow.Instance.FollowPlayer(transform.Find("PlayerCameraRoot"));

            BoostVelocidadAct = gameObject.transform.GetChild(2).transform.GetChild(2).GetComponent<Image>();
            BoostVelocidadDes = gameObject.transform.GetChild(2).transform.GetChild(3).GetComponent<Image>();

            BoostVelocidadDes.gameObject.SetActive(false);
            BoostVelocidadAct.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!TiempoEsperaSpawn)
        {
            StartCoroutine(TiempoSpawn());
        }
        var actualClients = GameObject.FindGameObjectsWithTag("Player").Length;
        
        if (actualClients > 0 && !hayJugadores)
        {
            hayJugadores = true;
        }

        if (actualClients > MaxNumberClients)
        {
            MaxNumberClients = actualClients;
        }

        if (actualClients == 0 && kill == MaxNumberClients && hayJugadores)
        {
            SceneManager.LoadScene("HasGanado");
        }

        if (actualClients == 0 && kill < MaxNumberClients && hayJugadores)
        {
            SceneManager.LoadScene("HasPerdido");
        }

        if (Input.GetKeyDown(KeyCode.V) && PowerUpVelocidad)
        {
            StartCoroutine(BoostVelocidad());
        }
        if (IsClient && IsOwner)
        {
            groundedPlayer = characterController.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = -2f;
            }

            ClientInput();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            StartCoroutine(Attack());
        }

        ClientVisuals();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private IEnumerator TiempoSpawn()
    {
        a = 1;
        yield return new WaitForSeconds(20f);
        TiempoEsperaSpawn = true;
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }
    }

    private void FixedUpdate()
    {
        if (IsClient && IsOwner)
        {
            if (networkPlayerState.Value == PlayerState.Attack && ActivePunchActionKey())
            {
                CheckPunch(Punto1.transform, Vector3.forward);
                CheckPunch(Punto2.transform, Vector3.forward);
                CheckPunch(Punto3.transform, Vector3.forward);
            }
        }
    }

    private void CheckPunch(Transform hand, Vector3 aimDirection)
    {
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Player");

        if (Physics.Raycast(hand.position, hand.transform.TransformDirection(aimDirection), out hit, minPunchDistance, layerMask))
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.yellow);

            var playerHit = hit.transform.GetComponent<NetworkObject>();
            if (playerHit != null && !Golpe && playerHit.tag == "Player")
            {
                UpdateHealthServerRpc(1, playerHit.OwnerClientId);
                StartCoroutine(Cooldown());
            }
        }
        else
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.red);
        }
    }

    private IEnumerator BoostVelocidad()
    {
        BoostVelocidadAct.gameObject.SetActive(false);
        BoostVelocidadDes.gameObject.SetActive(true);
        BoostVelocidadDes.transform.position = BoostVelocidadAct.transform.position;
        walkSpeed = 6.75f;

        yield return new WaitForSeconds(10);
        PowerUpVelocidad = false;
        walkSpeed = 4.5f;
        StartCoroutine(CooldownBoost());
    }

    private IEnumerator CooldownBoost()
    {
        yield return new WaitForSeconds(30);
        PowerUpVelocidad = true;
        BoostVelocidadAct.gameObject.SetActive(true);
        BoostVelocidadDes.gameObject.SetActive(false);
    }

    private IEnumerator Cooldown()
    {
        Golpe = true;
        yield return new WaitForSeconds(1.66f);
        Golpe = false;
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

    private IEnumerator Attack()
    {
        UpdatePlayerStateServerRpc(PlayerState.Attack);
        yield return new WaitForSeconds(0.5f);
        UpdatePlayerStateServerRpc(oldPlayerState);
    }

    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private static bool ActivePunchActionKey()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }

    [ServerRpc]
    public void UpdateHealthServerRpc(int takeAwayPoint, ulong clientId)
    {
        var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.GetComponent<PlayerControlAuthorative>();

        if (clientWithDamaged != null && clientWithDamaged.networkPlayerHealth.Value > 0)
        {
            clientWithDamaged.networkPlayerHealth.Value -= takeAwayPoint;
            if (clientWithDamaged.networkPlayerHealth.Value <= 0){
                kill++;
            }
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

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }
}