using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.NetworkRoom;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameManagerScript: NetworkBehaviour
    {
        [SerializeField] private GameObject _gameUi;
        [SerializeField] private GameObject _roundUi;
        [SerializeField] private GameObject _winUi;
        [SerializeField] private TextMeshProUGUI _winnerNameText;
 
        private NetworkRoomManagerExt _manager;
        private Dictionary<int, PlayerControl> _alivePlayers = new Dictionary<int, PlayerControl>();

        private void Start()
        {
            if (!isServer) return;
            
            print("Starting game manager");
            
            _manager = NetworkManager.singleton.GetComponentInChildren<NetworkRoomManagerExt>();
            _alivePlayers = _manager.players;

            StartCoroutine(GameLoop());
        }

        private IEnumerator GameLoop()
        {
            DisableAllPlayers();
            
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());
        }

        private IEnumerator RoundStarting()
        {
            yield return new WaitForSeconds(10);
            EnableAllPlayers();
            print("Starting Game now");
        }

        private IEnumerator RoundPlaying()
        {
            //TODO Update timer
            
            while (!IsOnePlayerLeft())
            { 
                print("more than 1 player left");
                yield return null;
            }
            
            print("1 player left... ending");
        }
        
        private IEnumerator RoundEnding()
        {
            DisableAllPlayers();
            print("Game ending now");
            
            ShowWinUi(GetGameWinner());
            
            yield return new WaitForSeconds(2);
        }

        [ClientRpc]
        private void ShowWinUi(PlayerControl winner)
        {
            _winnerNameText.text = winner == null ? "Tie" : winner.name;
            _winUi.SetActive(true);
        }
        
        private void EnableAllPlayers()
        {
            foreach (var player in _manager.players)
            {
                player.Value.RpcUnFreezeAllClient();
            }
            
            print("Unfrozed everyone");
        }
        
        private void DisableAllPlayers()
        {
            foreach (var player in _manager.players)
            {
                player.Value.RpcFreezeAllClient();
            }
            
            print("frozed everyone");
        }
        
        private PlayerControl GetGameWinner()
        {
            return _manager.players.Where(player => player.Value.isAlive).Select(player => player.Value).FirstOrDefault();
        }
        
        private bool IsOnePlayerLeft()
        {
            var alivePlayerCount = _manager.players.Count;
            var temp = _manager.players.ToList();
            foreach (var player in temp)
            {
                if (player.Value.isAlive) continue;
                alivePlayerCount--;
                _alivePlayers.Remove(player.Key);
            }
            return alivePlayerCount <= 1;
        }
    }
}