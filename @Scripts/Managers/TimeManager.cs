using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeManager : ContentManager {

    #region Const.

    private const int SpriteCount = 9;              // 애니메이션 한 번에 소요되는 프레임.
    private const float InitialFrameSpeed = 2f;     // 초기 FrameSpeed.

    #endregion
    
    #region Properties

    public bool Pause { get; set; }

    // 모든 Belt의 애니메이션을 통일하기 위한 Frame.
    public uint Frame {
        get => _frame;
        set {
            _frame = value;
        }
    }

    // 초당 Belt 등의 애니메이션 재생 횟수, 즉 Belt의 속력.
    public float FrameSpeed {
        get => _frameSpeed;
        set {
            _frameSpeed = value;
            _frameInterval = 1 / (SpriteCount * _frameSpeed);
        }
    }

    public float FrameTimer {
        get => _frameTimer;
        set {
            if (value <= 0) {
                Frame++;
                _frameTimer = _frameInterval;
                return;
            }

            _frameTimer = value;
        }
    }

    #endregion
    
    #region Fields

    private uint _frame;
    private float _frameSpeed;          
    private float _frameInterval;       // 프레임 증가 주기: FrameSpeed를 변경하면 자동으로 계산됨.
    private float _frameTimer;
    
    private Dictionary<string, NyoTimer> _timers = new();

    #endregion

    #region Initialize / Indexer

    public NyoTimer this[string key] => _timers.GetValueOrDefault(key);

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        FrameSpeed = InitialFrameSpeed;
        _frameTimer = _frameInterval;
        
        GameEvents.OnGamePause += () => Pause = true;
        GameEvents.OnGameResume += () => Pause = false;

        return true;
    }

    public override void Clear() {
        // TODO:: NyoTimer 실제 제거해야 함.
        foreach (NyoTimer timer in _timers.Values) timer.Clear();
        _timers.Clear();
        Pause = false;
    }
    
    #endregion

    public NyoTimer NewTimer(string key, float time, bool isAutoDestroy = false, bool start = true, bool loop = false) {
        if (_timers.TryGetValue(key, out NyoTimer timer)) timer.Clear();
        NyoTimer newTimer = new NyoTimer(key, time, isAutoDestroy, start, loop);
        _timers[key] = newTimer;
        newTimer.OnDestroy += t => {
            _timers.Remove(key);
        };
        return newTimer;
    }

    public void AddAllTime(float time, string[] excludes = null) {
        IEnumerable<NyoTimer> timers =
            excludes == null ? _timers.Values : _timers.Values.Where(t => !excludes.Contains(t.Key));
        foreach (NyoTimer timer in timers) timer.Add(time);
    }
    
    public void OnUpdate(float deltaTime) {
        if (Pause) return;
        FrameTimer -= deltaTime;
        if (GameScene.GameState != GameState.Playing) return;

        List<NyoTimer> timers = new(_timers.Values);
        foreach (NyoTimer timer in timers) {
            timer.OnUpdate(deltaTime);
        }
    }

}

public class NyoTimer {

    public string Key { get; private set; }

    public float Time { get; private set; }
    public float Current {
        get => _current;
        set {
            _current = value;
            OnChangedTime?.Invoke(this);

            if (_current <= 0) {
                OnTimeEnd?.Invoke(this);
                if (_loop) Current = Time;
                else if (_isAutoDestroy) OnDestroy?.Invoke(this);
            }
        }
    }
    public bool Pause { get; set; }

    private float _current;
    private bool _loop;             // 반복 여부.
    private bool _isAutoDestroy;             // 
    private float _secondOffset;    // 초 단위까지 남은 시간.

    public event Action<NyoTimer> OnChangedTime;
    public event Action<NyoTimer> OnSecond;
    public event Action<NyoTimer> OnTimeEnd;
    public event Action<NyoTimer> OnDestroy;

    public NyoTimer(string key, float time, bool isAutoDestroy = false, bool start = true, bool loop = false) {
        Key = key;
        Time = time;
        Pause = !start;
        _loop = loop;
        _isAutoDestroy = isAutoDestroy;
        
        _current = Time;
        _secondOffset = Time % 1;
    }

    public void Add(float time) {
        Current += time;
        OnSecond?.Invoke(this);
    }

    public void OnUpdate(float deltaTime) {
        if (Pause || Current <= 0) return;

        Current -= deltaTime;
        _secondOffset -= deltaTime;

        if (_secondOffset <= 0) {
            OnSecond?.Invoke(this);
            _secondOffset = 1;
        }
    }

    public void Owari() {
        _loop = false;
        Current = 0;
    }

    public void Clear() {
        _loop = false;
        _current = 0;
    }
    
    #region GetString

    public string GetTimeString() {
        int time = (int)Current;
        int min = time / 60;
        int sec = time % 60;
        return $"{min:00}:{sec:00}";
    }

    public string GetSecondString() {
        return $"{(int)Current}";
    }
    
    #endregion
    
} 