using System;
using Map;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private InputField _inputAddress;
    [SerializeField] private InputField _inputUsername;
    [SerializeField] private GameObject _hud;
    [SerializeField] private GameObject _whitePlayerPrefab;
    
    private int _playerCount = 0;
    private String _playerUsername;

    public void StartHostOnClick()
    {
        StartHost();
        _playerUsername = _inputUsername.text == "" ? "Host" : _inputUsername.text;
        _hud.SetActive(false);
    }

    public void ConnectToIP()
    {
        networkAddress = _inputAddress.text == "" ? "localhost" : _inputAddress.text;
        _playerUsername = _inputUsername.text == "" ? "Bomberman" : _inputUsername.text;
        StartClient();
        _hud.SetActive(false);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        var startPosition = GetStartPosition();
        var player = Instantiate(_whitePlayerPrefab, startPosition);
        player.name = $"{_playerUsername} [connId={conn.connectionId}]" ;
        NetworkServer.AddPlayerForConnection(conn, player);
        
        
    }

    private void InitGrid(PlayerControl player)
    {
        var grid = GameObject.Find("Grid"); 
        var gridScript = grid.GetComponent<GridScript>();
        gridScript.InitializeSauce();
        grid.GetComponentInChildren<MapGenerator>().GenerateMap(gridScript);
        grid.GetComponentInChildren<WallGenerator>().GenerateWalls(gridScript);
        grid.GetComponentInChildren<BreakableBlockGenerator>().GenerateBreakableBlocks(gridScript);
        grid.GetComponentInChildren<ItemGeneratorScript>().GenerateItemAtRandom(gridScript);
        
        
        //_referenceTilemap = _grid.GetComponentInChildren<Tilemap>();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == "GameScene")
        {
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
