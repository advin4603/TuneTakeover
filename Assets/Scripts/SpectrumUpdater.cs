using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumUpdater : MonoBehaviour
{
    private float[] spectrumData;
    private float[] rawSpectrumData;
    private float[] buffer;
    private float[] bufferDecrease;
    public HealthManager healthManager;
    public AnimationCurve scaleCurve;

    public Material material;

    public int spectrumCount = 1024;

    public int cutoffCount = 800;

    public int barCount;

    public float decreaseStart = 0.005f;
    public float decreaseMultiplier = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        barCount = spectrumCount - cutoffCount;
        spectrumData = new float[spectrumCount];
        rawSpectrumData = new float[spectrumCount];
        buffer = new float[spectrumCount];
        bufferDecrease = new float[spectrumCount];
    }

    // Update is called once per frame
    void Update()
    {
        Conductor.Instance.songSource.GetSpectrumData(rawSpectrumData, 0, FFTWindow.BlackmanHarris);
        for (int i = 0; i < 1024; i++)
        {
            if (rawSpectrumData[i] >= buffer[i])
            {
                buffer[i] = rawSpectrumData[i];
                bufferDecrease[i] = decreaseStart;
            }
            else
            {
                buffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= decreaseMultiplier;
            }
            spectrumData[i] = Mathf.Sqrt(buffer[i]) * scaleCurve.Evaluate(Mathf.Min((float)i / barCount, 1f));
        }

        // Debug.Log(spectrumData[100]);
        material.SetFloatArray("_Spectrum", spectrumData);
        material.SetFloat("_Health", healthManager.health);
    }
}