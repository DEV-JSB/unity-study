using System;
using UnityEngine;

public enum EDeltaMode
{
    Fixed60,        // 매 프레임 1/60초로 고정. 돌릴 때마다 같은 숫자가 나온다
    UnityDeltaTime, // Time.deltaTime 그대로. 실제 게임과 같은 조건
}

// 좌표가 커지면 "더하려던 이동량"과 "실제로 늘어난 좌표"가 얼마나 어긋나는지 잰다.
// 씬에서 부모가 없는(루트) 오브젝트에 붙일 것. 부모가 있으면 행렬 계산 오차가 섞인다.
public class FloatPrecision : MonoBehaviour
{
    [SerializeField] private float _startX = 0f;            // 시작 좌표
    [SerializeField] private float _speed = 3f;             // 초당 이동 거리
    [SerializeField] private EDeltaMode _deltaMode = EDeltaMode.Fixed60;
    [SerializeField] private bool _logEveryFrame = false;   // 프레임마다 한 줄씩 찍기
    [SerializeField] private int _summaryFrames = 60;       // 이 프레임 수마다 요약 한 줄

    private double _requestedSum;   // 요약 구간 동안 더하려던 이동량 합
    private double _actualSum;      // 요약 구간 동안 실제로 늘어난 좌표 합
    private int _stuckFrames;       // 요약 구간 동안 좌표가 전혀 안 변한 프레임 수
    private int _frameCount;

    private void Start()
    {
        transform.position = new Vector3(_startX, 0f, 0f);
    }

    // UGUI 버튼(X = 0)에서 부른다. 좌표를 0으로 돌려서 "작은 좌표에서는 멀쩡한가"를 바로 비교한다.
    public void MoveToZero()
    {
        MoveTo(0f);
    }

    // UGUI 버튼(X = Start X)에서 부른다. Inspector의 Start X 좌표로 다시 보낸다.
    public void MoveToStartX()
    {
        MoveTo(_startX);
    }

    private void MoveTo(float x)
    {
        transform.position = new Vector3(x, 0f, 0f);

        // 옮기기 전 좌표에서 쌓던 요약은 버리고 새로 센다.
        _requestedSum = 0;
        _actualSum = 0;
        _stuckFrames = 0;
        _frameCount = 0;

        Debug.Log($"[위치 변경] x = {Format(x)}");
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (_deltaMode == EDeltaMode.Fixed60)
        {
            deltaTime = 1f / 60f;
        }

        float requested = _speed * deltaTime;               // 더하려던 이동량

        float before = transform.position.x;
        transform.position += new Vector3(requested, 0f, 0f);
        float after = transform.position.x;

        float actual = after - before;                      // 실제로 늘어난 좌표

        _requestedSum += requested;
        _actualSum += actual;
        if (actual == 0f)
        {
            ++_stuckFrames;
        }
        ++_frameCount;

        if (_logEveryFrame)
        {
            Debug.Log($"x {Format(before)} -> {Format(after)} | 요청 {Format(requested)} | 실제 {Format(actual)}");
        }

        if (_frameCount >= _summaryFrames)
        {
            LogSummary(after);
            _requestedSum = 0;
            _actualSum = 0;
            _stuckFrames = 0;
            _frameCount = 0;
        }
    }

    private void LogSummary(float x)
    {
        double ratio = 0;
        if (_requestedSum != 0)
        {
            ratio = _actualSum / _requestedSum * 100.0;
        }

        Debug.Log($"[{_frameCount}프레임 요약] x {Format(x)} | 눈금 간격 {Format(GetGap(x))}"
                  + $" | 요청 합 {Format(_requestedSum)} | 실제 합 {Format(_actualSum)} ({ratio:F1}%)"
                  + $" | 안 움직인 프레임 {_stuckFrames}");
    }

    // x 바로 다음 float까지의 거리. float의 비트를 정수로 꺼내 1을 더하면 바로 다음 float이 된다.
    private static float GetGap(float x)
    {
        float abs = Mathf.Abs(x);
        float next = BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(abs) + 1);
        return next - abs;
    }

    // float을 그대로 문자열로 바꾸면 7자리쯤에서 잘려 보일 수 있다.
    // double로 바꾼 뒤(값은 그대로) 소수 8자리까지 찍는다.
    private static string Format(double value)
    {
        return value.ToString("F8");
    }
}
