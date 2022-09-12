using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer {
    private UnityAction toInvoke;
    private float seconds;
    private bool oneshot;

    private bool enabled = true;
    private float timeElapsed = 0;
    
    public Timer(UnityAction toInvoke, float seconds, bool oneshot) {
        this.toInvoke = toInvoke;
        this.seconds = seconds;
        this.oneshot = oneshot;
    }
    internal void Update(float elapsed) {
        if (!enabled) { return; }

        timeElapsed += elapsed;
        if (timeElapsed >= seconds) {
            if (oneshot) { enabled = false; }
            timeElapsed = 0;
            toInvoke.Invoke(); 
        }
    }
    public void Reset() {
        enabled = true;
        timeElapsed = 0;
    }
}

public class TimerDriver : MonoBehaviour
{
    private List<Timer> timers;

    public void CreateTimer(float seconds, UnityAction call, bool oneshot = false) {
        timers.Add(new Timer(call, seconds, oneshot));
    }

    void Awake() {
        timers = new List<Timer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Timer timer in timers) {
            timer.Update(Time.fixedDeltaTime);
        }
    }

    public void ResetAllTimers() {
        foreach (Timer timer in timers) {
            timer.Reset();
        }
    }
}