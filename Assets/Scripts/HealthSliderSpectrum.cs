using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class HealthSliderSpectrum : MonoBehaviour
{
    public int cutoff = 20;
    public int count = 256;
    public Color leftColor;
    public Color rightColor;
    public GameObject VisualiserBar;
    public Slider healthSlider;
    private (RectTransform, Image)[] bars;
    private RectTransform _rectTransform;


    private float[] spectrumData;
    public float spectrumScale;
    public AnimationCurve scaleCurve;
    private bool updateSpectrum = true;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        float width = _rectTransform.rect.width;
        spectrumData = new float[count];
        int barCount = count - cutoff;
        bars = new (RectTransform, Image)[barCount];
        for (int i = 0; i < barCount; i++)
        {
            var newObject = Instantiate(VisualiserBar, transform);
            bars[i] = (newObject.GetComponent<RectTransform>(), newObject.GetComponent<Image>());
            var localScale = bars[i].Item1.transform.localScale;
            localScale.x = 1f / barCount;
            localScale.y = 0;
            bars[i].Item1.transform.localScale = localScale;
            bars[i].Item1.anchoredPosition = new Vector2(i * width / barCount, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float value = healthSlider.value;
        int barCount = count - cutoff;

        if (updateSpectrum)
        {
            Conductor.Instance.songSource.GetSpectrumData(spectrumData, 0, FFTWindow.Triangle);
            for (int i = 0; i < barCount; i++)
            {
                bars[i].Item2.color = i + 1 > value * barCount ? rightColor : leftColor;
                var localScale = bars[i].Item1.transform.localScale;
                localScale.y = spectrumScale * Mathf.Sqrt(spectrumData[i]) * scaleCurve.Evaluate((float)i / barCount);
                bars[i].Item1.transform.localScale = localScale;
            }
        }
        else
            for (int i = 0; i < barCount; i++) 
                bars[i].Item2.color = i + 1 > value * barCount ? rightColor : leftColor;

        updateSpectrum = !updateSpectrum;
    }
}