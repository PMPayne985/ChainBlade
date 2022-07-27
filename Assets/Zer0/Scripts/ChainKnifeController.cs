using System;
using UnityEngine;

namespace Zer0
{
    public class ChainKnifeController : MonoBehaviour
    {
        
        [SerializeField, Tooltip("Initial Knife velocity")] 
        private float knifeVelocity = 20;
        [SerializeField, Tooltip("Knife velocity while dragging an object")] 
        private float dragVelocity = 25;
        [SerializeField, Tooltip("The frequency that links are added to the chain")] 
        private float emissionRate = 10;
        [SerializeField, Tooltip("Distance in meters the knife will travel")] 
        private int maxChainsLength = 50;
        [SerializeField, Tooltip("The prefab that will appear at the end of the chain")] 
        private GameObject knifePrefab;
        [SerializeField, Tooltip("The prefab for each link of the chain")] 
        private GameObject chainPrefab;
        [SerializeField, Tooltip("The blade of the static knife to be deactivated when the chain knife is extended.")] 
        private GameObject knifeBlade;
        [SerializeField, Tooltip("The layers to be detected by the Chain Knife aiming system")]
        private LayerMask detectionLayers;
        private float _distance;
        private float _emitAt;
        
        private Vector3 _velocity;
        private Vector3 _lastPosition;

        private GameObject _knife;
        private GameObject[] _chains;
        private Camera _mainCamera;
        
        private bool _pressed;
        private bool _dragging;
        
        private int _chainLength;
        private int _firstLink;

        private void Awake()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            
        }

        private void Start()
        {
            _emitAt = 1 / emissionRate;
            _chains = new GameObject[maxChainsLength];
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(1) && _knife != null)
            {
                _dragging = true;
                _pressed = false;
            }

            if (_pressed && (_chainLength >= maxChainsLength))
            {
                _dragging = true;
                _pressed = false;
            }
        }

        private void FixedUpdate()
        {
            if (_pressed)
                ChainsForward();
            

            if (_dragging)
                ChainsReturn();
        }

        public void LaunchChain()
        {
            if (_knife) return;
            
            _knife = Instantiate(knifePrefab);
            knifeBlade.SetActive(false);
            _knife.transform.position = transform.position + transform.forward * 0.3f;
            _knife.transform.rotation = transform.rotation;

            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitPoint, Mathf.Infinity, detectionLayers))
                _knife.transform.LookAt( hitPoint.point );
            
            _velocity = _knife.transform.forward * knifeVelocity;
            _pressed = true;
        }
        
        public void ChainsForward()
        {
            _velocity += Vector3.down * Time.deltaTime;
            _knife.transform.position += _velocity * Time.deltaTime;
            _knife.transform.rotation = Quaternion.LookRotation(_velocity);
            
            if (_chainLength < maxChainsLength)
            {
                _distance += Vector3.Distance(_lastPosition, _knife.transform.position);
                _lastPosition = _knife.transform.position;
                if (_distance > _emitAt)
                {
                    var chain = Instantiate(chainPrefab);
                    chain.transform.position = _knife.transform.position - _knife.transform.forward;
                    chain.transform.rotation = _knife.transform.rotation;
                    _chains[_chainLength] = chain;
                    _chainLength++;
                    _distance = 0;
                }
            }

            if (_knife.GetComponent<ApplyForce>().hit)
                EndExtension();
        }

        public void ChainsReturn()
        {
            if (_chainLength == 0)
            {
                knifeBlade.SetActive(true);
                DestroyChainPart(_knife);
                return;
            }

            if (_chains[_chainLength - 1] == null)
            {
                RemoveLinks(gameObject, ref _knife);
                if (!(Vector3.Distance(transform.position, _knife.transform.position) < 0.1f)) return;
                
                knifeBlade.SetActive(true);
                DestroyChainPart(_knife);
                _dragging = false;
                _chainLength = 0;
                _firstLink = 0;
            }
            else
            {
                var first = gameObject;
                if (Vector3.Distance(first.transform.position, _chains[_firstLink].transform.position) < 0.1f)
                {
                    DestroyChainPart(_chains[_firstLink]);
                    _firstLink++;
                }

                for (int i = _firstLink; i < _chainLength; i++)
                {
                    if (_chains[i] == null) continue;
                    RemoveLinks(first, ref _chains[i]);
                    first = _chains[i];
                }

                RemoveLinks(first, ref _knife);
            }

        }

        private void RemoveLinks(GameObject first, ref GameObject next)
        {
            var direction = first.transform.position - next.transform.position;
            var mag = direction.magnitude;
            direction.Normalize();
            mag = Mathf.Min(mag, dragVelocity * Time.deltaTime);
            next.transform.position += direction * mag;
            if (direction != Vector3.zero)
            {
                next.transform.rotation = Quaternion.RotateTowards(next.transform.rotation,
                    Quaternion.LookRotation(-direction), 500 * Time.deltaTime);
            }
        }

        private void DestroyChainPart(GameObject go)
        {
            var allChildren = go.GetComponentsInChildren<Transform>();

            if (allChildren.Length > 0)
            {
                foreach (var obj in allChildren)
                {
                    if (obj.TryGetComponent(out IDraggable dragged))
                        dragged.ReleaseTarget();
                }
            }
            
            DestroyImmediate(go);
        }

        public void EndExtension()
        {
            _pressed = false;
            _dragging = true;
            print("Ended on impact.");
        }
    }
}
