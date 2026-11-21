namespace SkillFeatureStudy
{
    // 리팩토링 "전" 유니크 목록. TBH 의 EGearUniqueMod 를 아주 작게 줄인 것.
    // enum 멤버 하나 = 유니크 하나.
    //
    // 작업자가 채울 곳: 아래 세 유니크의 동작. 어느 파일 어디에 넣을지도 작업자가 정한다.
    // 정답은 UniqueBehaviourChecker 의 시나리오 2~6 이다.
    public enum EUniqueType
    {
        // A. 스테이지당 한 번, 쓰러질 피해를 받으면 체력 1로 버틴다.
        //    같은 스테이지에서 뺐다 다시 껴도 다시 쓸 수 없다. 새 스테이지가 시작되면 다시 쓸 수 있다
        SurviveLethalOncePerStage,

        // B. 평타 다섯 번째마다 치명타 (피해 2배)
        EveryFifthBaseAttackCritical,

        // C. 받는 피해 10% 감소. 두 개 끼면 20%.
        //    A 와 같이 끼면 감소가 먼저고, 쓰러지나 판정은 감소한 피해로 한다
        DamageTakenReduction10,
    }
}
