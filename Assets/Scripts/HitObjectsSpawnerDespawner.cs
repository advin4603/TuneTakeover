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

    public delegate void UnneccessaryAttackDelegate();

    public UnneccessaryAttackDelegate OnUneccessaryAttack;
    
    
    public delegate void UnneccessaryDefendDelegate();

    public UnneccessaryDefendDelegate OnUneccessaryDefend;

    public delegate void MissedDefendDelegate();

    public MissedDefendDelegate OnMissedDefend;
    
    public delegate void MissedAttackDelegate();

    public MissedAttackDelegate OnMissedAttack;

    public delegate void SuccessfulAttackDelegate(float hitOffsetNormalised);

    public SuccessfulAttackDelegate OnSuccessfulAttack;

    public delegate void SuccessfulDefendDelegate(float hitOffsetNormalised);

    public SuccessfulDefendDelegate OnSuccessfulDefend;


    private void Attack(InputAction.CallbackContext _)
    {
        if (attackHitObjects.Count == 0)
        {
            UnnecessaryAttack();
            return;
        }
            
        var trackingHitObjectTime = attackHitObjects[attackHitObjects.Count - 1].Item2;
        var songPosition = conductorScript.songPosition * 1000;
        if (songPosition <= trackingHitObjectTime &&
            trackingHitObjectTime - songPosition <= earlyForgivenessTimeMilliseconds)
        {
            // Early but ok
            attackHitObjects[attackHitObjects.Count - 1].Item3.Hit();
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
            SuccessfulAttack((songPosition - trackingHitObjectTime) / earlyForgivenessTimeMilliseconds);
            return;
        }

        if (songPosition >= trackingHitObjectTime &&
            songPosition - trackingHitObjectTime <= lateForgivenessTimeMilliseconds)
        {
            // Late but ok
            attackHitObjects[attackHitObjects.Count - 1].Item3.Hit();
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
            SuccessfulAttack((songPosition - trackingHitObjectTime) / lateForgivenessTimeMilliseconds);
            return;
        }

        UnnecessaryAttack();
    }

    private void Defend(InputAction.CallbackContext _)
    {
        if (defendHitObjects.Count == 0)
        {
            UnnecessaryDefend();
            return;
        }
            
        var trackingHitObjectTime = defendHitObjects[defendHitObjects.Count - 1].Item2;
        var songPosition = conductorScript.songPosition * 1000;
        if (songPosition <= trackingHitObjectTime &&
            trackingHitObjectTime - songPosition <= earlyForgivenessTimeMilliseconds)
        {
            // Early but ok
            defendHitObjects[defendHitObjects.Count - 1].Item3.Hit();
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
            SuccessfulDefend((songPosition - trackingHitObjectTime) / earlyForgivenessTimeMilliseconds);
            return;
        }

        if (songPosition >= trackingHitObjectTime &&
            songPosition - trackingHitObjectTime <= lateForgivenessTimeMilliseconds)
        {
            // Late but ok
            defendHitObjects[defendHitObjects.Count - 1].Item3.Hit();
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
            SuccessfulDefend((songPosition - trackingHitObjectTime) / lateForgivenessTimeMilliseconds);
            return;
        }

        UnnecessaryDefend();
    }

    void SuccessfulAttack(float hitOffsetNormalised)
    {
        OnSuccessfulAttack?.Invoke(hitOffsetNormalised);
    }

    void SuccessfulDefend(float hitOffsetNormalised)
    {
        OnSuccessfulDefend?.Invoke(hitOffsetNormalised);
    }


    void UnnecessaryAttack()
    {
        OnUneccessaryAttack?.Invoke();
    }

    void UnnecessaryDefend()
    {
        OnUneccessaryDefend?.Invoke();
    }

    void MissedDefend()
    {
        OnMissedDefend?.Invoke();
    }

    void MissedAttack()
    {
        OnMissedAttack?.Invoke();
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
        while (attackHitObjects.Count > 0 && songPosition - lateForgivenessTimeMilliseconds >
               attackHitObjects[attackHitObjects.Count - 1].Item2)
        {
            MissedAttack();
            Destroy(attackHitObjects[attackHitObjects.Count - 1].Item1, despawnDelayMilliseconds/1000f);
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
        }

        while (defendHitObjects.Count > 0 && songPosition - lateForgivenessTimeMilliseconds >
               defendHitObjects[defendHitObjects.Count - 1].Item2)
        {
            MissedDefend();
            Destroy(defendHitObjects[defendHitObjects.Count - 1].Item1, despawnDelayMilliseconds/1000f);
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
        }
    }
}