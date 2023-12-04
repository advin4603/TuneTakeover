using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackHintTrigger : MonoBehaviour
{
    private Animator _animator;

    public string animatorboolName;

    public InputActionReference ActionReference;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInParent<Animator>();
    }

    private void OnEnable()
    {
        ActionReference.action.Enable();
    }

    private void OnDisable()
    {
        ActionReference.action.Disable();
    }

    void HideHint(InputAction.CallbackContext _)
    {
        ActionReference.action.performed -= HideHint;
        Conductor.Instance.songSource.Play();
        
        _animator.SetBool(animatorboolName, false);
    }
    public void Trigger(SongBeatmap.SongHitObject _)
    {
        ActionReference.action.performed += HideHint;
        Conductor.Instance.songSource.Pause();
        _animator.SetBool(animatorboolName, true);
    }
}