using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatmapManager : MonoBehaviour
{
    public SongBeatmap currentPlayingBeatmap;

    public delegate void PlayDelegate();

    public PlayDelegate OnPlay;

    public delegate void PauseDelegate();

    public PauseDelegate OnPause;

    public delegate void BeforeStartPlayDelegate();

    public BeforeStartPlayDelegate OnBeforeStartPlay;

    public bool playing = false;

    public static BeatmapManager Instance { get; private set; }

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



    // Start is called before the first frame update
    void Start()
    {
        StartPlay();
    }


    void StartPlay()
    {
        OnBeforeStartPlay?.Invoke();
        Play();
    }

    void Play()
    {
        playing = true;
        OnPlay?.Invoke();
    }

    void Pause()
    {
        playing = false;
        OnPause?.Invoke();
    }
}