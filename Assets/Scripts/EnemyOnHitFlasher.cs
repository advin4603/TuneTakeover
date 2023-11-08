using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnHitFlasher : MonoBehaviour
{
    public HitObjectsSpawnerDespawner hitObjectsSpawnerDespawner;
    private ColorFlash _colorFlash;
    // Start is called before the first frame update

    void Flash(float t)
    {
        _colorFlash.Flash();
    }
    private void OnEnable()
    {
        _colorFlash = GetComponent<ColorFlash>();
        hitObjectsSpawnerDespawner.OnSuccessfulAttack += Flash;
    }

    private void OnDisable()
    {
        hitObjectsSpawnerDespawner.OnSuccessfulAttack -= Flash;
    }
}
