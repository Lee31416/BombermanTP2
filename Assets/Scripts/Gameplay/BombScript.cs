using System;
using System.Collections;
using Map;
using Mirror;
using Player;
using UnityEngine;

namespace Gameplay
{
    public class BombScript : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;
        
        private PlayerControl _bombLayer;
        private bool _animationFinished = false;
        
        [SyncVar]
        private bool _hasExploded = false;
        
        public PlayerControl bombLayer
        {
            set => _bombLayer = value;
        }

        public int firepower { get; set; } = 1;

        void Start()
        {
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(4);
            _animator.SetTrigger("Explode");
            _hasExploded = true;
            if (isServer)
                RpcExplodeBombOnAllClients();
            else
                CmdExplodeBomb();
        }
        
        //For now to fix this 
        [Command(requiresAuthority = false)]
        public void CmdExplodeBomb()
        {
            RpcExplodeBombOnAllClients();
        }

        [ClientRpc]
        private void RpcExplodeBombOnAllClients()
        {
            ToggleIsTrigger();
            var position = transform.position;
            var grid = GameObject.Find("Grid").GetComponent<GridScript>();
            grid.DestroyTiles(position, firepower);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer) return;
            if (!_hasExploded) return;
            var player = other.GetComponent<PlayerControl>();
            if (player == null) return;
            player.Kill();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ToggleIsTrigger();
        }

        private void ToggleIsTrigger()
        {
            var collider2D = GetComponent<BoxCollider2D>();
            collider2D.isTrigger = !collider2D.isTrigger;
        }

        public void DestroyBombAfterAnimation()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
