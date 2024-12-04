using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shake : MonoBehaviour
{
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
        int beatCount = (int)Conductor.Instance.SongPositionInBeats(true, false, true);
        if (oldBeat != beatCount)
        {
            oldBeat = beatCount;
            var velocity = _rigidbody.velocity;
            velocity.y = jumpSpeed;
            _rigidbody.velocity = velocity;
        }
    }
}