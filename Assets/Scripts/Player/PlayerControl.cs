using System;
using Camera;
using Gameplay;
using Map;
using Mirror;
using Network;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerControl : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _cameraPrefab;
        [SerializeField] private GameObject _uiPrefab;
        [SerializeField] private GameObject _pauseMenuPrefab;
        [SerializeField] private GameObject _bombPrefab;
        
        private Vector2 _movement;
        private bool _isMoving = false;
        public int firepowerCount = 1;
        public int rollerbladeCount = 1;
        public int bombCount = 1;
        public int currentPlacedBombCount = 0;
        public float _speed;

        private CameraControl _cameraControl;
        private ItemCountScript[] _uiCounters;
        private PauseMenu _pauseMenu;
        private GameObject _grid;
        private Tilemap _tilemap;

        [SyncVar]
        private bool _isAlive = true;
        
        public void Start()
        {
            _grid = GameObject.Find("Grid");
            _tilemap = _grid.GetComponentInChildren<Tilemap>();
            _grid.GetComponentInChildren<MapGenerator>().GenerateMap();
            _grid.GetComponentInChildren<WallGenerator>().GenerateWalls();
            //_grid.GetComponentInChildren<BreakableBlockGenerator>().GenerateBreakableBlocks();
            //_grid.GetComponentInChildren<ItemGeneratorScript>().GenerateItemAtRandom();
        }

        public override void OnStartLocalPlayer()
        {
            _cameraControl = Instantiate(_cameraPrefab).GetComponent<CameraControl>();
            _uiCounters = Instantiate(_uiPrefab).GetComponentsInChildren<ItemCountScript>();
            _pauseMenu = Instantiate(_pauseMenuPrefab).GetComponent<PauseMenu>();
            _cameraControl.target = gameObject;
            _pauseMenu.player = this;
            
            foreach (var counter in _uiCounters)
            {
                counter.target = this;
            }
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
       
            _speed = rollerbladeCount;

            if (Input.GetKeyDown("space") && currentPlacedBombCount < bombCount)
            {
                var pos = transform.position;
                var cell = _tilemap.WorldToCell(pos);
                var cellCenterPos = _tilemap.GetCellCenterWorld(cell);
                CmdLayBomb(GetComponent<NetworkIdentity>(), cellCenterPos);
            }

            _animator.SetFloat("MovementX", _movement.x);
            _animator.SetFloat("MovementY", _movement.y);
 
            _isMoving = _rb.velocity.x != 0 || _rb.velocity.y != 0;
            _animator.SetBool("IsMoving", _isMoving);
        }

        private void FixedUpdate()
        {
            Move();
        }

        [Command]
        private void CmdLayBomb(NetworkIdentity networkIdentity, Vector3 cellCenterPos)
        {
            var bomb = Instantiate(_bombPrefab, cellCenterPos, Quaternion.identity);
            var bombScript = bomb.GetComponent<BombScript>();
            
            bombScript.firepower = firepowerCount;
            bombScript.bombLayer = this;
            currentPlacedBombCount++;
            NetworkServer.Spawn(bomb);
        }

        private void Move()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            _rb.velocity = new Vector2(_movement.x * _speed / 2, _movement.y * _speed / 2);
        }
        
        public void Kill()
        {
            _isAlive = false;
            RpcKillClient();
        }

        [TargetRpc]
        private void RpcKillClient()
        {
            _animator.SetBool("IsAlive", _isAlive);
            _rb.velocity = new Vector2(5, 5);
            _rb.simulated = false;
            enabled = false;
        }
    }
}
