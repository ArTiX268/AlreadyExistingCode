namespace Components.Health
{
    public interface IDamageable
    {
        abstract void TakeDamage(in float damageAmount);

        abstract void RegenHealth(in float regenAmount);

        abstract float GetHealth(in bool asAPercentage = false);
    }
}