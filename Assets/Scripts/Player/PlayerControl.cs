using System;
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
        [SerializeField] private Tilemap _referenceTilemap;
        [SerializeField] private GameObject _bombPrefab;
        [SerializeField] private GameObject _cameraPrefab;
        [SerializeField] private GameObject _uiPrefab;
        
        private Vector2 _movement;
        private bool _isMoving = false;
        public int firepowerCount = 1;
        public int rollerbladeCount = 1;
        public int bombCount = 1;
        public int currentPlacedBombCount = 0;
        public float _speed;
        private bool _isAlive = true;
        private CameraControl _cameraControl;
        private ItemCountScript[] _uiCounters;
        private GridScript _gridScript;
        
        [SyncVar]
        private GameObject _grid;

        private void Start()
        {
            if (isServer)
            {
                print("Im the server");
                _grid = GameObject.Find("Grid");
                _gridScript = _grid.GetComponent<GridScript>();
                _grid.GetComponentInChildren<MapGenerator>().GenerateMap();
                _grid.GetComponentInChildren<WallGenerator>().GenerateWalls();
                _grid.GetComponentInChildren<BreakableBlockGenerator>().GenerateBreakableBlocks();
                _grid.GetComponentInChildren<ItemGeneratorScript>().GenerateItemAtRandom();
                _referenceTilemap = _grid.GetComponentInChildren<Tilemap>();
                print("Just initialised the grid");
                GetGridData(_grid);
            }
        }

        public override void OnStartLocalPlayer()
        {
            print("test on startlocalplayer");
            base.OnStartLocalPlayer();

            _cameraControl = Instantiate(_cameraPrefab).GetComponent<CameraControl>();
            _uiCounters = Instantiate(_uiPrefab).GetComponentsInChildren<ItemCountScript>();
            _cameraControl.target = gameObject;
            
            foreach (var counter in _uiCounters)
            {
                counter.target = this;
            }
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            if (_grid == null)
            {
                AskServerForGridData();
            }
            Move();

            _speed = rollerbladeCount;
            
            if (Input.GetMouseButtonDown(0) && currentPlacedBombCount < bombCount)
            {
               OnLayBombCommand();
            }

            if (Input.GetKeyDown("e"))
            {
                _gridScript.PrintGrid();
            }

            if (Input.GetKeyDown("f"))
            {
               AskServerForGridData();
            }
        
            _animator.SetFloat("MovementX", _movement.x);
            _animator.SetFloat("MovementY", _movement.y);
            _animator.SetBool("IsAlive", _isAlive);
 
            _isMoving = _rb.velocity.x != 0 || _rb.velocity.y != 0;
            _animator.SetBool("IsMoving", _isMoving);
        }

        private void Move()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            _rb.velocity = new Vector2(_movement.x * _speed / 2, _movement.y * _speed / 2);
        }

        [Command]
        private void AskServerForGridData()
        {
            var grid = _grid;
            print("client asked for grid data");
            GetGridData(grid);
        }
        
        [Command]
        private void OnLayBombCommand()
        {
            print("Hello this is the server : A client ask to lay a bomb");
            
            var pos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cell = _referenceTilemap.WorldToCell(pos);
            var cellCenterPos = _referenceTilemap.GetCellCenterWorld(cell);
            var bomb = Instantiate(_bombPrefab, cellCenterPos, Quaternion.identity);
            var bombScript = bomb.GetComponent<BombScript>();
            
            bombScript.firepower = firepowerCount;
            bombScript.grid = _gridScript;
            bombScript.bombLayer = this;
            currentPlacedBombCount++;
            
            NetworkServer.Spawn(bomb);
            
            print("Server : Hello i shat a bomb where player told me to");
        }
        
        [ClientRpc]
        private void GetGridData(GameObject grid)
        {
            print("Received grid data from server");
            _grid = grid;
        }
        
        [TargetRpc]
        public void Kill()
        {
            _isAlive = false;
        }
        
    }
}
