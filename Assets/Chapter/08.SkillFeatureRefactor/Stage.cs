namespace SkillFeatureStudy
{
    // 리팩토링 "전" 스테이지. 스테이지 시작을 맡는다 (TBH 의 StageManager 자리). 평타 흐름은 Battle 이 맡는다.
    public class Stage
    {
        private readonly Hero[] _heroes;

        public Stage(params Hero[] heroes)
        {
            _heroes = heroes;
        }

        // 스테이지를 시작한다
        public void StartStage()
        {
            foreach (var hero in _heroes)
            {
                hero.StartSetting();
            }
        }
    }
}
