using UnityEngine;

    public class Impulse : MonoBehaviour
    {
        [SerializeField, Tooltip("Force applied to objects struck")]
        private float force = 1f;

        [SerializeField, Tooltip("Particle effect displayed when an object is struck")]
        private GameObject smokePrefab;

        private Transform _emissionPoint;
        private GameObject _smoke;

        [HideInInspector] public bool hit;

        private void Start()
        {
            _smoke = Instantiate(smokePrefab);
            _smoke.SetActive(false);
            _emissionPoint = transform.Find("barrel");
        }

        private void OnTriggerEnter(Collider col)
        {
            hit = true;
            _smoke.transform.position = _emissionPoint.position;
            _smoke.transform.rotation = Quaternion.LookRotation(-transform.forward);
            _smoke.SetActive(true);
            Destroy(_smoke, 3);
            _smoke = Instantiate(smokePrefab);
            _smoke.SetActive(false);

            if (col.TryGetComponent(out Zer0.IPushable pushable))
                pushable.Push(transform, force);
                //pushable.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            else if (col.TryGetComponent(out Zer0.IDraggable draggable))
                draggable.Drag(transform);
        }

        private void OnDestroy()
        {
            Destroy(_smoke);
        }

    }
