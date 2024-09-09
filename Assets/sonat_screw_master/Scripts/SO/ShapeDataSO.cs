using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ShapeDataSO", fileName = "ShapeDataSO")]
public class ShapeDataSO : ScriptableObject
{
    public List<ShapeSO> shapes = new();

    public ShapeSO GetShapeVisual(int id)
    {
        if (id < 0 || id >= shapes.Count) return null;

        return shapes[id];
    }

    public int GetId(ShapeSO shape)
    {
        return shapes.FindIndex(x => x == shape);
    }
}

[Serializable]
public class ShapeSO
{
    public Sprite fill;

    [HideInInspector]
    public Sprite shadow;
    [HideInInspector]
    public Sprite light;

    [HideInInspector]
    public Vector3 posShadow;
}