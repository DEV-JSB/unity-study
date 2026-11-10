using UnityEngine;

// 측정과는 상관없는 "눈으로 보기" 전용 스크립트. Main Camera에 붙인다.
// 1) 카메라가 대상을 따라간다.
// 2) 월드의 정수 좌표(…, 999999, 1000000, 1000001, …)마다 눈금 막대를 세운다.
//    대상이 실제로 움직이면 눈금이 왼쪽으로 흘러가고, 좌표가 안 변하면 눈금도 멈춘다.
public class FloatPrecisionView : MonoBehaviour
{
    [SerializeField] private Transform _target;                           // 따라갈 대상 (Mover)
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, -10f); // 대상 기준 카메라 위치
    [SerializeField] private int _markerCount = 31;                       // 화면에 깔아 둘 눈금 개수

    private Transform[] _markers;

    private void Start()
    {
        _markers = new Transform[_markerCount];
        for (int i = 0; i < _markerCount; ++i)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "Marker";
            _markers[i] = marker.transform;
        }
    }

    // 대상이 Update에서 움직인 뒤에 따라가도록 LateUpdate를 쓴다.
    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = _target.position + _offset;

        // 눈금은 월드에 박혀 있는 것처럼 보여야 하므로 항상 정수 좌표에 놓는다.
        // 무한히 깔 수는 없으니, 대상 주변의 정수 좌표로 매 프레임 다시 옮긴다.
        long centerX = (long)Mathf.Floor(_target.position.x);
        int half = _markerCount / 2;

        for (int i = 0; i < _markerCount; ++i)
        {
            long worldX = centerX + i - half;

            // 5의 배수 좌표는 긴 눈금, 나머지는 짧은 눈금
            float height = 0.4f;
            if (worldX % 5 == 0)
            {
                height = 1f;
            }

            // 눈금 윗면을 대상(지름 1짜리 Sphere) 바닥 바로 아래에 맞춘다.
            _markers[i].position = new Vector3(worldX, -0.6f - height * 0.5f, 0f);
            _markers[i].localScale = new Vector3(0.1f, height, 0.1f);
        }
    }
}
