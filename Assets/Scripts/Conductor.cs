using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Conductor : MonoBehaviour
{
    public AudioSource songSource;

    public BeatmapManager beatmapManager;

    private double initialDspOffset;

    public float songPosition
    {
        get
        {
            if (!beatmapManager.playing)
                return 0;
            var position = (float)songSource.timeSamples / (songSource.clip.frequency);
            if (AudioSettings.dspTime - initialDspOffset < beatmapManager.currentPlayingBeatmap.beforePlayDelay)
            {
                position = (float)(AudioSettings.dspTime - initialDspOffset) -
                           beatmapManager.currentPlayingBeatmap.beforePlayDelay;
                position *= songSource.pitch;
            }

            return position;
        }
    }

    public float songPositionInBeats
    {
        get
        {
            return (songPosition - (float)beatmapManager.currentPlayingBeatmap.initialOffset) /
                   (float)beatmapManager.currentPlayingBeatmap.SecondsPerBeatAt((int)(songPosition * 1000));
        }
    }

    void StartSong()
    {
        initialDspOffset = (float)AudioSettings.dspTime;
        songSource.clip = beatmapManager.currentPlayingBeatmap.audioClip;

        songSource.PlayScheduled(initialDspOffset + beatmapManager.currentPlayingBeatmap.beforePlayDelay);
    }

    void PauseSong()
    {
        songSource.Pause();
    }

    private void OnEnable()
    {
        songSource = GetComponent<AudioSource>();
        beatmapManager.OnPlay += StartSong;
        beatmapManager.OnPause += PauseSong;
    }

    private void OnDisable()
    {
        beatmapManager.OnPlay -= StartSong;
        beatmapManager.OnPause -= PauseSong;
    }
}