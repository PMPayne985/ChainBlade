using UnityEngine;
namespace Zer0
{
    public class Impact : MonoBehaviour
    {
        [SerializeField, Tooltip("Force applied to objects struck")]
        private float force = 1f;
        [SerializeField, Tooltip("Particle effect displayed when an object is struck")]
        private GameObject smokePrefab;
        [SerializeField, Tooltip("Check if this weapon should push, pushable objects.")]
        private bool canPush;
        [SerializeField, Tooltip("Check if this weapon should drag, draggable objects.")]
        private bool canDrag;

        private Transform _emissionPoint;
        private GameObject _smoke;
        
        [HideInInspector] public bool hit;

        private void Start()
        {
            _smoke = Instantiate(smokePrefab);
            _smoke.SetActive(false);
            _emissionPoint = transform.Find("EmissionPoint");
        }

        private void OnTriggerEnter(Collider col)
        {
            print($"Impacted {col.name}");
            hit = true;
            _smoke.transform.position = _emissionPoint.position;
            _smoke.transform.rotation = Quaternion.LookRotation(-transform.forward);
            _smoke.SetActive(true);
            
            Destroy(_smoke, 3);
            _smoke = Instantiate(smokePrefab);
            _smoke.SetActive(false);

            if (canPush && col.TryGetComponent(out IPushable pushable))
                pushable.Push(transform, force);
            else if (canDrag && col.TryGetComponent(out IDraggable draggable))
                draggable.Drag(transform);
        }

        private void OnDestroy()
        {
            Destroy(_smoke);
        }

    }
}
