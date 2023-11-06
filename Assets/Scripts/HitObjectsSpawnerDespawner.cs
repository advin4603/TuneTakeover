using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitObjectsSpawnerDespawner : MonoBehaviour
{
    public Conductor conductorScript;
    public GameObject attackHitObject;
    public GameObject defendHitObject;

    public InputActionReference attackActionReference;
    public InputActionReference defendActionReference;


    public BeatmapManager beatmapManagerScript;

    private List<(GameObject, float, HitObjectDespawner)> attackHitObjects =
        new List<(GameObject, float, HitObjectDespawner)>();

    private List<(GameObject, float, HitObjectDespawner)> defendHitObjects =
        new List<(GameObject, float, HitObjectDespawner)>();


    public int lateForgivenessTimeMilliseconds;
    public int earlyForgivenessTimeMilliseconds;
    public int despawnDelayMilliseconds;

    private void Attack(InputAction.CallbackContext _)
    {
        var trackingHitObjectTime = attackHitObjects[attackHitObjects.Count - 1].Item2;
        var songPosition = conductorScript.songPosition * 1000;
        if (songPosition <= trackingHitObjectTime &&
            trackingHitObjectTime - songPosition <= earlyForgivenessTimeMilliseconds)
        {
            // Early but ok
            attackHitObjects[attackHitObjects.Count - 1].Item3.Hit();
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
            return;
        }

        if (songPosition >= trackingHitObjectTime &&
            songPosition - trackingHitObjectTime <= lateForgivenessTimeMilliseconds)
        {
            // Late but ok
            attackHitObjects[attackHitObjects.Count - 1].Item3.Hit();
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
            return;
        }

        UnnecessaryAttack();
    }

    private void Defend(InputAction.CallbackContext _)
    {
        var trackingHitObjectTime = defendHitObjects[defendHitObjects.Count - 1].Item2;
        var songPosition = conductorScript.songPosition * 1000;
        if (songPosition <= trackingHitObjectTime &&
            trackingHitObjectTime - songPosition <= earlyForgivenessTimeMilliseconds)
        {
            // Early but ok
            defendHitObjects[defendHitObjects.Count - 1].Item3.Hit();
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
            return;
        }

        if (songPosition >= trackingHitObjectTime &&
            songPosition - trackingHitObjectTime <= lateForgivenessTimeMilliseconds)
        {
            // Late but ok
            defendHitObjects[defendHitObjects.Count - 1].Item3.Hit();
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
            return;
        }

        UnnecessaryDefend();
    }

    void UnnecessaryAttack()
    {
    }

    void UnnecessaryDefend()
    {
    }

    private void OnEnable()
    {
        beatmapManagerScript.OnBeforeStartPlay += SpawnHitObjects;
        attackActionReference.action.started += Attack;
        defendActionReference.action.started += Defend;

        attackActionReference.action.Enable();
        defendActionReference.action.Enable();
    }

    private void OnDisable()
    {
        beatmapManagerScript.OnBeforeStartPlay -= SpawnHitObjects;
        attackActionReference.action.started -= Attack;
        defendActionReference.action.started -= Defend;


        attackActionReference.action.Disable();
        defendActionReference.action.Disable();
    }

    void SpawnHitObjects()
    {
        foreach (var hitObject in beatmapManagerScript.currentPlayingBeatmap.hitObjects)
        {
            GameObject spawnedHitObject;
            if (hitObject.hitObjectType == SongBeatmap.SongHitObject.SongHitObjectType.Attack)
            {
                spawnedHitObject = Instantiate(attackHitObject, transform);
                attackHitObjects.Add((spawnedHitObject, hitObject.time,
                    spawnedHitObject.GetComponent<HitObjectDespawner>()));
            }
            else
            {
                spawnedHitObject = Instantiate(defendHitObject, transform);
                defendHitObjects.Add((spawnedHitObject, hitObject.time,
                    spawnedHitObject.GetComponent<HitObjectDespawner>()));
            }

            var position = spawnedHitObject.transform.localPosition;
            position.x += hitObject.time / 1000 * beatmapManagerScript.currentPlayingBeatmap.approachRate;
            spawnedHitObject.transform.localPosition = position;
        }

        attackHitObjects.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        defendHitObjects.Sort((a, b) => b.Item2.CompareTo(a.Item2));
    }

    private void Update()
    {
        var songPosition = conductorScript.songPosition * 1000;
        while (attackHitObjects.Count > 0 && songPosition - lateForgivenessTimeMilliseconds - despawnDelayMilliseconds >
               attackHitObjects[attackHitObjects.Count - 1].Item2)
        {
            Destroy(attackHitObjects[attackHitObjects.Count - 1].Item1);
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
        }

        while (defendHitObjects.Count > 0 && songPosition - lateForgivenessTimeMilliseconds - despawnDelayMilliseconds >
               defendHitObjects[defendHitObjects.Count - 1].Item2)
        {
            Destroy(defendHitObjects[defendHitObjects.Count - 1].Item1);
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
        }
    }
}