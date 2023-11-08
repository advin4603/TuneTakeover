using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private Slider _slider;
    public HitObjectsSpawnerDespawner hitObjectsSpawnerDespawner;
    public BeatmapManager beatmapManager;
    private int attackCount;
    private int defendCount;
    // Start is called before the first frame update

    void MissedDefend()
    {
        _slider.value += 1f / defendCount;
    }

    void SuccessfulAttack(float offsetNormalised)
    {
        _slider.value -= (1 + (1 - offsetNormalised)) / attackCount;
    }

    void UnnecessaryAttack()
    {
        _slider.value += 1f / attackCount;
    }

    private void OnEnable()
    {
        hitObjectsSpawnerDespawner.OnMissedDefend += MissedDefend;
        hitObjectsSpawnerDespawner.OnSuccessfulAttack += SuccessfulAttack;
        hitObjectsSpawnerDespawner.OnUneccessaryAttack += UnnecessaryAttack;
    }

    private void OnDisable()
    {
        hitObjectsSpawnerDespawner.OnMissedDefend -= MissedDefend;
        hitObjectsSpawnerDespawner.OnSuccessfulAttack -= SuccessfulAttack;
        hitObjectsSpawnerDespawner.OnUneccessaryAttack -= UnnecessaryAttack;
    }

    void Start()
    {
        attackCount = 0;
        defendCount = 0;
        foreach (var hitObject in beatmapManager.currentPlayingBeatmap.hitObjects)
            if (hitObject.hitObjectType == SongBeatmap.SongHitObject.SongHitObjectType.Attack)
                attackCount++;
            else
                defendCount++;


        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}