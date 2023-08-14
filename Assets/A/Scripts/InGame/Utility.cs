using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class Utility
{
    public static T GetEnum<T>(string enumName)
    {
        return (T)Enum.Parse(typeof(T), enumName);
    }

    public static T SelectOne<T>(this List<T> ts)
    {
        return ts[UnityEngine.Random.Range(0, ts.Count)];
    }

    public static void AddListener(this EventTrigger eventTrigger, EventTriggerType type, UnityAction<PointerEventData> callBack)
    {
        var triggerEntry = new EventTrigger.Entry
        {
            eventID = type
        };
        triggerEntry.callback.AddListener((data) => callBack(data as PointerEventData));
        eventTrigger.triggers.Add(triggerEntry);
    }
}