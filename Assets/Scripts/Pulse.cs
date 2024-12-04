using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float pulseFactor;

    private int previousBeat = -1;

    private Vector3 originalScale;

    private float t = 0;
    public float pulseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        int new_beat = (int)Conductor.Instance.SongPositionInBeats(true, false, true);
        if (new_beat >= 0)
        {
            if (previousBeat != new_beat)
                t = 1;
            previousBeat = new_beat;
        }

        transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseFactor, t);
        t -= Time.deltaTime * pulseSpeed *
             (float)BeatmapManager.Instance.currentPlayingBeatmap.SecondsPerBeatAt(
                 (int)(Conductor.Instance.SongPosition(true, false, true) * 1000));
    }
}