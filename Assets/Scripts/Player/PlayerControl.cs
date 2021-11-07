using System.Collections;
using Camera;
using Gameplay;
using Map;
using Mirror;
using UnityEngine;
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
        
        [SyncVar]
        public int firepowerCount = 1;
        
        [SyncVar]
        public int rollerbladeCount = 1;
        
        [SyncVar]
        public int bombCount = 1;
        
        [SyncVar]
        public int currentPlacedBombCount = 0;
        
        private float _speed;

        private CameraControl _cameraControl;
        private ItemCountScript[] _uiCounters;
        private PauseMenu _pauseMenu;
        private GameObject _grid;
        private Tilemap _tilemap;

        [field: SyncVar]
        public bool isAlive { get; private set; } = true;

        [SyncVar]
        private bool _isControllable = true;

        public void Start()
        {
            _grid = GameObject.Find("Grid");
            _tilemap = _grid.GetComponentInChildren<Tilemap>();
            _grid.GetComponentInChildren<MapGenerator>().GenerateMap();
            _grid.GetComponentInChildren<WallGenerator>().GenerateWalls();

            if (!isServer) return;
            _grid.GetComponentInChildren<ItemGeneratorScript>().GenerateItemAtRandom();
            _grid.GetComponentInChildren<BreakableBlockGenerator>().GenerateBreakableBlocks();
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
            if (!_isControllable) return;
            if (!isAlive) return;
            
            _speed = (rollerbladeCount == 1) ? 1 : rollerbladeCount * 0.75f;  

            if (Input.GetKeyDown("space") && currentPlacedBombCount < bombCount)
            {
                var pos = transform.position;
                var newPos = new Vector3(pos.x, pos.y - 0.02f, 0);
                var cell = _tilemap.WorldToCell(newPos);
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
            if (!isLocalPlayer) return;
            if (!_isControllable) return;
            if (!isAlive) return;
            Move();
        }

        [Command]
        private void CmdLayBomb(NetworkIdentity networkIdentity, Vector3 cellCenterPos)
        {
            var player = networkIdentity.gameObject.GetComponent<PlayerControl>();
            var bomb = Instantiate(_bombPrefab, cellCenterPos, Quaternion.identity);
            var bombScript = bomb.GetComponent<BombScript>();
            bombScript.firepower = player.firepowerCount;
            StartCoroutine(DecrementPlayerBombCount(player));
            player.currentPlacedBombCount++;
            NetworkServer.Spawn(bomb);
        }

        private IEnumerator DecrementPlayerBombCount(PlayerControl player)
        {
            yield return new WaitForSeconds(4);
            player.currentPlacedBombCount--;
        }

        private void Move()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            _rb.velocity = new Vector2(_movement.x * _speed / 2, _movement.y * _speed / 2);
        }
        
        public void Kill()
        {
            if (!isAlive) return;
            isAlive = false;
            RpcKillClient();
        }

        [ClientRpc]
        private void RpcKillClient()
        {
            _animator.SetBool("IsAlive", isAlive);
            _isControllable = false;
        }

        [ClientRpc]
        public void RpcFreezeAllClient()
        {
            _isControllable = false;
        }
        
        [ClientRpc]
        public void RpcUnFreezeAllClient()
        {
            _isControllable = true;
        }
    }
}
