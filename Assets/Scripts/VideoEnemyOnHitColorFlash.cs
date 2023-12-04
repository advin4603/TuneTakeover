using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoEnemyOnHitColorFlash : MonoBehaviour
{
    private VideoColorFlash _colorFlash;
    // Start is called before the first frame update

    void Flash(float t)
    {
        _colorFlash.Flash();
    }
    private void OnEnable()
    {
        _colorFlash = GetComponent<VideoColorFlash>();
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack += Flash;
    }

    private void OnDisable()
    {
        HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack -= Flash;
    }
}
