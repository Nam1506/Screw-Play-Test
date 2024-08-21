using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateBounds : SingletonBase<CalculateBounds>
{
    private Bounds _bounds;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }

    public void SetBound(GameObject obj)
    {
        _bounds = Calculate(obj);
    }

    public Bounds Calculate(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds;
        }
        else
        {
            Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);

            foreach (Renderer childRenderer in obj.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(childRenderer.bounds);
            }

            return bounds;
        }
    }
}
