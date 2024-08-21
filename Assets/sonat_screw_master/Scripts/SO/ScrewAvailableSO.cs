using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ScrewAvailableSO", fileName = "ScrewAvailableSO")]
public class ScrewAvailableSO : ScriptableObject
{
    public List<ScrewSO> screws = new();

    public ScrewSO GetScrewSO(int id)
    {
        return screws.Find(x => x.id == id);
    }

    public ScrewSO GetCurSkin()
    {
        return screws[SkinSystem.data.screwCollection.curSkin];
    }
}
