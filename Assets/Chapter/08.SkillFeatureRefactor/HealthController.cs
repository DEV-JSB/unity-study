using UnityEngine;

namespace SkillFeatureStudy
{
    // 체력 하나를 맡는다. 최대 체력, 현재 체력, 0 하한, 쓰러짐 판정.
    // 체력이 바뀌면 HpChanged 로 알리고(UI 가 구독), 0이 되면 Died 로 알린다(주인이 구독).
    // 주인이 누군지(Hero 인지 몬스터인지) 모른다. 그래서 어느 쪽에도 붙일 수 있다.
    public class HealthController
    {
        public float MaxHp { get; private set; }
        public float CurrentHp { get; private set; }
        public bool IsDead => CurrentHp <= 0f;

        public readonly Observer<HealthController> HpChanged = new();
        public readonly Observer<HealthController> Died = new();

        public HealthController(float maxHp)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }

        // 계산이 끝난 피해를 받는다. 피해 계산(감소, 버티기)은 여기서 하지 않는다
        public void ApplyDamage(float finalDamage)
        {
            if (IsDead || finalDamage <= 0f)
            {
                return;
            }

            CurrentHp = Mathf.Max(0f, CurrentHp - finalDamage);
            HpChanged.Notify(this);

            if (IsDead)
            {
                Died.Notify(this);
            }
        }

        // 체력을 회복한다. 최대 체력을 넘지 않는다. 쓰러진 뒤에는 회복하지 않는다
        public void Heal(float healAmount)
        {
            if (IsDead || healAmount <= 0f)
            {
                return;
            }

            CurrentHp = Mathf.Min(MaxHp, CurrentHp + healAmount);
            HpChanged.Notify(this);
        }
    }
}
