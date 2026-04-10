using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Health
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(0), MinValue(0)] private float maxHealth = 100;
        [SerializeField] private bool startAtMaxHealth = true;
        [SerializeField, Min(0), MinValue(0), HideIf("startAtMaxHealth")] private float startingHealth = 100;

        private float health;

        private void Start()
        {
            if (startAtMaxHealth)
            {
                health = startingHealth = maxHealth;
            }
            else health = startingHealth;
        }

        public float GetHealth(in bool asAPercentage = false) => 
            asAPercentage ? health / maxHealth : health;

        public void RegenHealth(in float regenAmount) => 
            health = Mathf.Min(maxHealth, health + regenAmount);

        public void TakeDamage(in float damageAmount) =>
            health = Mathf.Max(0, health - damageAmount);
    }
}