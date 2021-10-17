using System;
using System.Collections;
using Map;
using Player;
using UnityEngine;

namespace Gameplay
{
    public class BombScript : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _fireUp;
        [SerializeField] private GameObject _fireDown;
        [SerializeField] private GameObject _fireLeft;
        [SerializeField] private GameObject _fireRight;
        [SerializeField] private GameObject _fireEndUp;
        [SerializeField] private GameObject _fireEndDown;
        [SerializeField] private GameObject _fireEndLeft;
        [SerializeField] private GameObject _fireEndRight;
        
        private GridScript _grid;
        private PlayerControl _bombLayer;
        private int _firepower = 1;
        
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
            var position = transform.position;
            _grid.DestroyTiles(position, firepower);
            _bombLayer.currentPlacedBombCount--;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            print("BombTrigger: " + other);
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

        private void DestroyBomb()
        {
            Destroy(gameObject);
        }
    }
}
