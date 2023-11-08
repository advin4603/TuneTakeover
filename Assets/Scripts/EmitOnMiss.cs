using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitOnMiss : MonoBehaviour
{
    public bool attack;
    public bool defend;
    private ParticleSystem _particleSystem;
    public HitObjectsSpawnerDespawner hitObjectsSpawnerDespawner;

    void Emit()
    {
        _particleSystem.Emit(1);
    }

    private void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (defend)
            hitObjectsSpawnerDespawner.OnMissedDefend += Emit;
        if (attack)
            hitObjectsSpawnerDespawner.OnMissedAttack += Emit;
    }

    private void OnDisable()
    {
        if (defend)
            hitObjectsSpawnerDespawner.OnMissedDefend -= Emit;
        if (attack)
            hitObjectsSpawnerDespawner.OnMissedAttack -= Emit;
    }
}