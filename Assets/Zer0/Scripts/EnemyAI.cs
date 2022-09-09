using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Zer0
{
    public class EnemyAI : Character, IDraggable
    {
        [SerializeField, Tooltip("The player object that this enemy will target.")]
        private Player player;

        [SerializeField, Tooltip("The Enemy Impact component on the weapon carried by this enemy.")]
        private EnemyImpact weapon;

        [SerializeField, Tooltip("The animator that controls attack and death states for this enemy.")]
        private Animator animator;
        
        [SerializeField, Tooltip("If this scene is using a scoring system assign the Score UI component here.")]
        private ScoreUI scoreUI;

        [SerializeField, Tooltip("The Status Effect Component that will control active effects on this enemy.")]
        private StatusEffects statusEffects;

        [SerializeField, Tooltip("The distance from the player this enemy will stop and attack.")]
        private float attackDistance;
        
        private EnemySpawner _spawner;
        private Collider _weaponCollider;
        private AISimpleController _controller;
        
        private static int _score;
        
        private float _snapToHeight;
        private float _lastAttackIndex;
        private float _targetDistance;
        private float _damageDelay;
        
        private bool _attacking;
        
        private Quaternion _originalRotation;
        
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Damage = Animator.StringToHash("TakeDamage");

        private void Awake()
        {
            #region Check For Weapon
            if (!weapon)
                weapon = GetComponentInChildren<EnemyImpact>();
            if (!weapon)
                Debug.LogError($"Missing Enemy Impact component.");
            #endregion
            #region Check For Animator
            if (!animator)
                animator = GetComponent<Animator>();
            if (!animator)
                Debug.LogError($"Missing Animator component.");
            #endregion
            #region Check For Status Effect System
            if (!statusEffects)
                statusEffects = GetComponent<StatusEffects>();
            if (!statusEffects)
                Debug.LogError($"Missing Animator component.");
            #endregion
            #region Check For Controller
            if (!_controller)
                _controller = GetComponent<AISimpleController>();
            if (!_controller)
                Debug.LogError($"Missing AI Simple Controller component.");
            #endregion
            #region Check For Score system
            if (!scoreUI)
                scoreUI = FindObjectOfType<ScoreUI>();
            #endregion

            _weaponCollider = weapon.GetComponent<Collider>();
        }

        private void OnEnable()
        {
            #region Find Player
            if (!player)
                player = FindObjectOfType<Player>();
            #endregion
        }

        private void Update()
        {
            Attack();
            
            _targetDistance = Vector3.Distance(player.transform.position, transform.position);

            _controller.stopped = (_targetDistance <= attackDistance);

            if (DamageReceived)
            {
                _damageDelay += Time.deltaTime;
                if (_damageDelay >= 0.25f)
                {
                    _controller.stopped = false;
                    DamageReceived = false;
                }
            }
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            _spawner = spawner;
        }
        
        public void Drag(Transform dragger)
        {
            _originalRotation = transform.rotation;
            _snapToHeight = dragger.transform.position.y + 1;
            transform.parent = dragger;
        }

        public void ReleaseTarget()
        {
            transform.parent = null;
            transform.rotation = _originalRotation;
            var pos = transform.position;
            transform.position = new Vector3(pos.x, _snapToHeight, pos.z);
        }

        private void Attack()
        {
            if (!CanAttack()) return;
            
            _attacking = true;
            _weaponCollider.enabled = true;

            var randomAttackIndex = RandomAttackIndex();

            animator.SetTrigger(AttackTrigger);
            animator.SetFloat(AttackIndex, randomAttackIndex);
            _lastAttackIndex = randomAttackIndex;
        }

        public bool CanAttack()
        {
            if (_attacking) return false;
            if (!player) return false;

            return !(_targetDistance > attackDistance);
        }
        
        public void SendDamage(int amount)
        {
            _attacking = false;
            _weaponCollider.enabled = false;
        }

        public override void TakeDamage(float damageTaken)
        {
            base.TakeDamage(damageTaken);
            if (DamageReceived)
            {
                animator.SetTrigger(Damage);
                _controller.stopped = true;
                _damageDelay = 0;
            }
        }

        public override void Death()
        {
            base.Death();

            statusEffects.SetDeathStatus(dead);
            
            if (_spawner)
                _spawner.ReleaseEnemy(gameObject);
            
            statusEffects.ClearAllEffects();
            
            gameObject.SetActive(false);
            _attacking = false;
        }
        
        private int RandomAttackIndex()
        {
            var randomNum = Random.Range(0, 3);

            while (randomNum == _lastAttackIndex)
                randomNum = Random.Range(0, 3);

            return randomNum;
        }

        public void EndAttack()
        {
            _weaponCollider.enabled = false;
        }

        public override void InitiateDeath()
        {
            base.InitiateDeath();
            animator.SetBool(Dead, true);
            _score++;

            if (scoreUI)
                scoreUI.SetScore(_score);
        }

        public override void Revive()
        {
            base.Revive();
            dead = false;
            Health = maxHealth;
        }

        public static void ResetScore()
        {
            _score = 0;
        }
    }
}
