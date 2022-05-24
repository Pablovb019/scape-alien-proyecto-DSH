using DilmerGames.Core.Singletons;
using Unity.Netcode;

public class PlayerManager: Singleton<PlayerManager>
{
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Logger.Instance.LogInfo($"{id} se ha conectado ...");
                playersInGame.Value++;
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Logger.Instance.LogInfo($"{id} se ha desconectado ...");
                playersInGame.Value--;
            }
        };


    }
}