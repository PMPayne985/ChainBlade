using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = System.Object;

namespace Zer0
{
    public class ChainKnife : MonoBehaviour
    {
        
        [SerializeField, Tooltip("Initial Knife velocity")] 
        private float knifeVelocity = 20;
        [SerializeField, Tooltip("Knife velocity while dragging an object")] 
        private float dragVelocity = 25;
        [SerializeField, Tooltip("The rate to add links to the chain.")] 
        private float emissionRate = 10;
        [SerializeField, Tooltip("Distance in meters the knife will travel")] 
        private int maxChainsLength = 50;
        [SerializeField, Tooltip("The prefab that will appear at the end of the chain")] 
        private GameObject knifePrefab;
        [SerializeField, Tooltip("The prefab for each link of the chain")] 
        private GameObject chainPrefab;
        [SerializeField, Tooltip("The blade of the static knife to be deactivated when the chain knife is extended.")] 
        private GameObject knifeBlade;

        private float _distance;
        private float _emitAt;
        
        private Vector3 _velocity;
        private Vector3 _lastPosition;

        private GameObject _chainHead;
        private GameObject[] _chains;
        private Transform _character;

        private bool _pressed;
        private bool _dragging;
        
        private int _chainLength;
        private int _firstLink;

        private ObjectPool<GameObject> _chainPool;
        private Impact _impact;

        private void Awake()
        {
            _character = transform.root;
        }

        private void Start()
        {
            _emitAt = 1 / emissionRate;
            _chains = new GameObject[maxChainsLength];
            _chainHead = Instantiate(knifePrefab, transform, true);
            _impact = _chainHead.GetComponent<Impact>();
            _chainHead.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(1) && _chainHead.activeInHierarchy)
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
                ChainExtend();
            

            if (_dragging)
                ChainReturn();
        }

        public void LaunchChain()
        {
            if (_chainHead.activeInHierarchy) return;
            
            _chainHead.SetActive(true);
            knifeBlade.SetActive(false);
            _chainHead.transform.position = transform.position + _character.forward * 0.3f;
            _chainHead.transform.rotation = _character.rotation;
            
            var lookPoint = transform.position + _character.forward * maxChainsLength;

            _chainHead.transform.LookAt(lookPoint);
            _velocity = _chainHead.transform.forward * knifeVelocity;
            _pressed = true;
        }
        
        public void ChainExtend()
        {
            _velocity += Vector3.down * Time.deltaTime;
            _chainHead.transform.position += _velocity * Time.deltaTime;
            _chainHead.transform.rotation = Quaternion.LookRotation(_velocity);
            
            if (_chainLength < maxChainsLength)
            {
                _distance += Vector3.Distance(_lastPosition, _chainHead.transform.position);
                _lastPosition = _chainHead.transform.position;
                if (_distance > _emitAt)
                {
                    var chain = Instantiate(chainPrefab, _chainHead.transform, true);
                    chain.transform.position = _chainHead.transform.position - _chainHead.transform.forward;
                    chain.transform.rotation = _chainHead.transform.rotation;
                    _chains[_chainLength] = chain;
                    _chainLength++;
                    _distance = 0;
                }
            }

            if (_impact.hit)
                EndExtension();
        }

        public void ChainReturn()
        {
            if (_chainLength == 0)
            {
                knifeBlade.SetActive(true);
                DisableChainPart(_chainHead);
                return;
            }

            if (!_chains[_chainLength - 1])
            {
                AlignChainPart(gameObject, ref _chainHead);
                if (!(Vector3.Distance(transform.position, _chainHead.transform.position) < 0.1f)) return;
                
                knifeBlade.SetActive(true);
                DisableChainPart(_chainHead);
                _dragging = false;
                _chainLength = 0;
                _firstLink = 0;
            }
            else
            {
                var first = gameObject;
                if (Vector3.Distance(first.transform.position, _chains[_firstLink].transform.position) < 0.1f)
                {
                    DisableChainPart(_chains[_firstLink]);
                    _firstLink++;
                }

                for (int i = _firstLink; i < _chainLength; i++)
                {
                    if (!_chains[i]) continue;
                    AlignChainPart(first, ref _chains[i]);
                    first = _chains[i];
                }

                AlignChainPart(first, ref _chainHead);
            }

        }

        private void AlignChainPart(GameObject first, ref GameObject next)
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

        private void DisableChainPart(GameObject go)
        {
            if (go == _chainHead)
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

                _chainHead.SetActive(false);
            }
            else
                DestroyImmediate(go);
        }

        public void EndExtension()
        {
            _pressed = false;
            _dragging = true;
        }
    }
}
