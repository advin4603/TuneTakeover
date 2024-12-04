using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitObjectsSpawnerDespawner : MonoBehaviour
{
    public GameObject attackHitObject;
    public GameObject defendHitObject;

    public InputActionReference attackActionReference;
    public InputActionReference defendActionReference;


    private List<(GameObject, float, HitObjectDespawner, HitObjectMover)> attackHitObjects =
        new List<(GameObject, float, HitObjectDespawner, HitObjectMover)>();

    private List<(GameObject, float, HitObjectDespawner, HitObjectMover)> defendHitObjects =
        new List<(GameObject, float, HitObjectDespawner, HitObjectMover)>();


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

    public static HitObjectsSpawnerDespawner Instance { get; private set; }

    void InitialiseSingleton()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = null;
        }

        Instance = this;
    }

    private void Awake()
    {
        InitialiseSingleton();
    }


    private void Attack(InputAction.CallbackContext _)
    {
        if (attackHitObjects.Count == 0)
        {
            UnnecessaryAttack();
            return;
        }

        var trackingHitObjectTime = attackHitObjects[attackHitObjects.Count - 1].Item2;
        var songPosition = Conductor.Instance.SongPosition(false, true, true) * 1000;
        if (songPosition <= trackingHitObjectTime &&
            trackingHitObjectTime - songPosition <=
            BeatmapManager.Instance.currentPlayingBeatmap.earlyForgivenessTimeMilliseconds)
        {
            // Early but ok
            attackHitObjects[attackHitObjects.Count - 1].Item3.Hit();
            attackHitObjects[attackHitObjects.Count - 1].Item4.enabled = false;
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
            SuccessfulAttack((songPosition - trackingHitObjectTime) /
                             BeatmapManager.Instance.currentPlayingBeatmap.earlyForgivenessTimeMilliseconds);
            return;
        }

        if (songPosition >= trackingHitObjectTime &&
            songPosition - trackingHitObjectTime <=
            BeatmapManager.Instance.currentPlayingBeatmap.lateForgivenessTimeMilliseconds)
        {
            // Late but ok
            attackHitObjects[attackHitObjects.Count - 1].Item3.Hit();
            attackHitObjects[attackHitObjects.Count - 1].Item4.enabled = false;
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
            SuccessfulAttack((songPosition - trackingHitObjectTime) /
                             BeatmapManager.Instance.currentPlayingBeatmap.lateForgivenessTimeMilliseconds);
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
        var songPosition = Conductor.Instance.SongPosition(false, true, true) * 1000;
        if (songPosition <= trackingHitObjectTime &&
            trackingHitObjectTime - songPosition <=
            BeatmapManager.Instance.currentPlayingBeatmap.earlyForgivenessTimeMilliseconds)
        {
            // Early but ok
            defendHitObjects[defendHitObjects.Count - 1].Item3.Hit();
            defendHitObjects[defendHitObjects.Count - 1].Item4.enabled = false;
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
            SuccessfulDefend((songPosition - trackingHitObjectTime) /
                             BeatmapManager.Instance.currentPlayingBeatmap.earlyForgivenessTimeMilliseconds);
            return;
        }

        if (songPosition >= trackingHitObjectTime &&
            songPosition - trackingHitObjectTime <=
            BeatmapManager.Instance.currentPlayingBeatmap.lateForgivenessTimeMilliseconds)
        {
            // Late but ok
            defendHitObjects[defendHitObjects.Count - 1].Item3.Hit();
            defendHitObjects[defendHitObjects.Count - 1].Item4.enabled = false;
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
            SuccessfulDefend((songPosition - trackingHitObjectTime) /
                             BeatmapManager.Instance.currentPlayingBeatmap.lateForgivenessTimeMilliseconds);
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

    void Pause()
    {
        attackActionReference.action.started -= Attack;
        defendActionReference.action.started -= Defend;
    }

    void Resume()
    {
        attackActionReference.action.started += Attack;
        defendActionReference.action.started += Defend;
    }

    private void OnEnable()
    {
        Instance = this;

        BeatmapManager.Instance.OnBeforeStartPlay += SpawnHitObjects;
        attackActionReference.action.started += Attack;
        defendActionReference.action.started += Defend;

        BeatmapManager.Instance.OnPause += Pause;
        BeatmapManager.Instance.OnResume += Resume;

        BeatmapManager.Instance.OnGameOverStart += Pause;
        BeatmapManager.Instance.OnFinish += Pause;

        attackActionReference.action.Enable();
        defendActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (BeatmapManager.Instance != null)
            BeatmapManager.Instance.OnBeforeStartPlay -= SpawnHitObjects;
        attackActionReference.action.started -= Attack;
        defendActionReference.action.started -= Defend;


        BeatmapManager.Instance.OnGameOverStart -= Pause;
        BeatmapManager.Instance.OnFinish -= Pause;

        BeatmapManager.Instance.OnPause -= Pause;
        BeatmapManager.Instance.OnResume -= Resume;

        attackActionReference.action.Disable();
        defendActionReference.action.Disable();
    }

    void SpawnHitObjects()
    {
        foreach (var hitObject in BeatmapManager.Instance.currentPlayingBeatmap.hitObjects)
        {
            GameObject spawnedHitObject;
            if (hitObject.hitObjectType == SongBeatmap.SongHitObject.SongHitObjectType.Attack)
            {
                spawnedHitObject = Instantiate(attackHitObject, transform);

                var mover = spawnedHitObject.GetComponent<HitObjectMover>();
                attackHitObjects.Add((spawnedHitObject, hitObject.time,
                    spawnedHitObject.GetComponent<HitObjectDespawner>(),
                    mover));
                mover.approachRateMultiplier = hitObject.approachMultiplier > 0 ? hitObject.approachMultiplier : 1;
                var position = spawnedHitObject.transform.localPosition;
                position.x += hitObject.time / 1000 * BeatmapManager.Instance.currentPlayingBeatmap.approachRate * mover.approachRateMultiplier;
                spawnedHitObject.transform.localPosition = position;
            }
            else if (hitObject.hitObjectType == SongBeatmap.SongHitObject.SongHitObjectType.Defend)
            {
                spawnedHitObject = Instantiate(defendHitObject, transform);
                var mover = spawnedHitObject.GetComponent<HitObjectMover>();

                mover.approachRateMultiplier = hitObject.approachMultiplier > 0 ? hitObject.approachMultiplier : 1;
                defendHitObjects.Add((spawnedHitObject, hitObject.time,
                    spawnedHitObject.GetComponent<HitObjectDespawner>(),
                    mover));

                var position = spawnedHitObject.transform.localPosition;
                position.x += hitObject.time / 1000 * BeatmapManager.Instance.currentPlayingBeatmap.approachRate * mover.approachRateMultiplier;
                spawnedHitObject.transform.localPosition = position;
            }
        }

        attackHitObjects.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        defendHitObjects.Sort((a, b) => b.Item2.CompareTo(a.Item2));
    }

    private void Update()
    {
        var songPosition = Conductor.Instance.SongPosition(false, true, true) * 1000;
        while (attackHitObjects.Count > 0 &&
               songPosition - BeatmapManager.Instance.currentPlayingBeatmap.lateForgivenessTimeMilliseconds >
               attackHitObjects[attackHitObjects.Count - 1].Item2)
        {
            MissedAttack();
            Destroy(attackHitObjects[attackHitObjects.Count - 1].Item1,
                BeatmapManager.Instance.currentPlayingBeatmap.despawnDelayMilliseconds / 1000f);
            attackHitObjects.RemoveAt(attackHitObjects.Count - 1);
        }

        while (defendHitObjects.Count > 0 &&
               songPosition - BeatmapManager.Instance.currentPlayingBeatmap.lateForgivenessTimeMilliseconds >
               defendHitObjects[defendHitObjects.Count - 1].Item2)
        {
            MissedDefend();
            Destroy(defendHitObjects[defendHitObjects.Count - 1].Item1,
                BeatmapManager.Instance.currentPlayingBeatmap.despawnDelayMilliseconds / 1000f);
            defendHitObjects.RemoveAt(defendHitObjects.Count - 1);
        }
    }
}