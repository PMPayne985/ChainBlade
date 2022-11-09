using UnityEngine;

namespace Zer0
{
    public class MapCamera : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject mapCamera;
        [SerializeField] private Vector3 cameraOffset;
        
        private void Start()
        {
            transform.parent = null;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        
        void Update ()
        {
            mapCamera.transform.position = player.transform.position + cameraOffset;
        }
    }
}
