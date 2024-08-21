using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeHole : Hole
{
    [SerializeField] private SpriteRenderer circle;

    public int identify;

    public EScrewColor eScrewColor;

    public void SetMaterial(int layer)
    {
        circle.sharedMaterial = PoolingManager.Instance.GetHoleMaterial(layer);
    }
}
