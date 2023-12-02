using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorFlash : MonoBehaviour
{
    public Color flashColor;
    public float flashSpeed;
    public AnimationCurve flashCurve;
    private Color originalColor;

    private SpriteRenderer _spriteRenderer;
    private float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = _spriteRenderer.color;
    }

    public void Flash()
    {
        t = 1;
    }

    // Update is called once per frame
    void Update()
    {
        _spriteRenderer.material.SetColor("_Color", Color.Lerp(originalColor, flashColor, flashCurve.Evaluate(t)));
        if (t > 0)
            t -= Time.deltaTime * flashSpeed;
        else
            t = 0;
    }
}