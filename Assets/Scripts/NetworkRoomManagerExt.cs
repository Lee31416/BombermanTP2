using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.NetworkRoom
{
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        [SerializeField] private InputField _inputAddress;
        //[SerializeField] private InputField _inputUsername;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _connectionMenu;
        [SerializeField] private TextMeshProUGUI _connectionMenuTitle;
        
        private int _playerCount;
        
        public Dictionary<int, PlayerControl> players = new Dictionary<int, PlayerControl>();
        
        /// <summary>
        /// This is called on the server when a networked scene finishes loading.
        /// </summary>
        /// <param name="sceneName">Name of the new scene.</param>


        /// <summary>
        /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
        /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
        /// into the GamePlayer object as it is about to enter the Online scene.
        /// </summary>
        /// <param name="roomPlayer"></param>
        /// <param name="gamePlayer"></param>
        /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            //PlayerScore playerScore = gamePlayer.GetComponent<PlayerScore>();
            //playerScore.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
            return true;
        }

        public override void OnRoomStopClient()
        {
            base.OnRoomStopClient();
        }

        public override void OnRoomStopServer()
        {
            base.OnRoomStopServer();
        }
        
            public void StartHostOnClick()
    {
        StartHost();
        //_playerUsername = _inputUsername.text == "" ? "Host" : _inputUsername.text;
        _mainMenu.SetActive(false);
    }

    public void ConnectToIP()
    {
        networkAddress = _inputAddress.text == "" ? "localhost" : _inputAddress.text;
        //_playerUsername = _inputUsername.text == "" ? "Bomberman" : _inputUsername.text;
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

    /*public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        //var startPosition = GetStartPosition();
        //var player = Instantiate(_whitePlayerPrefab, startPosition);
        //player.name = $"{_playerUsername} [connId={conn.connectionId}]" ;
        var startPos = GetStartPosition();
        var player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        _playerCount++;
        players.Add(_playerCount, player.GetComponent<PlayerControl>());
        //print("SERVER: player grid instance" + playerScript.grid);
    }*/
        
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

        /*
            This code below is to demonstrate how to do a Start button that only appears for the Host player
            showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
            all players are ready, but if a player cancels their ready state there's no callback to set it back to false
            Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
            Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
            is set as DontDestroyOnLoad = true.
        */

        bool showStartButton;

        public override void OnRoomServerPlayersReady()
        {
            // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
#endif
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                // set to false to hide it in the game scene
                showStartButton = false;

                ServerChangeScene(GameplayScene);
            }
        }
    }
}
