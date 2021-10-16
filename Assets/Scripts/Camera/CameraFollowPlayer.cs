using UnityEngine;

namespace Camera
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        public Transform player;
        private UnityEngine.Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        void FixedUpdate () 
        {
            transform.position = new Vector3 (player.position.x, player.position.y, transform.position.z);
        }
    }
}