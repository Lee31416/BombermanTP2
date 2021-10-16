using UnityEngine;

namespace Gameplay
{
    public class ExplosionCheck : MonoBehaviour
    {
        private bool _hasBeenHit = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            _hasBeenHit = true;
        }

        public bool HasBeenHit()
        {
            return _hasBeenHit;
        }
    }
}
