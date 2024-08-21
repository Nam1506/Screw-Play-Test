using UnityEngine;

public class Hole : MonoBehaviour
{
    public EHoleType type;
    public Screw screw;

    public bool IsEmpty => screw == null;
}
