using System;
using System.Collections.Generic;
using UnityEngine;


public struct TestStruct
{
    public int X;
    public string TestString;
}
public struct TestStruct2 : IEquatable<TestStruct2>
{
    public int X;
    public string TestString;
    public bool Equals(TestStruct2 other)
    {
        return other.X == X;
    }

    public override int GetHashCode()
    {
        return X;
    }
}



public enum EBoxingTestCase
{
    DirectForeach,
    InterfaceForeach,
    StructKeyLookup,
    StructKeyLookupEquatable,
    SimpleBoxing
}

public class Code : MonoBehaviour
{

    [SerializeField] private EBoxingTestCase _testCase;
    [SerializeField] private int _iterationCount = 10000;

    private Dictionary<TestStruct, int> _dict = new Dictionary<TestStruct, int>();
    private Dictionary<TestStruct2, int> _dict2 = new Dictionary<TestStruct2, int>();

    private object _test;
    
    private void Start()
    {

        for (int i = 0; i < 1000; ++i)
        {
            var testStruct = new TestStruct();
            testStruct.X = i;
            _dict.TryAdd(testStruct, i);
        }
        for (int i = 0; i < 1000; ++i)
        {
            var testStruct = new TestStruct2();
            testStruct.X = i;
            _dict2.TryAdd(testStruct, i);
        }
    }


    private void Update()
    {
        switch (_testCase)
        {
            case EBoxingTestCase.DirectForeach:
                TestDirectForeach();
                break;
            case EBoxingTestCase.InterfaceForeach:
                TestInterfaceForeach();
                break;
            case EBoxingTestCase.StructKeyLookup:
                TestStructKeyLookup();
                break;
            case EBoxingTestCase.StructKeyLookupEquatable:
                TestStructKeyLookup2();
                break;
            
            case EBoxingTestCase.SimpleBoxing:
                SimpleBoxing();
                break;
        }
    }

    private void SimpleBoxing()
    {
        for (int i = 0; i < _iterationCount; ++i)
        {
            _test = i;
        }
    }
    private void TestDirectForeach()
    {
        for (int i = 0; i < 10; ++i)
        {
            
            foreach (var test in _dict)
            {
            
            }
        }
        

    }

    private void TestInterfaceForeach()
    {
        for (int i = 0; i < 10; ++i)
        {
            IEnumerable<KeyValuePair<TestStruct, int>> iterator = _dict;

            foreach (var test in iterator)
            {
            
            }
        }
        
        
    }

    private void TestStructKeyLookup2()
    {

        for (int i = 0; i < 10; ++i)
        {
            var testStruct = new TestStruct2();
            testStruct.X = i;
            _dict2.TryGetValue(testStruct, out int value);
        }
    }
    private void TestStructKeyLookup()
    {

        for (int i = 0; i < 10; ++i)
        {
            var testStruct = new TestStruct();
            testStruct.X = i;
            _dict.TryGetValue(testStruct, out int value);
        }
    }
    
    
}
