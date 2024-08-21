using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Screw3DSO", fileName = "Screw3DSO")]
public class Screw3DSO : ScriptableObject
{
    public Screw3DData[] datas = new Screw3DData[Enum.GetValues(typeof(EScrewColor)).Length];

    public bool IsValid()
    {
        return datas.Length == Enum.GetValues(typeof(EScrewColor)).Length - 1;
    }

    public Material GetMaterial(EScrewColor color)
    {
        return Array.Find(datas, x => x.eScrewColor == color).material;
    }
}

[Serializable]
public class Screw3DData
{
    public EScrewColor eScrewColor;
    public Material material;
}
