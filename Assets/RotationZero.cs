using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationZero : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
