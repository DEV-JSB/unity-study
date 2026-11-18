# 07. CompareTo — List.Sort에 비교 넘기기

> 정리 노트: (아직 없음)

- 씬: 따로 없음. 아무 씬에 빈 GameObject를 만들고 `CompareToChecker`를 붙인다
- 스크립트: `ItemComparisons.cs` — `items.Sort(비교 메서드)`에 넘길 비교 메서드 3개를 채운다 (낮은 순 / 높은 순 / 높은 순 + 이름 순)
- 스크립트: `SortItem.cs` — `ItemGrade` enum과 `SortItem` 클래스. `items.Sort()`(아무것도 안 넘김)가 쓰는 `CompareTo`를 채운다
- 스크립트: `CompareToChecker.cs` — Play 하거나 컴포넌트 메뉴 "정렬 검사 실행"
