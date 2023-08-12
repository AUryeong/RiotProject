using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class Utility
{
    public static T SelectOne<T>(this List<T> ts)
    {
        return ts[Random.Range(0, ts.Count)];
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