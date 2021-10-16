using Gameplay;
using Map;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private GameObject _bomb;
        [SerializeField] private GridScript _grid;
        
        private Vector2 _movement;
        private bool _isMoving = false;
        public int firepowerCount = 1;
        public int rollerbladeCount = 1;
        public int bombCount = 1;
        public int currentPlacedBombCount = 0;
        public float _speed;
        private bool _isAlive = true;

        private void Update()
        {
            Move();

            _speed = rollerbladeCount;
            
            if (Input.GetMouseButtonDown(0) && currentPlacedBombCount < bombCount)
            {
                LayBomb();
            }

            if (Input.GetKeyDown("e"))
            {
                _grid.PrintGrid();
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

        private void LayBomb()
        {
            var pos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cell = _tilemap.WorldToCell(pos);
            var cellCenterPos = _tilemap.GetCellCenterWorld(cell);
            var bomb = Instantiate(_bomb, cellCenterPos, Quaternion.identity);
            var bombScript = bomb.GetComponent<BombScript>();
            
            bombScript.firepower = firepowerCount;
            bombScript.grid = _grid;
            bombScript.bombLayer = this;
            currentPlacedBombCount++;
        }

        public void Kill()
        {
            _isAlive = false;
        }
    }
}
