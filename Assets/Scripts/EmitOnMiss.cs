using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EmitOnMiss : MonoBehaviour
{
    public bool attack;
    public bool defend;
    private ParticleSystem _particleSystem;

    void Emit()
    {
        _particleSystem.Emit(1);
    }

    private void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (defend)
            HitObjectsSpawnerDespawner.Instance.OnMissedDefend += Emit;
        if (attack)
            HitObjectsSpawnerDespawner.Instance.OnMissedAttack += Emit;
    }

    private void OnDisable()
    {
        if (HitObjectsSpawnerDespawner.Instance != null)
        {
            if (defend)
                HitObjectsSpawnerDespawner.Instance.OnMissedDefend -= Emit;
            if (attack)
                HitObjectsSpawnerDespawner.Instance.OnMissedAttack -= Emit;
        }
    }
}