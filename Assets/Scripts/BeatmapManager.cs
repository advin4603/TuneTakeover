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

    public delegate void FinishDelegate();

    public FinishDelegate OnFinish;

    public delegate void PauseDelegate();

    public PauseDelegate OnPauseStart;
    public PauseDelegate OnPause;

    public delegate void ResumeDelegate();

    public ResumeDelegate OnResume;
    public ResumeDelegate OnResumePressed;

    public delegate void BeforeStartPlayDelegate();

    public BeforeStartPlayDelegate OnBeforeStartPlay;

    public bool playing = false;
    public bool disablePause = false;

    public bool paused = false;
    public bool gameOverStarted = false;

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
        if (!disablePause)
        {
            PauseActionReference.action.Enable();
            PauseActionReference.action.performed += PausePressHandler;
        }
    }

    private void OnDisable()
    {
        if (disablePause) return;
        PauseActionReference.action.Disable();

        PauseActionReference.action.performed -= PausePressHandler;
    }

    void PausePressHandler(InputAction.CallbackContext _)
    {
        PauseStart();
    }

    void ResumePressHandler(InputAction.CallbackContext _)
    {
        ResumePressed();
    }

    public void ResumePressed()
    {
        PauseActionReference.action.performed -= ResumePressHandler;
        Countdown.Instance.OnFinishCountdown += Resume;
        OnResumePressed?.Invoke();
    }

    void Resume()
    {
        paused = false;
        Countdown.Instance.OnFinishCountdown -= Resume;
        PauseActionReference.action.performed += PausePressHandler;
        OnResume?.Invoke();
    }

    void PauseStart()
    {
        paused = true;
        OnPauseStart?.Invoke();
        Conductor.Instance.OnFinishSlowedPause += Pause;

        PauseActionReference.action.performed -= PausePressHandler;
    }

    public void Finish()
    {
        playing = false;
        PauseActionReference.action.performed -= PausePressHandler;
        OnFinish?.Invoke();
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
        gameOverStarted = true;
        PauseActionReference.action.performed -= PausePressHandler;
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