using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObjectMover : MonoBehaviour
{
    public Conductor conductorScript;
    
    public BeatmapManager beatmapManagerScript;
    public Vector3 initialPosition;

   
    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var position = initialPosition;
        position.x += -conductorScript.songPosition * beatmapManagerScript.currentPlayingBeatmap.approachRate;
        transform.localPosition = position;
    }

    
}