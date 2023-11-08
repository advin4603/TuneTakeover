using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnHitColorFlasher : MonoBehaviour
{
    public HitObjectsSpawnerDespawner hitObjectsSpawnerDespawner;
    private ColorFlash _colorFlash;
    // Start is called before the first frame update

    void Flash()
    {
        _colorFlash.Flash();
    }
    private void OnEnable()
    {
        _colorFlash = GetComponent<ColorFlash>();
        hitObjectsSpawnerDespawner.OnMissedDefend += Flash;
    }

    private void OnDisable()
    {
        hitObjectsSpawnerDespawner.OnMissedDefend -= Flash;
    }
}
