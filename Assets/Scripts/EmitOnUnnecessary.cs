using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitOnUnnecessary : MonoBehaviour
{
    // Start is called before the first frame update
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
        if (attack)
            hitObjectsSpawnerDespawner.OnUneccessaryAttack += Emit;
        if (defend)
            hitObjectsSpawnerDespawner.OnUneccessaryDefend += Emit;
    }

    private void OnDisable()
    {
        if (attack)
            hitObjectsSpawnerDespawner.OnUneccessaryAttack -= Emit;
        if (defend)
            hitObjectsSpawnerDespawner.OnUneccessaryDefend -= Emit;
    }
}