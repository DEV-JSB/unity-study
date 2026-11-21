namespace SkillFeatureStudy
{
    // 장비 하나. TBH 처럼 장비마다 고유 번호(UniqueId)가 있고, 장비 하나에 유니크 모드가 하나 달린다.
    // 영웅은 장비를 끼고(Hero.Equip) 빼며(Hero.Unequip), 뺄 때는 장비 고유 번호로 어느 장비인지 가린다.
    // 같은 유니크가 달린 장비를 두 개 껴도 고유 번호가 달라서 서로 헷갈리지 않는다.
    public class Gear
    {
        // 장비 고유 번호. 장비마다 하나, 겹치지 않는다
        public readonly ulong UniqueId;

        // 이 장비에 달린 유니크 모드
        public readonly EUniqueType UniqueType;

        public Gear(ulong uniqueId, EUniqueType uniqueType)
        {
            UniqueId = uniqueId;
            UniqueType = uniqueType;
        }
    }
}
