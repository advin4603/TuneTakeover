using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class OnPauseSetTrigger : MonoBehaviour
{
    private Animator _animator;
    private int triggerHash;

    void Pause()
    {
        _animator.SetBool(triggerHash, true);
    }

    void Resume()
    {
        _animator.SetBool(triggerHash, false);
    }
    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        triggerHash = Animator.StringToHash("Paused");
        BeatmapManager.Instance.OnPauseStart += Pause;
        BeatmapManager.Instance.OnResumePressed += Resume;
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnPauseStart -= Pause;
        BeatmapManager.Instance.OnResumePressed -= Resume;
    }
    
}
