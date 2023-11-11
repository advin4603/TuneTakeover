using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapEventsManager : MonoBehaviour
{

    [Serializable]
    public class EventMapping
    {
        public string eventName;
        public UnityEvent<SongBeatmap.SongHitObject> eventAction;
    }

    public List<EventMapping> eventMap;
    public Dictionary<string, UnityEvent<SongBeatmap.SongHitObject>> eventMapDict;

    private int nextEvent = 0;

    public static BeatmapEventsManager Instance { get; private set; }
    
    void InitialiseSingleton()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = null;
        }
        Instance = this;
    }
    private void Awake()
    {
        InitialiseSingleton();
    }

    private void OnEnable()
    {
        Instance = this;
    }


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
        var songPosition = Conductor.Instance.songPosition * 1000;
        while (nextEvent < BeatmapManager.Instance.currentPlayingBeatmap.hitObjects.Count &&
               BeatmapManager.Instance.currentPlayingBeatmap.hitObjects[nextEvent].time < songPosition)
        {
            var hitObject = BeatmapManager.Instance.currentPlayingBeatmap.hitObjects[nextEvent++];
            foreach (var eventName in BeatmapManager.Instance.currentPlayingBeatmap.commonHitObjectEvent)
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