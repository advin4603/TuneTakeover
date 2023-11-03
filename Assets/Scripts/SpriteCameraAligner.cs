using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCameraAligner : MonoBehaviour
{
    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        Vector3 awayDirection = transform.position - cameraTransform.position;
        Quaternion awayRotation = Quaternion.LookRotation(awayDirection);

        transform.rotation = awayRotation;
        eulerAngles.y = transform.eulerAngles.y;
        transform.eulerAngles = eulerAngles;
    }
}