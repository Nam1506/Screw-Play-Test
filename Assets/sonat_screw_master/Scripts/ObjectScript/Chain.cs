using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    public Screw centerScrew;
    public Screw linkedScrew;

    public void Set(Screw centerScrew, Screw linkedScrew)
    {
        this.centerScrew = centerScrew;  
        this.linkedScrew = linkedScrew;

        ChainGenerator.Instance.GenerateChain(centerScrew.transform, linkedScrew.transform, this);

        transform.SetLocalPositionZ(-centerScrew.shape.Layer);
    }

    public void Clear()
    {
        //centerScrew = null;
        //linkedScrew = null;

        //List<Transform> children = new List<Transform>();

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    children.Add(transform.GetChild(i));
        //}

        //foreach (Transform child in children)
        //{
        //    ClearChild(child);        
        //}

        //PoolBoss.Despawn(transform);
        ChainGenerator.Instance.chains.Remove(this);

        Destroy(gameObject);
    }

    public void Cut()
    {
        PoolBoss.Despawn(transform.GetChild(transform.childCount - 1));

        //Destroy(transform.GetChild(transform.childCount - 1).GetComponent<HingeJoint2D>());
        //transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);

        StartCoroutine(IeDestroySelf());
    }

    private IEnumerator IeDestroySelf()
    {
        yield return new WaitForSeconds(0.75f);

        transform.GetChild(1).GetComponent<HingeJoint2D>().enabled = false;
        //Destroy(transform.GetChild(1).GetComponent<HingeJoint2D>());
        //PoolBoss.Despawn(transform.GetChild(0));
        ClearChild(transform.GetChild(0));

        //Destroy(transform.GetChild(1).GetComponent<HingeJoint2D>());
        //transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
            }           
        }

        Clear();
    }

    private void ClearChild(Transform child)
    {
        //if (child.TryGetComponent(out HingeJoint2D hingeJoint))
        //{
        //    hingeJoint.enabled = false;
        //    //Destroy(hingeJoint);
        //}
        //if (child.TryGetComponent(out FixedJoint2D fixedJoint))
        //{
        //    fixedJoint.enabled = false;
        //    //Destroy(fixedJoint);
        //}
        ////if (child.TryGetComponent(out Rigidbody2D rigidbody))
        ////{
        ////    rigidbody.constraints = RigidbodyConstraints2D.None;
        ////}
        //child.localEulerAngles = Vector3.zero;

        //PoolBoss.Despawn(child);
        Destroy(child.gameObject);
    }
}