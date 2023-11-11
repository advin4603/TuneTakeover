using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthManager : MonoBehaviour
{
    private Slider _slider;
    private int attackCount;

    private int defendCount;
    // Start is called before the first frame update

    void MissedDefend()
    {
        _slider.value -= 1f / defendCount;
    }

    void SuccessfulAttack(float offsetNormalised)
    {
        _slider.value += (1 + (1 - offsetNormalised)) / attackCount;
    }

    void UnnecessaryAttack()
    {
        _slider.value -= 1f / attackCount;
    }

    private void OnEnable()
    {
        HitObjectsSpawnerDespawner.Instance.OnMissedDefend += MissedDefend;
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack += SuccessfulAttack;
        HitObjectsSpawnerDespawner.Instance.OnUneccessaryAttack += UnnecessaryAttack;
    }

    private void OnDisable()
    {
        if (HitObjectsSpawnerDespawner.Instance == null) return;
        HitObjectsSpawnerDespawner.Instance.OnMissedDefend -= MissedDefend;
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack -= SuccessfulAttack;
        HitObjectsSpawnerDespawner.Instance.OnUneccessaryAttack -= UnnecessaryAttack;
    }

    void Start()
    {
        attackCount = 0;
        defendCount = 0;
        foreach (var hitObject in BeatmapManager.Instance.currentPlayingBeatmap.hitObjects)
            if (hitObject.hitObjectType == SongBeatmap.SongHitObject.SongHitObjectType.Attack)
                attackCount++;
            else
                defendCount++;


        _slider = GetComponent<Slider>();
    }
}