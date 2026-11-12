using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class RaceCondition : MonoBehaviour
{

    public enum EFixMode
    {
        None,
        Lock,
        LockAllFor,
        InterLocked,
        Semaphore,
        SemaphoreAllFor
    }
    
    [SerializeField] private int _loopCount = 10_000_000;
    [SerializeField] private int _trialCount = 5;
    [SerializeField] private EFixMode _fixMode = EFixMode.None;
    private int _count;

    private readonly object _lock = new();

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    

    private async void Start()
    {
        for (int trial = 1; trial <= _trialCount; ++trial)
        {
            _count = 0;


            System.Action action = null;

            switch (_fixMode)
            {
                
                case EFixMode.Lock:
                    action = AddManyLock;
                    break;
                case EFixMode.InterLocked:
                    action = AddManyInterLock;
                    break;
                case EFixMode.Semaphore:
                    action = AddManySemaphore;
                    break;
                case EFixMode.LockAllFor:
                    action = AddManyLockForAll;
                    break;
                case EFixMode.SemaphoreAllFor:
                    action = AddManySemaphoreAllFor;
                    break;
                default:
                    action = AddMany;
                    break;
            }
            
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Task workerA = Task.Run(action);
            //Task workerB = Task.Run(action);
            Task.WaitAll(workerA/*, workerB*/);
            Debug.Log($"[{trial}회차] 기대값 {_loopCount * 2:N0} / 실제 {_count:N0} / {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    private void AddManyInterLock()
    {
        for (int i = 0; i < _loopCount; ++i)
        {
            Interlocked.Increment(ref _count);
        }
    }

    private void AddManySemaphoreAllFor()
    {

        _semaphore.Wait();
        try
        {
            for (int i = 0; i < _loopCount; ++i)
            {
                _count++;
            }

        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void AddManySemaphore()
    {

        for (int i = 0; i < _loopCount; ++i)
        {
            _semaphore.Wait();
            try
            {
                _count++;

            }
            finally
            {
                _semaphore.Release();

            }
        }
    }

    private void AddManyLockForAll()
    {

        lock (_lock)
        {
            for (int i = 0; i < _loopCount; ++i)
            {
                _count++;
            }

        }

    }

    private void AddManyLock()
    {

        for (int i = 0; i < _loopCount; ++i)
        {
            lock (_lock)
            {
                _count++;
            }
        }
    }

    private void AddMany()
    {
        for (int i = 0; i < _loopCount; ++i)
        {
            _count += 1;
        }
    }

}
