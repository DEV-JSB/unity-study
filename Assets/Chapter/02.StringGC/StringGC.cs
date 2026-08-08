using System;
using System.Text;
using UnityEngine;

public enum EStringTestCase
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
}


public class StringGC : MonoBehaviour
{
    [SerializeField] private EStringTestCase _testCase;
    [SerializeField] private int _iterationCount;
    [SerializeField] private int _case5Frame;
    private string _str;
    private int _frameCount = 0;
    private StringBuilder _builder = new();

    private int _lastValue;
    private void Update()
    {
        switch (_testCase)
        {
            case EStringTestCase.Zero:
                for (int i = 0; i < _iterationCount; ++i)
                {
                    _str = "Score : " + i.ToString();
                }
                break;
            case EStringTestCase.One:
                for (int i = 0; i < _iterationCount; ++i)
                {
                    _str = "Score : " + i;
                }
                break;
            case EStringTestCase.Two:
                for (int i = 0; i < _iterationCount; ++i)
                {
                    _str = $"Score : {i}";
                }
                break;
            case EStringTestCase.Three:
                for (int i = 0; i < _iterationCount; ++i)
                {
                    _str = string.Format("Score : {0}", i);
                }
                break;
            case EStringTestCase.Four:
                for (int i = 0; i < _iterationCount; ++i)
                {
                    _builder.Clear();
                    _str = _builder.Append("Score : ").Append(i).ToString();
                }
                break;
            case EStringTestCase.Five:
                for (int i = 0; i < _iterationCount; ++i)
                {
                    int value = i / 60;
                    if (value != _lastValue)
                    {
                        _str = "Score : " + value;
                        _lastValue = value;
                    }
                }
                break;
            
            
            
        }
    }
}
