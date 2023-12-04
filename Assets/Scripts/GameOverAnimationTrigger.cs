using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class GameOverAnimationTrigger : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        BeatmapManager.Instance.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnGameOver -= GameOver;
    }

    void GameOver()
    {
        _animator.SetTrigger("GameOver");
    }
}