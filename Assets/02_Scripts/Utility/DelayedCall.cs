using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionTimePair
{
    public Action action;
    public float tiggerTime;

    public ActionTimePair(Action action, float tiggerTime)
    {
        this.action = action;
        this.tiggerTime = tiggerTime;
    }
}

public class DelayedCall : Singleton<DelayedCall>
{
    private List<ActionTimePair> actionTimePair = new List<ActionTimePair>();
    private List<LoopAction> loopActions = new List<LoopAction>();
    private float currentTimer = 0f;

    public void AddTimer(Action action, float time)
    {
        actionTimePair.Add(new ActionTimePair(action, currentTimer + time));
    }

    public void AddLoop(Action action, float interval)
    {
        loopActions.Add(new LoopAction(action, interval, currentTimer + interval));
    }

    public void RemoveLoop(Action action)
    {
        loopActions.RemoveAll(l => l.action == action);
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;

        actionTimePair
            .Where(a => currentTimer >= a.tiggerTime)
            .ToList()
            .ForEach(a => a.action?.Invoke());

        actionTimePair.RemoveAll(a => currentTimer >= a.tiggerTime);

        foreach (var loop in loopActions.ToList())
        {
            if (currentTimer >= loop.nextTriggerTime)
            {
                loop.action?.Invoke();
                loop.nextTriggerTime = currentTimer + loop.interval;
            }
        }
    }

    private class ActionTimePair
    {
        public Action action;
        public float tiggerTime;

        public ActionTimePair(Action action, float tiggerTime)
        {
            this.action = action;
            this.tiggerTime = tiggerTime;
        }
    }

    private class LoopAction
    {
        public Action action;
        public float interval;
        public float nextTriggerTime;

        public LoopAction(Action action, float interval, float nextTriggerTime)
        {
            this.action = action;
            this.interval = interval;
            this.nextTriggerTime = nextTriggerTime;
        }
    }
}

