using Mirror;
using UnityEngine;

public class GameStarter : NetworkBehaviour
{
    [SyncVar]
    private bool _gameStarted = false;

    private void Start()
    {
        if (isServer)
        {
            // Хост спавнится сразу после создания лобби
            SpawnPlayer(NetworkServer.localConnection);
        }
    }

    private void OnPlayerConnected(NetworkConnection conn)
    {
        if (isServer)
        {
            // Спавн второго игрока после подключения
            SpawnPlayer(conn);
        }
    }

    private void SpawnPlayer(NetworkConnection conn)
    {
        // Спавн игрока
        var playerPrefab = NetworkManager.singleton.playerPrefab;
        var spawnPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(player);
    }
}
