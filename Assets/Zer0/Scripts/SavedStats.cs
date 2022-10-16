using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zer0
{
    public class SavedStats : MonoBehaviour
    {
        public static SavedStats Instance;
        public  int linkedIndex;
        public int maxHealth;
        public float currentHealth;
        
        public float maxSpellPoints;
        public float currentSpellPoints;
        public List<SpellData> spells;
        public int spellIndex;

        public int currentLinks;
        public int maxLinks;
        public int armorUpgrade;
        public int bladeUpgrade;
        public int spellUpgrade;
        
        public int aHealth;
        public int aDefence;
        public int aSpeed;
        public int aResistance;
        public int aRetaliation;
        public int aCurrentPoints;
        public int aTotalPoints;
        
        public int bKnifeDamage;
        public int bChainDamage;
        public int bLifeLeech;
        public int bChainPull;
        public int bLength;
        public int bCurrentPoints;
        public int bTotalPoints;
        public bool bChainPullAdded;
        
        public int sCurrentPoints;
        public int sTotalPoints;
        public int sLightningChains;
        public int sShackles;
        public int sDisarm;
        public int sRefresh;
        public int sIronWeb;
        public int sSpellPoints;

        private Player _player;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{scene.name} scene loaded.");
            Debug.Log(mode);

            _player = FindObjectOfType<Player>();
            Debug.Log($"{_player.gameObject.name} found.");
            if (maxHealth <= 0) return;
            SetupPlayer();
        }

        private void SetupPlayer()
        {
            _player.SetStartingPosition();
            var saveAll = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

            foreach (var saveable in saveAll)
            {
                saveable.LoadData();
            }
        }
    }
}
