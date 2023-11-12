using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeatmapManager : MonoBehaviour
{
    public InputActionReference PauseActionReference;

    public SongBeatmap currentPlayingBeatmap;

    public delegate void GameOverDelegate();

    public GameOverDelegate OnGameOverStart;

    public GameOverDelegate OnGameOver;

    public delegate void PlayDelegate();

    public PlayDelegate OnPlay;

    public delegate void PauseDelegate();

    public PauseDelegate OnPauseStart;
    public PauseDelegate OnPause;

    public delegate void ResumeDelegate();

    public ResumeDelegate OnResume;
    public ResumeDelegate OnResumePressed;

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
        PauseActionReference.action.Enable();

        PauseActionReference.action.performed += PausePressHandler;
    }

    private void OnDisable()
    {
        PauseActionReference.action.Disable();

        PauseActionReference.action.performed -= PausePressHandler;
    }

    void PausePressHandler(InputAction.CallbackContext _)
    {
        PauseStart();

        PauseActionReference.action.performed -= PausePressHandler;
    }

    void ResumePressHandler(InputAction.CallbackContext _)
    {
        PauseActionReference.action.performed -= ResumePressHandler;
        ResumePressed();
    }

    void ResumePressed()
    {
        Countdown.Instance.OnFinishCountdown += Resume;
        OnResumePressed?.Invoke();
    }

    void Resume()
    {
        Countdown.Instance.OnFinishCountdown -= Resume;
        PauseActionReference.action.performed += PausePressHandler;
        OnResume?.Invoke();
    }

    void PauseStart()
    {
        OnPauseStart?.Invoke();
        Conductor.Instance.OnFinishSlowedPause += Pause;
    }

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
        PauseActionReference.action.performed += ResumePressHandler;
        Conductor.Instance.OnFinishSlowedPause -= Pause;
        playing = false;
        OnPause?.Invoke();
    }

    public void GameOverStart()
    {
        OnGameOverStart?.Invoke();
        Conductor.Instance.OnFinishSlowedPause += GameOver;
    }

    void GameOver()
    {
        Conductor.Instance.OnFinishSlowedPause -= GameOver;
        playing = false;
        OnGameOver?.Invoke();
    }
}