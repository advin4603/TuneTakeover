using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationSync : MonoBehaviour
{
    private Animator _animator;

    public BeatmapManager beatmapManager;

    public Conductor conductor;

    //Records the animation state or animation that the Animator is currently in
    public AnimatorStateInfo animatorStateInfo;

//Used to address the current state within the Animator using the Play() function
    public int idleState;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        //Load the animator attached to this object
        _animator = GetComponent<Animator>();

        //Get the info about the current animator state
        animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        //Convert the current state name to an integer hash for identification
        idleState = animatorStateInfo.fullPathHash;
    }

    // Update is called once per frame
    void Update()
    {
        float positionInBeats = conductor.songPositionInBeats;
        if (positionInBeats >= 0)
            _animator.Play(idleState, -1, positionInBeats - (int)positionInBeats);
        
    }
}