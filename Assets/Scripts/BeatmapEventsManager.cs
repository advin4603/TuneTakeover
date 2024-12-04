using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapEventsManager : MonoBehaviour
{

    
    public SerializedDictionary<string, UnityEvent<SongBeatmap.SongHitObject>> eventMapDict;

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
    


    // Update is called once per frame
    void FixedUpdate()
    {
        var songPosition = Conductor.Instance.SongPosition(false, false, false) * 1000;
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