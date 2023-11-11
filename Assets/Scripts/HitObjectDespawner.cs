using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitObjectDespawner : MonoBehaviour
{
    public bool isHit;
    private SpriteRenderer[] _childSpriteRenderers;
    public float fadeOutSpeed;
    private float t = 1;
    private SpriteRenderer _spriteRenderer;
    public AnimationCurve fadeOutCurve;
    public AnimationCurve scaleOutCurve;
    private Vector3 localScale;
    private bool impendingDoom = false; // whether the object is set to be detroyed
    private void Start()
    {
        localScale = transform.localScale;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Hit()
    {
        isHit = true;
        transform.SetParent(transform.parent.parent);
    }

    private void Update()
    {
        if (!isHit || impendingDoom)
            return;
        var color = _spriteRenderer.color;
        float curveT = fadeOutCurve.Evaluate(t);
        color.a = curveT;
        _spriteRenderer.color = color;
        transform.localScale = localScale * scaleOutCurve.Evaluate(t);
        
        foreach (var childSpriteRenderer in _childSpriteRenderers)
        {
            var childColor = childSpriteRenderer.color;
            childColor.a = curveT;
            childSpriteRenderer.color = childColor;
        }
        t -= Time.deltaTime * fadeOutSpeed;
        if (t <= 0)
        {
            impendingDoom = true;
            Destroy(gameObject, 1f/fadeOutSpeed);
        }
            
    }
}