using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ColorShapeSO", fileName = "ColorShapeSO")]
public class ColorShapeSO : ScriptableObject
{
    public List<ColorShape> colorShapes = new();

    public Color GetColorByEColorShape(EColorShape eColorShape)
    {
        var colorShape = colorShapes.Find(x => x.eColorShape == eColorShape);

        return colorShape.color;
    }
}

[Serializable]
public class ColorShape
{
    public EColorShape eColorShape;
    public Color color;
}
