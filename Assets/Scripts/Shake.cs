using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public Conductor conductorScript;
    private int oldBeat = -1;
    private Rigidbody _rigidbody;
    public float jumpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        int beatCount = (int)conductorScript.songPositionInBeats;
        if (oldBeat != beatCount)
        {
            oldBeat = beatCount;
            var velocity = _rigidbody.velocity;
            velocity.y = jumpSpeed;
            _rigidbody.velocity = velocity;
        }
    }
}