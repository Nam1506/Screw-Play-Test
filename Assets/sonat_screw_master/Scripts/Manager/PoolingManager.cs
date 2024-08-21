using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : SingletonBase<PoolingManager>
{
    [SerializeField] private Material holeMaterial;
    [SerializeField] private Material shapeMaterial;
    [SerializeField] private List<Material> poolHoleMats = new();
    [SerializeField] private List<Material> poolShapeMats = new();

    private const string LAYER_PROP_NAME = "_Stencil";

    public Material GetHoleMaterial(int layer)
    {
        if (poolHoleMats.Count > 0)
        {
            foreach (Material mat in poolHoleMats)
            {
                if (mat.GetInt(LAYER_PROP_NAME) == layer)
                {
                    return mat;
                }
            }

            return InitNewHoleMat(layer);
        }
        else
        {
            return InitNewHoleMat(layer);
        }
    }

    private Material InitNewHoleMat(int layer)
    {
        Material newMat = Instantiate(holeMaterial);

        newMat.SetInt(LAYER_PROP_NAME, layer);

        poolHoleMats.Add(newMat);

        return newMat;
    }

    public Material GetShapeMaterial(int layer)
    {
        if (poolShapeMats.Count > 0)
        {
            foreach (Material mat in poolShapeMats)
            {
                if (mat.GetInt(LAYER_PROP_NAME) == layer)
                {
                    return mat;
                }
            }

            return InitNewShapeMat(layer);
        }
        else
        {
            return InitNewShapeMat(layer);
        }
    }

    private Material InitNewShapeMat(int layer)
    {
        Material newMat = Instantiate(shapeMaterial);

        newMat.SetInt(LAYER_PROP_NAME, layer);

        poolShapeMats.Add(newMat);

        return newMat;
    }
}
