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
        
        private GridScript _grid;
        private PlayerControl _bombLayer;
        private int _firepower = 1;
        
        [SyncVar]
        private bool _hasExploded = false;
        
        public PlayerControl bombLayer
        {
            get => _bombLayer;
            set => _bombLayer = value;
        }
        public GridScript grid
        {
            get => _grid;
            set => _grid = value;
        }
        public int firepower
        {
            get => _firepower;
            set => _firepower = value;
        }
        

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            yield return new WaitForSeconds(4);
            _animator.SetTrigger("Explode");
            _hasExploded = true;
            RpcExplodeBombOnAllClients();
        }

        [ClientRpc]
        private void RpcExplodeBombOnAllClients()
        {
            ToggleIsTrigger();
            var position = transform.position;
            _grid.DestroyTiles(position, firepower);
            _bombLayer.currentPlacedBombCount--;
            NetworkServer.Destroy(gameObject);
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
    }
}
