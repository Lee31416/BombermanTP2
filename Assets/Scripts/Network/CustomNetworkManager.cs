using System;
using System.Collections.Generic;
using Gameplay;
using Map;
using Mirror;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private InputField _inputAddress;
    [SerializeField] private InputField _inputUsername;
    [FormerlySerializedAs("_hud")] [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _connectionMenu;
    [SerializeField] private GameObject _whitePlayerPrefab;
    [SerializeField] private TextMeshProUGUI _connectionMenuTitle;

    private int _playerCount;
    private String _playerUsername;
    private GameObject _grid;
    private GridScript _gridScript;
    private Tilemap _bombReferenceTilemap;
    
    public Dictionary<int, PlayerControl> players = new Dictionary<int, PlayerControl>();

    public void StartHostOnClick()
    {
        StartHost();
        _playerUsername = _inputUsername.text == "" ? "Host" : _inputUsername.text;
        _mainMenu.SetActive(false);
    }

    public void ConnectToIP()
    {
        networkAddress = _inputAddress.text == "" ? "localhost" : _inputAddress.text;
        _playerUsername = _inputUsername.text == "" ? "Bomberman" : _inputUsername.text;
        StartClient();
        _mainMenu.SetActive(false);
        _connectionMenu.SetActive(true);
        _connectionMenuTitle.text = "Connecting to: " + networkAddress;
    }

    public void CancelConnection()
    {
        StopClient();
        _mainMenu.SetActive(true);
        _connectionMenu.SetActive(false); 
    }
    
    public void UpdateStatus(TextMeshProUGUI textObj)
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            textObj.text = ($"<b>Host</b>: running via {Transport.activeTransport}");
        }
        // server only
        else if (NetworkServer.active)
        {
            textObj.text = ($"<b>Server</b>: running via {Transport.activeTransport}");
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            textObj.text = ($"<b>Client</b>: connected to {networkAddress} via {Transport.activeTransport}");
        }
    }
    
    
    public void HandleStopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            StopServer();
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        var startPosition = GetStartPosition();
        var player = Instantiate(_whitePlayerPrefab, startPosition);
        player.name = $"{_playerUsername} [connId={conn.connectionId}]" ;
        NetworkServer.AddPlayerForConnection(conn, player);
        var playerScript = player.GetComponent<PlayerControl>();
        playerScript.grid = _grid;
        _playerCount++;
        players.Add(_playerCount, playerScript);
        //print("SERVER: player grid instance" + playerScript.grid);
    }

    private void InitGrid()
    {
        _grid = GameObject.Find("Grid"); 
        _gridScript = _grid.GetComponent<GridScript>();
        _gridScript.InitializeSauce();
        _grid.GetComponentInChildren<MapGenerator>().GenerateMap(_gridScript);
        _grid.GetComponentInChildren<WallGenerator>().GenerateWalls(_gridScript);
        _grid.GetComponentInChildren<BreakableBlockGenerator>().GenerateBreakableBlocks(_gridScript);
        _grid.GetComponentInChildren<ItemGeneratorScript>().GenerateItemAtRandom(_gridScript);
        _bombReferenceTilemap = _grid.GetComponentInChildren<Tilemap>();
    }
    
    public void OnLayBombCommand(GameObject player)
    {
        //print("Hello this is the server : A client ask to lay a bomb");
        print(_bombReferenceTilemap);
        var bombPrefab = spawnPrefabs[0];
        var pos = player.GetComponent<Rigidbody2D>().position;
        var cell = _bombReferenceTilemap.WorldToCell(pos);
        var cellCenterPos = _bombReferenceTilemap.GetCellCenterWorld(cell);
        var bomb = Instantiate(bombPrefab, cellCenterPos, Quaternion.identity);
        var bombScript = bomb.GetComponent<BombScript>();
        var playerScript = player.GetComponent<PlayerControl>();
            
        bombScript.firepower = playerScript.firepowerCount;
        bombScript.grid = _gridScript;
        bombScript.bombLayer = playerScript;
        playerScript.currentPlacedBombCount++;
            
        NetworkServer.Spawn(bomb);
            
        //print("Server : Hello i shat a bomb where player told me to");
    }

    public void OnCreateFireCommand(GameObject fireInstance)
    {
        NetworkServer.Spawn(fireInstance);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        //print("Server: Changed scene to -> " + sceneName);

        if (sceneName == "Assets/Scenes/GameScene.unity")
        {
            InitGrid();
            print("Grid inited");
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        _playerCount++;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        _playerCount--;
    }

}
