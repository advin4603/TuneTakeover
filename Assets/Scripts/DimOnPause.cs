using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DimOnPause : MonoBehaviour
{
    public Color dimColor;
    public float dimSpeed;
    private bool dimming = false;
    private float t = 1;


    private Image _image;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        BeatmapManager.Instance.OnPauseStart += Dim;
        BeatmapManager.Instance.OnResumePressed += Undim;
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnPauseStart -= Dim;
        BeatmapManager.Instance.OnResumePressed -= Undim;
    }

    void Dim()
    {
        dimming = true;
    }

    void Undim()
    {
        dimming = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dimming)
            t -= Time.deltaTime * dimSpeed;
        else
            t += Time.deltaTime * dimSpeed;

        t = Mathf.Clamp(t, 0, 1);
        _image.color = Color.Lerp(dimColor, Color.clear, t);
    }
}