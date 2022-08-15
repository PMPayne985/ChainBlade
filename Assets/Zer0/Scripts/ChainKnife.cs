using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
        [SerializeField, Tooltip("The point the knife blade first appears on a chain attack.")]
        private Transform emitPoint;

        private float _distance;
        private float _emitAt;
        
        private Vector3 _velocity;
        private Vector3 _lastPosition;

        private GameObject _chainHead;
        private List<GameObject> _chain;
        private Transform _character;

        private bool _pressed;

        private ObjectPool<GameObject> _chainPool;

        private void Awake()
        {
            _character = transform.root;
            _chainPool = new ObjectPool<GameObject>(CreateLink, OnGetLink, OnReleaseLink, OnDestroyLink, true, 5, 50);
        }

        private void Start()
        {
            _emitAt = 1 / emissionRate;
            _chainHead = Instantiate(knifePrefab, emitPoint, true);
            _chainHead.GetComponentInChildren<PlayerImpact>().SetChainKnife(this);
            _chainHead.SetActive(false);
            _chain = new List<GameObject>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(1) && _chainHead.activeInHierarchy)
                _pressed = false;

            if (_pressed && (_chain.Count >= maxChainsLength))
                _pressed = false;
        }

        private void FixedUpdate()
        {
            if (_pressed)
                ChainExtend();
            else if (!_pressed && _chainHead.activeInHierarchy)
                ChainReturn();
        }

        public void LaunchChain()
        {
            if (_chainHead.activeInHierarchy) return;
            
            _chainHead.SetActive(true);
            knifeBlade.SetActive(false);
            _chainHead.transform.position = emitPoint.position + _character.forward * 0.3f;
            _chainHead.transform.rotation = _character.rotation;
            
            var lookPoint = emitPoint.position + _character.forward * maxChainsLength;

            _chainHead.transform.LookAt(lookPoint);
            _velocity = _chainHead.transform.forward * knifeVelocity;
            _pressed = true;
        }

        private void ChainExtend()
        {
            MoveChainHeadForward();
            
            if (_chain.Count >= maxChainsLength) return;
            
            _distance += Vector3.Distance(_lastPosition, _chainHead.transform.position);
            _lastPosition = _chainHead.transform.position;

            if (_distance > _emitAt)
                _chainPool.Get();
        }

        private void MoveChainHeadForward()
        {
            _velocity += Vector3.forward * Time.deltaTime;
            _chainHead.transform.position += _velocity * Time.deltaTime;
            _chainHead.transform.rotation = Quaternion.LookRotation(_velocity);
        }
        
        private void ChainReturn()
        {
            if (_chain.Count == 0)
            {
                knifeBlade.SetActive(true);
                DisableChainPart(_chainHead);
                return;
            }
            
            var currentChainPart = emitPoint.gameObject;
            
            if (Vector3.Distance(currentChainPart.transform.position, _chain[0].transform.position) < 0.1f)
                DisableChainPart(_chain[0]);

            if (_chain.Count == 0)
                return;
            
            foreach (var link in _chain)
            {
                MoveChainPartsBack(currentChainPart, link);
                currentChainPart = link;
            }

            MoveChainPartsBack(currentChainPart,  _chainHead);
        }

        private void MoveChainPartsBack(GameObject firstChainPart, GameObject nextChainPart)
        {
            var direction = firstChainPart.transform.position - nextChainPart.transform.position;
            var mag = Mathf.Min(direction.magnitude, dragVelocity * Time.deltaTime);
            direction.Normalize();
            
            nextChainPart.transform.position += direction * mag;
            
            if (direction != Vector3.zero)
            {
                nextChainPart.transform.rotation = Quaternion.RotateTowards(nextChainPart.transform.rotation,
                    Quaternion.LookRotation(-direction), 500 * Time.deltaTime);
            }
        }

        private void DisableChainPart(GameObject chainPart)
        {
            if (chainPart == _chainHead)
            {
                var allChildren = chainPart.GetComponentsInChildren<Transform>();

                if (allChildren.Length > 0)
                {
                    foreach (var t in allChildren)
                    {
                        if (t.TryGetComponent(out IDraggable dragged))
                            dragged.ReleaseTarget();
                    }
                }

                _chainHead.SetActive(false);
            }
            else
                _chainPool.Release(chainPart);
        }

        public void EndExtension()
        {
            _pressed = false;
        }

        private GameObject CreateLink()
        {
            var link = Instantiate(chainPrefab, _chainHead.transform, false);
            return link;
        }

        private void OnGetLink(GameObject link)
        {
            _chain.Add(link);
            link.transform.position = _chainHead.transform.position - _chainHead.transform.forward;
            link.transform.rotation = _chainHead.transform.rotation;
            _distance = 0;
            link.SetActive(true);
        }

        private void OnReleaseLink(GameObject link)
        {
            _chain.Remove(link);
            link.transform.position = emitPoint.transform.position;
            link.transform.rotation = emitPoint.transform.rotation;
            link.SetActive(false);
        }

        private void OnDestroyLink(GameObject link)
        {
            DestroyImmediate(link);
        }
    }
}
