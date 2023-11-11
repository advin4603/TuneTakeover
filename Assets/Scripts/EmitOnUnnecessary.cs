using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class EmitOnUnnecessary : MonoBehaviour
{
    // Start is called before the first frame update
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
        if (attack)
            HitObjectsSpawnerDespawner.Instance.OnUneccessaryAttack += Emit;
        if (defend)
            HitObjectsSpawnerDespawner.Instance.OnUneccessaryDefend += Emit;
    }

    private void OnDisable()
    {
        if (HitObjectsSpawnerDespawner.Instance == null) return;
        if (attack)
            HitObjectsSpawnerDespawner.Instance.OnUneccessaryAttack -= Emit;
        if (defend)
            HitObjectsSpawnerDespawner.Instance.OnUneccessaryDefend -= Emit;
    }
}