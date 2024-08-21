using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BoxAvailableSO", fileName = "BoxAvailableSO")]
public class BoxAvailableSO : ScriptableObject
{
    public List<BoxSO> boxes = new();

    public BoxSO GetBoxSO(int id)
    {
        return boxes.Find(x => x.id == id);
    }

    public BoxSO GetCurSkin()
    {
        return boxes[SkinSystem.data.boxCollection.curSkin];
    }
}
