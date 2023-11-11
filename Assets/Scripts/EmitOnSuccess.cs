using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class EmitOnSuccess : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    public Vector2 successWindow;

    void Emit(float offset)
    {
        if (successWindow.x <= Mathf.Abs(offset) && Mathf.Abs(offset) < successWindow.y)
            _particleSystem.Emit(1);
    }

    private void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack += Emit;
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulDefend += Emit;
    }

    private void OnDisable()
    {
        if (HitObjectsSpawnerDespawner.Instance != null)
        {
            HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack -= Emit;
            HitObjectsSpawnerDespawner.Instance.OnSuccessfulDefend -= Emit;
        }
    }
}