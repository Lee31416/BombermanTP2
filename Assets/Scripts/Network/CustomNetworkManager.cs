using System;
using System.Collections.Generic;
using Gameplay;
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

    private int _playerCount;
    private string _playerUsername; // NO! Lobby should have its own script and use the NetworkManager instead of adding to NetworkManager directly!
    public Dictionary<int, PlayerControl> players = new Dictionary<int, PlayerControl>();

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
        var startPos = GetStartPosition();
        var player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        
        _playerCount++;
        players.Add(_playerCount, player.GetComponent<PlayerControl>());
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
