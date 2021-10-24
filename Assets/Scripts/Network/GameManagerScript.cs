using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameManagerScript: NetworkBehaviour
    {
        [SerializeField] private GameObject _gameUi;
        [SerializeField] private GameObject _roundUi;
        private CustomNetworkManager _networkManager;
        
        private void Start()
        {
            var temp = GameObject.Find("NetworkManager");
            _networkManager = temp.GetComponent<CustomNetworkManager>();
            StartCoroutine(GameLoop());
        }
        
        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());
            
                     
           
        }

        private IEnumerator RoundStarting()
        {
            //TODO Update timer
            EnableAllPlayers();
            yield return null;
        }



        private IEnumerator RoundPlaying()
        {
            //TODO Update timer
            yield return null;
        }
        
        private IEnumerator RoundEnding()
        {
            //TODO Update timer
            yield return null;
        }
        
        private void EnableAllPlayers()
        {
            foreach (var player in _networkManager.players)
            {
                player.Value.enabled = true;
            }
        }
    }
}