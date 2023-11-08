using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmitOnSuccess : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    public HitObjectsSpawnerDespawner hitObjectsSpawnerDespawner;
    public Vector2 successWindow;

    void Emit(float offset)
    {
        if (successWindow.x <= Mathf.Abs(offset) && Mathf.Abs(offset) < successWindow.y)
            _particleSystem.Emit(1);
    }

    private void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        hitObjectsSpawnerDespawner.OnSuccessfulAttack += Emit;
        hitObjectsSpawnerDespawner.OnSuccessfulDefend += Emit;
    }

    private void OnDisable()
    {
        hitObjectsSpawnerDespawner.OnSuccessfulAttack -= Emit;
        hitObjectsSpawnerDespawner.OnSuccessfulDefend -= Emit;
    }
}