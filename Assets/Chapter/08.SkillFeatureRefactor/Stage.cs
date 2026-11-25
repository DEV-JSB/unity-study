namespace SkillFeatureStudy
{
    // 스테이지. 스테이지 시작을 알리기만 한다 (TBH 의 StageManager 자리). 평타 흐름은 Battle 이 맡는다.
    // OnStageStart 는 static 이라, 스테이지 객체를 몰라도 누구나 구독할 수 있다 (지금은 UniqueModHandler 가 사용 기록 비우기에 쓴다).
    public class Stage
    {
        public static readonly Observer<Stage> OnStageStart = new ();

        // 스테이지를 시작한다
        public void StartStage()
        {
            OnStageStart.Notify(this);
        }
    }
}
