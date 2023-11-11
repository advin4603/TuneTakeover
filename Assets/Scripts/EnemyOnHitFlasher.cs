using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorFlash))]
public class EnemyOnHitFlasher : MonoBehaviour
{
    private ColorFlash _colorFlash;
    // Start is called before the first frame update

    void Flash(float t)
    {
        _colorFlash.Flash();
    }
    private void OnEnable()
    {
        _colorFlash = GetComponent<ColorFlash>();
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack += Flash;
    }

    private void OnDisable()
    {
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack -= Flash;
    }
}
