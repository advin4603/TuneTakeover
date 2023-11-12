using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public static Countdown Instance { get; private set; }
    public List<Transform> countdownObjects;
    public float countdownScaleSpeed;
    public float countdownWaitDelay;
    private bool isCountingDown = false;
    private int countdownIndex = 0;
    private bool finalScaleDown = false;

    public delegate void FinishCountdownDelegate();

    public FinishCountdownDelegate OnFinishCountdown;
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
        BeatmapManager.Instance.OnResumePressed += StartCountdown;
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnResumePressed -= StartCountdown;
    }

    void StartCountdown()
    {
        countdownIndex = 0;
        ContinueCountdown();
    }

    void ContinueCountdown()
    {
        isCountingDown = true;
    }

    void StartFinalScaleDown()
    {
        finalScaleDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountingDown)
        {
            countdownObjects[countdownIndex].localScale = Vector3.one *
                                                          (countdownObjects[countdownIndex].localScale.x +
                                                           Time.deltaTime * countdownScaleSpeed);
            if (countdownObjects[countdownIndex].localScale.x >= 1)
            {
                countdownObjects[countdownIndex].localScale = Vector3.one;
                countdownIndex++;
                isCountingDown = false;
                Invoke(countdownIndex >= countdownObjects.Count ? "StartFinalScaleDown" : "ContinueCountdown",
                    countdownWaitDelay);
            }
        }
        else if (finalScaleDown)
        {
            foreach (var countdownObject in countdownObjects)
                countdownObject.localScale = Vector3.one *
                                             (countdownObject.localScale.x -
                                              Time.deltaTime * countdownScaleSpeed);


            if (countdownObjects[countdownObjects.Count - 1].localScale.x <= 0)
            {
                foreach (var countdownObject in countdownObjects)
                    countdownObject.localScale = Vector3.zero;

                finalScaleDown = false;
                OnFinishCountdown?.Invoke();
            }
        }
    }
}