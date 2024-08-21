using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScrew : MonoBehaviour
{
    private Transform _transform;

    [SerializeField]
    private int abc;

    public Vector3 rotateVector = new Vector3(0, 0, 120);

    public bool canRotate = true;

    public void Start()
    {
        _transform = transform;
    }

    private void OnEnable()
    {
        _transform = transform;
        canRotate = true;
        //   _transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canRotate) return;

        _transform.localEulerAngles += Time.unscaledDeltaTime * rotateVector;
    }
}
