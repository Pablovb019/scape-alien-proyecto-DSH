using DilmerGames.Core.Singletons;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Tayx.Graphy;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    private GameObject PrefabKiller;

    private bool hasServerStarted;

    private void Awake()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        playersInGameText.text = $"Jugadores en Línea: {PlayerManager.Instance.PlayersInGame}";
    }

    void Start()
    {        
        // START SERVER
        startServerButton?.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
                Logger.Instance.LogInfo("Servidor iniciado...");
            else
                Logger.Instance.LogInfo("Error al iniciar el servidor...");
        });

        // START HOST
        startHostButton?.onClick.AddListener(async () =>
        {
            // this allows the UnityMultiplayer and UnityMultiplayerRelay scene to work with and without
            // relay features - if the Unity transport is found and is relay protocol then we redirect all the 
            // traffic through the relay, else it just uses a LAN type (UNET) communication.
            if (RelayManager.Instance.IsRelayEnabled) 
                await RelayManager.Instance.SetupRelay();

            if (NetworkManager.Singleton.StartHost()){
                startHostButton.gameObject.SetActive(false);
                startClientButton.gameObject.SetActive(false);
                startServerButton.gameObject.SetActive(false);
                joinCodeInput.gameObject.SetActive(false);
                playersInGameText.transform.position = startClientButton.transform.position;
                Logger.Instance.LogInfo("Host iniciado...");


                GameObject go = Instantiate(PrefabKiller, new Vector3(4,0,0), Quaternion.identity);
                var check_spawn  = go.GetComponent<NetworkObject>();

                if (!check_spawn.IsSpawned)
                {
                    check_spawn.Spawn();
                }

                if(GameObject.Find("PlayerAuthorative(Clone)"))
                {
                    Destroy(GameObject.Find("PlayerAuthorative(Clone)"));
                }
            }
                
            else
                Logger.Instance.LogInfo("Error al iniciar el host...");
        });

        // START CLIENT
        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);

            if(NetworkManager.Singleton.StartClient())
            {
                startHostButton.gameObject.SetActive(false);
                startClientButton.gameObject.SetActive(false);
                startServerButton.gameObject.SetActive(false);
                joinCodeInput.gameObject.SetActive(false);
                playersInGameText.gameObject.SetActive(false);
                Logger.Instance.LogInfo("Cliente iniciado...");
            }
                
            else
                Logger.Instance.LogInfo("Error al iniciar el cliente...");
        });

        // STATUS TYPE CALLBACKS
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Logger.Instance.LogInfo($"Jugador con id {id} conectado...");
        };

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
    }
}