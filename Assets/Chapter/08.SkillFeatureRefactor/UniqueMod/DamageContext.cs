
namespace SkillFeatureStudy
{
    public class DamageContext : TriggerContext
    {
        public bool IsBaseAttack;
        public bool IsCritical;
        public float Damage;
        public float DamageDecreaseRatio;
        public float CriticalDamageMultiplier;
        public DamageContext(float damage, bool isCritical, bool isBaseAttack)
        {
            IsBaseAttack = isBaseAttack;
            Damage = damage;
            IsCritical = isCritical;
            CriticalDamageMultiplier = 2f;
        }
    }

    
}
