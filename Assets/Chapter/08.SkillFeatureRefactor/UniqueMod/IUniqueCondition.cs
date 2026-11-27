using UnityEngine;


namespace SkillFeatureStudy
{
    public interface IUniqueCondition
    {
        bool IsSatisfied(Hero hero, TriggerContext context)
        {
            return false;
        }
    }
}