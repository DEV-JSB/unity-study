using System.Collections.Generic;

namespace SkillFeatureStudy
{
    // 리팩토링 "전" 영웅. 장착 목록과 기본 피해 처리만 있다.
    // 유니크 A·B·C 의 동작은 아직 없다. 작업자가 채운다 (규칙은 UniqueType.cs, 정답은 검사기).
    public class Hero
    {
        public readonly string heroName;
        public readonly float attackPower;

        private readonly List<UniqueType> _equippedUniques = new List<UniqueType>();

        public float CurrentHp { get; private set; }

        public bool IsDead
        {
            get { return CurrentHp <= 0f; }
        }

        public Hero(string heroName, float startHp, float attackPower)
        {
            this.heroName = heroName;
            this.attackPower = attackPower;
            CurrentHp = startHp;
        }

        // 유니크 하나를 낀다. 같은 유니크를 두 번 끼면 목록에 두 개가 들어간다
        public void Equip(UniqueType uniqueType)
        {
            _equippedUniques.Add(uniqueType);
        }

        // 유니크 하나를 뺀다. 같은 유니크가 두 개면 하나만 빠진다
        public void Unequip(UniqueType uniqueType)
        {
            _equippedUniques.Remove(uniqueType);
        }

        public bool HasUnique(UniqueType uniqueType)
        {
            return _equippedUniques.Contains(uniqueType);
        }

        public int CountUnique(UniqueType uniqueType)
        {
            int uniqueCount = 0;
            foreach (UniqueType equippedUnique in _equippedUniques)
            {
                if (equippedUnique == uniqueType)
                {
                    ++uniqueCount;
                }
            }
            return uniqueCount;
        }

        // 피해를 받는다. 체력이 0 이하가 되면 0으로 두고 쓰러진다
        public void TakeDamage(float incomingDamage)
        {
            if (IsDead)
            {
                return;
            }

            if (CurrentHp - incomingDamage > 0f)
            {
                CurrentHp -= incomingDamage;
                return;
            }

            CurrentHp = 0f;
        }
    }
}
