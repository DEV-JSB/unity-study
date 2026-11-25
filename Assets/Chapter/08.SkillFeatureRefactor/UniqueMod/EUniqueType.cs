namespace SkillFeatureStudy
{
    // 유니크 목록. TBH 의 EGearUniqueMod 를 아주 작게 줄인 것. enum 값 하나 = 게임에 있는 유니크 하나.
    // 장비에 달려 있고(Gear.UniqueType), 장비를 끼면 UniqueModHandler.EquipUniqueMod 가 enum 값에 맞는 유니크 모드 객체를 만든다.
    // 동작은 UniqueMod 폴더의 각 class 에 있고, 정답은 UniqueBehaviourChecker 의 시나리오 2~8 이다.
    public enum EUniqueType
    {
        // 치명상 버티기 (검사기의 A) → SurviveLethalMod
        // 스테이지당 한 번, 쓰러질 피해를 받으면 체력 1로 버틴다.
        // 같은 스테이지에서 뺐다 다시 껴도 다시 쓸 수 없다. 새 스테이지가 시작되면 다시 쓸 수 있다
        SurviveLethalOncePerStage,

        // 다섯 번째 평타 치명타 (검사기의 B) → EveryNthBaseAttackCriticalMod(5)
        // 평타 다섯 번째마다 치명타 (피해 2배)
        EveryFifthBaseAttackCritical,

        // 받는 피해 감소 (검사기의 C) → DamageTakenReductionMod(0.1f)
        // 받는 피해 10% 감소. 두 개 끼면 20%.
        // 치명상 버티기와 같이 끼면 감소가 먼저고, 쓰러지나 판정은 감소한 피해로 한다
        DamageTakenReduction10,
    }
}
