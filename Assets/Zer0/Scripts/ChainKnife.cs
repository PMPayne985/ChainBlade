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
        [SerializeField, Tooltip("The number of chain links per meter extended.")] 
        private float emissionRate = 10;
        [SerializeField, Tooltip("Total number of chain links the knife will travel.")] 
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
        private GameObject[] _chain;
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
            _chainPool = new ObjectPool<GameObject>(CreateLink, OnLinkGet, OnLinkReturn, OnLinkReturn, true, 5, 50);
        }

        private void Start()
        {
            _emitAt = 1 / emissionRate;
            _chain = new GameObject[maxChainsLength];
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
                    AddChainPart();
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

            if (!_chain[_chainLength - 1])
            {
                MoveChainParts(gameObject, ref _chainHead);
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
                if (Vector3.Distance(first.transform.position, _chain[_firstLink].transform.position) < 0.1f)
                {
                    DisableChainPart(_chain[_firstLink]);
                    _firstLink++;
                }

                for (int i = _firstLink; i < _chainLength; i++)
                {
                    if (!_chain[i]) continue;
                    MoveChainParts(first, ref _chain[i]);
                    first = _chain[i];
                }

                MoveChainParts(first, ref _chainHead);
            }

        }

        private void MoveChainParts(GameObject first, ref GameObject next)
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
                //_chainPool.Release(go);
        }

        private void AddChainPart()
        {
            var chain = Instantiate(chainPrefab, _chainHead.transform, true);
            //var chain = _chainPool.Get();
            chain.transform.position = _chainHead.transform.position - _chainHead.transform.forward;
            chain.transform.rotation = _chainHead.transform.rotation;
            _chain[_chainLength] = chain;
            _chainLength++;
            _distance = 0;
        }
        
        public void EndExtension()
        {
            _pressed = false;
            _dragging = true;
        }

        private GameObject CreateLink()
        {
            var chain = Instantiate(chainPrefab, _chainHead.transform, true);
            return chain;
        }

        private void OnLinkGet(GameObject link)
        {
            link.SetActive(true);
        }

        private void OnLinkReturn(GameObject link)
        {
            link.SetActive(false);
        }
    }
}
