using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapEventsManager : MonoBehaviour
{
    public Conductor conductorScript;

    public BeatmapManager beatmapManager;

    [Serializable]
    public class EventMapping
    {
        public string eventName;
        public UnityEvent<SongBeatmap.SongHitObject> eventAction;
    }

    public List<EventMapping> eventMap;
    public Dictionary<string, UnityEvent<SongBeatmap.SongHitObject>> eventMapDict;

    private int nextEvent = 0;

    private void Start()
    {
        eventMapDict = new Dictionary<string, UnityEvent<SongBeatmap.SongHitObject>>();
        foreach (var mapping in eventMap)
        {
            eventMapDict.Add(mapping.eventName, mapping.eventAction);
        }
    }


    // Update is called once per frame
    void Update()
    {
        var songPosition = conductorScript.songPosition * 1000;
        while (nextEvent < beatmapManager.currentPlayingBeatmap.hitObjects.Count &&
               beatmapManager.currentPlayingBeatmap.hitObjects[nextEvent].time < songPosition)
        {
            var hitObject = beatmapManager.currentPlayingBeatmap.hitObjects[nextEvent++];
            foreach (var eventName in beatmapManager.currentPlayingBeatmap.commonHitObjectEvent)
            {
                eventMapDict[eventName]?.Invoke(hitObject);
            }

            foreach (var eventName in hitObject.hitObjectEvent)
            {
                eventMapDict[eventName]?.Invoke(hitObject);
            }
        }
    }
}