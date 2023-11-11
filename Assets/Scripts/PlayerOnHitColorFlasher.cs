using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorFlash))]
public class PlayerOnHitColorFlasher : MonoBehaviour
{
    private ColorFlash _colorFlash;
    // Start is called before the first frame update

    void Flash()
    {
        _colorFlash.Flash();
    }

    private void OnEnable()
    {
        _colorFlash = GetComponent<ColorFlash>();
        HitObjectsSpawnerDespawner.Instance.OnMissedDefend += Flash;
    }

    private void OnDisable()
    {
        if (HitObjectsSpawnerDespawner.Instance != null)
            HitObjectsSpawnerDespawner.Instance.OnMissedDefend -= Flash;
    }
}