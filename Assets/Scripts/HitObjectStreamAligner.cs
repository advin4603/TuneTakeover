using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HitObjectStreamAligner : MonoBehaviour
{
    public Transform alignTo;

    private void Update()
    {
        var eulerAngles = transform.eulerAngles;
        transform.LookAt(alignTo);
        eulerAngles.y = transform.eulerAngles.y - 90;
        transform.eulerAngles = eulerAngles;
    }

}
