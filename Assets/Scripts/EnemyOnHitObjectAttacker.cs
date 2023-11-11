using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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