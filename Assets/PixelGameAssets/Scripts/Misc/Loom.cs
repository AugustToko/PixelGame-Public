using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class Loom : MonoBehaviour
{
    public static readonly int maxThreads = 8;
    static int _numThreads;
    private static Loom _current;
    private int _count;

    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    private void Awake()
    {
        _current = this;
        _initialized = true;
        
        DontDestroyOnLoad(this);
    }

    static bool _initialized;

    static void Initialize()
    {
        if (!_initialized)
        {
            if (!Application.isPlaying)
                return;
            _initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }
    }

    private readonly List<Action> _actions = new List<Action>();

    public struct DelayedQueueItem
    {
        public float Time;
        public Action Action;
    }

    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }

    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem {Time = Time.time + time, Action = action});
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (_numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }

        Interlocked.Increment(ref _numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action) action)();
        }
        catch
        {
            // ignored
        }
        finally
        {
            Interlocked.Decrement(ref _numThreads);
        }
    }

    void OnDisable()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

//    // Use this for initialization
//    void Start()
//    {
//    }

    readonly List<Action> _currentActions = new List<Action>();

    // Update is called once per frame
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }

        foreach (var a in _currentActions)
        {
            a();
        }

        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.Time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }

        foreach (var delayed in _currentDelayed)
        {
            delayed.Action();
        }
    }
}