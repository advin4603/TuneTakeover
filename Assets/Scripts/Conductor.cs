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
    public float gameOverSlowSpeed;
    public float PauseSlowSpeed;
    public float ResumeSpeed;


    private bool slowingDown = false;
    private bool resuming = false;
    private float slowDownSpeed;
    public bool hasBeforePlayDelay = true;

    public delegate void FinishedSlowedPauseDelegate();

    public FinishedSlowedPauseDelegate OnFinishSlowedPause;

    public void PlayLoopPreview()
    {
        
        initialDspOffset = (float)AudioSettings.dspTime;
        songSource.clip = BeatmapManager.Instance.currentPlayingBeatmap.audioClip;
        songSource.timeSamples =
            (int)(BeatmapManager.Instance.currentPlayingBeatmap.previewTimeSeconds * songSource.clip.frequency);
        songSource.loop = true;
        songSource.Play();
    }

    private bool finished = false;
    public void Finished()
    {
        BeatmapManager.Instance.Finish();
    }

    private float lastSongPosition = -Mathf.Infinity;
    public float songPosition
    {
        get
        {
            var position = (float)songSource.timeSamples / (songSource.clip.frequency);
            
            if (hasBeforePlayDelay && AudioSettings.dspTime - initialDspOffset <=
                BeatmapManager.Instance.currentPlayingBeatmap.beforePlayDelay)
            {
                position = (float)(AudioSettings.dspTime - initialDspOffset) -
                           BeatmapManager.Instance.currentPlayingBeatmap.beforePlayDelay;
                // position *= songSource.pitch;
            }

            if (lastSongPosition > position && !finished)
            {
                finished = true;
                Finished();
            }
            lastSongPosition = position;
            
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
        InitialiseSingleton();
    }

    private void OnEnable()
    {
        Instance = this;

        songSource = GetComponent<AudioSource>();
        BeatmapManager.Instance.OnPlay += StartSong;
        BeatmapManager.Instance.OnPauseStart += PauseSlowedPause;
        BeatmapManager.Instance.OnResume += SmoothResume;
        BeatmapManager.Instance.OnGameOverStart += GameOverSlowedPause;
    }

    void PauseSlowedPause()
    {
        SlowedPause(PauseSlowSpeed);
    }

    void GameOverSlowedPause()
    {
        SlowedPause(gameOverSlowSpeed);
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnPlay -= StartSong;
        BeatmapManager.Instance.OnPauseStart -= PauseSlowedPause;
        BeatmapManager.Instance.OnResume -= SmoothResume;
        BeatmapManager.Instance.OnGameOverStart -= GameOverSlowedPause;
    }

    void StartSong()
    {
        initialDspOffset = (float)AudioSettings.dspTime;
        songSource.clip = BeatmapManager.Instance.currentPlayingBeatmap.audioClip;
        
        songSource.PlayScheduled(initialDspOffset + BeatmapManager.Instance.currentPlayingBeatmap.beforePlayDelay);
    }

    void SlowedPause(float slowDownSpeed)
    {
        slowingDown = true;
        this.slowDownSpeed = slowDownSpeed;
    }

    void FinishSlowedPause()
    {
        OnFinishSlowedPause?.Invoke();
    }

    void SmoothResume()
    {
        resuming = true;
        songSource.pitch = 0;
        songSource.UnPause(); // TODO fix the click
    }

    private void Update()
    {
        if (slowingDown)
        {
            songSource.pitch -= Time.deltaTime * slowDownSpeed;
            songSource.volume -= Time.deltaTime * slowDownSpeed;
            if (songSource.pitch <= 0 || songSource.volume <= 0)
            {
                songSource.pitch = 0;
                songSource.volume = 0;
                slowingDown = false;
                songSource.Pause();
                FinishSlowedPause();
            }
        }
        else if (resuming)
        {
            songSource.pitch += Time.deltaTime * ResumeSpeed;
            songSource.volume += Time.deltaTime * ResumeSpeed;
            if (songSource.pitch >= 1 || songSource.volume >= 1)
            {
                songSource.volume = 1;
                songSource.pitch = 1;
                resuming = false;
            }
        }
    }
}