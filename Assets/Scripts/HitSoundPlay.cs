using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundPlay : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool onAttack;
    [SerializeField] private bool onDefend;

    [SerializeField] private AudioSource sound;

    private void OnEnable()
    {
        if (onAttack)
            HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack += Play;
        if (onDefend)
            HitObjectsSpawnerDespawner.Instance.OnSuccessfulDefend += Play;
    }

    private void OnDisable()
    {
        if (HitObjectsSpawnerDespawner.Instance != null)
        {
            if (onAttack)
                HitObjectsSpawnerDespawner.Instance.OnSuccessfulAttack -= Play;
            if (onDefend)
                HitObjectsSpawnerDespawner.Instance.OnSuccessfulDefend -= Play;
        }
    }

    // Update is called once per frame
    void Play(float _)
    {
        sound.Play();
    }
}