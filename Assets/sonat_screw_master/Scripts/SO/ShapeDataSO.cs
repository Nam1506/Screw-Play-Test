using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ShapeDataSO", fileName = "ShapeDataSO")]
public class ShapeDataSO : ScriptableObject
{
    public List<ShapeSO> shapes;

    public ShapeSO GetShapeVisual(int id)
    {
        return shapes.Find(x => x.id == id);
    }
}

[Serializable]
public class ShapeSO
{
    public int id = 0;

    public Sprite fill;
    public Sprite shadow;
    public Sprite light;

    public Vector3 posShadow;
}