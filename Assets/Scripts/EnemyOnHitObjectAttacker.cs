using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnHitObjectAttacker : MonoBehaviour
{
    private AttackDefendInvoker _attackDefendInvoker;

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