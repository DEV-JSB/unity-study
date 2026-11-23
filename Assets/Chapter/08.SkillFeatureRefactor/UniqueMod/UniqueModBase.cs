namespace SkillFeatureStudy
{
    public abstract class UniqueModBase
    {
        protected Hero _ownerHero;


        public virtual void Attach(Hero hero)
        {
            _ownerHero = hero;
            SubscribeEvents(hero);
        }

        public virtual void Detach()
        {
            UnsubscribeEvents(_ownerHero);
            _ownerHero = null;
        }

        protected abstract void UnsubscribeEvents(Hero hero);
        protected abstract void SubscribeEvents(Hero hero);
    }
}


