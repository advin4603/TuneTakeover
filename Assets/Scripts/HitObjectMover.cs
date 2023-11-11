using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObjectMover : MonoBehaviour
{
    Vector3 initialPosition;

   
    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var position = initialPosition;
        position.x += -Conductor.Instance.songPosition * BeatmapManager.Instance.currentPlayingBeatmap.approachRate;
        transform.localPosition = position;
    }

    
}