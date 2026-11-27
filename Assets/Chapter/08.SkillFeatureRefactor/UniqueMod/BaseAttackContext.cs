


namespace SkillFeatureStudy
{
 
    public class BaseAttackContext : TriggerContext
    {

        public bool IsCritical;
        public BaseAttackContext(bool isCritical)
        {
            IsCritical = isCritical;
        }

    }
}
