using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyOnHitObjectAttacker))]
public class EnemyOnHitObjectAttacker : MonoBehaviour
{
    private AttackDefendInvoker _attackDefendInvoker;

    void DefendOnUnnecessaryAttack()
    {
        _attackDefendInvoker.Defend();
    }

    private void OnEnable()
    {
        HitObjectsSpawnerDespawner.Instance.OnUneccessaryAttack += DefendOnUnnecessaryAttack;
        BeatmapEventsManager.Instance.eventMapDict.TryAdd("Enemy Beat Action",
            new UnityEvent<SongBeatmap.SongHitObject>());
        BeatmapEventsManager.Instance.eventMapDict["Enemy Beat Action"].AddListener(BeatAction);
    }

    private void OnDisable()
    {
        HitObjectsSpawnerDespawner.Instance.OnUneccessaryAttack -= DefendOnUnnecessaryAttack;
    }

    private void Start()
    {
        _attackDefendInvoker = GetComponent<AttackDefendInvoker>();
    }

    public void BeatAction(SongBeatmap.SongHitObject songHitObject)
    {
        if (songHitObject.hitObjectType != SongBeatmap.SongHitObject.SongHitObjectType.Defend)
            return;

        _attackDefendInvoker.Attack();
    }
}