using Invector.vMelee;
using UnityEngine;

namespace Zer0
{
    public class PickupWeapon : MonoBehaviour
    {
        [SerializeField] private GameObject leftWeapon;
        [SerializeField] private GameObject rightWeapon;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                if (leftWeapon)
                {
                    leftWeapon.SetActive(true);
                    player.GetComponent<vMeleeManager>().leftWeapon = leftWeapon.GetComponentInChildren<vMeleeWeapon>();
                }
                if (rightWeapon)
                {
                    rightWeapon.SetActive(true);
                    player.GetComponent<vMeleeManager>().rightWeapon = rightWeapon.GetComponentInChildren<vMeleeWeapon>();
                }
                
                player.UpdateWeapons();
            }
        }
    }
}
