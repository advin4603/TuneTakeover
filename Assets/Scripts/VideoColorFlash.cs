using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoColorFlash : MonoBehaviour
{
    public Color flashColor;
    public float flashSpeed;
    public AnimationCurve flashCurve;
    private Color originalColor;
    private Material material;
    private float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        originalColor = material.GetColor("_Color");
    }

    public void Flash()
    {
        t = 1;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetColor("_Color", Color.Lerp(originalColor, flashColor, flashCurve.Evaluate(t)));
        if (t > 0)
            t -= Time.deltaTime * flashSpeed;
        else
            t = 0;
    }
}