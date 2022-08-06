namespace Zer0
{
    public interface IDamagable
    {
        public void TakeDamage(float damageTaken);
        public void RecoverHealth(float healingDone);
        public void Death();
    }
}
