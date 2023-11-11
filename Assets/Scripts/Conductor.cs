using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class Conductor : MonoBehaviour
{
    public AudioSource songSource { get; private set; }


    private double initialDspOffset;

    public float songPosition
    {
        get
        {
            if (!BeatmapManager.Instance.playing)
                return 0;
            var position = (float)songSource.timeSamples / (songSource.clip.frequency);
            if (AudioSettings.dspTime - initialDspOffset <
                BeatmapManager.Instance.currentPlayingBeatmap.beforePlayDelay)
            {
                position = (float)(AudioSettings.dspTime - initialDspOffset) -
                           BeatmapManager.Instance.currentPlayingBeatmap.beforePlayDelay;
                position *= songSource.pitch;
            }

            return position;
        }
    }

    public float songPositionInBeats
    {
        get
        {
            return (songPosition - (float)BeatmapManager.Instance.currentPlayingBeatmap.initialOffset) /
                   (float)BeatmapManager.Instance.currentPlayingBeatmap.SecondsPerBeatAt((int)(songPosition * 1000));
        }
    }

    public static Conductor Instance { get; private set; }

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
        Instance = this;
    }

    private void OnEnable()
    {
        InitialiseSingleton();

        songSource = GetComponent<AudioSource>();
        BeatmapManager.Instance.OnPlay += StartSong;
        BeatmapManager.Instance.OnPause += PauseSong;
    }

    private void OnDisable()
    {
        
        BeatmapManager.Instance.OnPlay -= StartSong;
        BeatmapManager.Instance.OnPause -= PauseSong;
    }

    void StartSong()
    {
        initialDspOffset = (float)AudioSettings.dspTime;
        songSource.clip = BeatmapManager.Instance.currentPlayingBeatmap.audioClip;

        songSource.PlayScheduled(initialDspOffset + BeatmapManager.Instance.currentPlayingBeatmap.beforePlayDelay);
    }

    void PauseSong()
    {
        songSource.Pause();
    }
}