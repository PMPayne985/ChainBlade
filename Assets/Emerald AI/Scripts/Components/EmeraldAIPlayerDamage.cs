using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI.Example;

namespace EmeraldAI
{
    //This script will automatically be added to player targets. You can customize the DamagePlayerStandard function
    //or create your own. Ensure that it will be called within the SendPlayerDamage function. This allows users to customize
    //how player damage is received and applied without having to modify any main system scripts. The EmeraldComponent can
    //be used for added functionality such as only allowing blocking if the received AI is using the Melee Weapon Type.
    public class EmeraldAIPlayerDamage : MonoBehaviour
    {
        public List<string> ActiveEffects = new List<string>();
        public bool IsDead = false;

        public void SendPlayerDamage(int DamageAmount, Transform Target, EmeraldAISystem EmeraldComponent, bool CriticalHit = false)
        {
            //The standard damage function that sends damage to the Emerald AI demo player
            DamagePlayerStandard(DamageAmount);

            //Creates damage text on the player's position, if enabled.
            if (TryGetComponent(out EmeraldAI.Utility.TargetPositionModifier targetPositionModifier))
                CombatTextSystem.Instance.CreateCombatText(DamageAmount, new Vector3(transform.position.x, transform.position.y + targetPositionModifier.PositionModifier, transform.position.z), CriticalHit, false, true);
            else
                CombatTextSystem.Instance.CreateCombatText(DamageAmount, transform.position, CriticalHit, false, true);

            //Sends damage to another function that will then send the damage to the RFPS player.
            //If you are using RFPS, you can uncomment this to allow Emerald Agents to damage your RFPS player.
            //DamageRFPS(DamageAmount, Target);

            //Sends damage to another function that will then send the damage to the Invector player.
            //If you are using Invector, you can uncomment this to allow Emerald Agents to damage your Invector player.
            DamageInvectorPlayer(DamageAmount, Target);

            //Sends damage to another function that will then send the damage to the UFPS player.
            //If you are using UFPS, you can uncomment this to allow Emerald Agents to damage your UFPS player.
            //DamageUFPSPlayer(DamageAmount);
        }

        void DamagePlayerStandard(int DamageAmount)
        {
            if (TryGetComponent(out EmeraldAIPlayerHealth PlayerHealth))
            {
                PlayerHealth.DamagePlayer(DamageAmount);

                if (PlayerHealth.CurrentHealth <= 0)
                {
                    IsDead = true;
                }
            }
        }

        /*
        void DamageRFPS(int DamageAmount, Transform Target)
        {
            if (GetComponent<FPSPlayer>() != null)
            {
                GetComponent<FPSPlayer>().ApplyDamage((float)DamageAmount, Target, true);
            }
        }
        */

        void DamageInvectorPlayer (int DamageAmount, Transform Target)
        {
            if (TryGetComponent(out Invector.vCharacterController.vCharacter character))
            {
                var _Damage = new Invector.vDamage(DamageAmount);
                _Damage.sender = Target;
                _Damage.hitPosition = Target.position;
                
                //Applies damage to Invector and allows its melee weapons to block incoming Emerald AI damage.
                if (TryGetComponent(out Invector.vCharacterController.vMeleeCombatInput PlayerInput))
                {
                    if (PlayerInput.meleeManager)
                    {
                        var MeleeManager = PlayerInput.meleeManager;

                        if (PlayerInput.isBlocking)
                        {
                            var DamageReduction = MeleeManager != null ? MeleeManager.GetDefenseRate() : 0;
                            if (DamageReduction > 0)
                                _Damage.ReduceDamage(DamageReduction);
                        
                            MeleeManager.OnDefense();
                            _Damage.reaction_id = MeleeManager.GetDefenseRecoilID();
                        }
                    }
                }
                
                character.TakeDamage(_Damage);
            }
        }

        /*
        void DamageUFPSPlayer(int DamageAmount)
        {
            if (GetComponent<vp_FPPlayerDamageHandler>())
            {
                GetComponent<vp_FPPlayerDamageHandler>().Damage((float)DamageAmount);
            }
        }
        */
    }
}
