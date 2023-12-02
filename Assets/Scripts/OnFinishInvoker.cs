using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFinishInvoker : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Finish()
    {
        _animator.SetTrigger("ResultsShow");
    }
    private void OnEnable()
    {
        BeatmapManager.Instance.OnFinish += Finish;
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnFinish -= Finish;
    }
}
