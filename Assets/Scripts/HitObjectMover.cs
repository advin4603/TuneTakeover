using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObjectMover : MonoBehaviour
{
    Vector3 initialPosition;
    public float approachRateMultiplier;

   
    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var position = initialPosition;
        position.x += -Conductor.Instance.SongPosition(true, false, true) * BeatmapManager.Instance.currentPlayingBeatmap.approachRate * approachRateMultiplier;
        transform.localPosition = position;
    }

    
}