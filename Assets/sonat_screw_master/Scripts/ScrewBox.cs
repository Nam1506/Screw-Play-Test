using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewBox : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.y < 0)
        {
            PoolBoss.Despawn(this.transform);
        }
    }
}
