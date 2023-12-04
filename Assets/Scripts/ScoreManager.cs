using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreManager : MonoBehaviour
{
    private TextMeshProUGUI text;
    public float score = 0;
    public static ScoreManager Instance;

    public float maxScore = 10_000f;
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
        text = GetComponent<TextMeshProUGUI>();
        
        text.text = $"Score: {score:F2}";
    }

    void IncrementScore(float offsetNormalised)
    {
        score += (1f - (offsetNormalised * offsetNormalised)) * maxScore / BeatmapManager.Instance.currentPlayingBeatmap.hitObjects.Count;

        text.text = $"Score: {score:F2}";
    }
    private void OnEnable()
    {
        Instance = this;
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack += IncrementScore;
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulDefend += IncrementScore;
    }

    private void OnDisable()
    {
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack -= IncrementScore;
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulDefend -= IncrementScore;
    }
}
