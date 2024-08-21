using DarkTonic.PoolBoss;
using System.Collections.Generic;
using UnityEngine;

public class ChainGenerator : SingletonBase<ChainGenerator>
{
    public List<Chain> chains;

    public GameObject linkPrefab;
    public float linkLength = 0.1f;

    public float angle = 0f;
    public void GenerateChain(Transform a, Transform b, Chain rope)
    {
        float distance = Vector2.Distance(a.position, b.position);
        int requiredLinks = Mathf.FloorToInt(distance / linkLength);
        int currentLinks = rope.transform.childCount;
        Vector3 direction = (b.position - a.position).normalized;

        GameObject previousLink = null;

        // Remove existing fixed joints if any
        RemoveFixedJoints(rope);

        if (requiredLinks < currentLinks - 1)
        {
            // If chain is too long, remove extra links
            for (int i = currentLinks - 1; i > requiredLinks; i--)
            {
                DestroyImmediate(rope.transform.GetChild(i).gameObject);
            }
        }
        else if (requiredLinks > currentLinks - 1)
        {
            // If chain is too short, add more links
            previousLink = (currentLinks > 0) ? rope.transform.GetChild(currentLinks - 1).gameObject : null;
            for (int i = currentLinks; i <= requiredLinks; i++)
            {
                Vector3 position = a.position + direction * (i * linkLength);

                //Transform link = PoolBoss.Spawn(linkPrefab.transform, position, Quaternion.identity, rope.transform);
                Transform link = Instantiate(linkPrefab, position, Quaternion.identity, rope.transform).transform;

                // Calculate the angle needed to align with the direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Set the rotation only around the Z-axis
                link.rotation = Quaternion.Euler(0, 0, angle);

                Rigidbody2D linkRigidbody = link.GetComponent<Rigidbody2D>();
                if (linkRigidbody != null)
                {
                    // Freeze rotation Z
                    linkRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                }

                // Add Hinge Joint to the link
                AddHingeJoint(previousLink, link.gameObject);

                previousLink = link.gameObject;
            }
        }

        // Update positions of existing links
        for (int i = 0; i < rope.transform.childCount; i++)
        {
            Transform link = rope.transform.GetChild(i);
            Vector3 position = a.position + direction * (i * linkLength);
            link.position = position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            link.rotation = Quaternion.Euler(0, 0, angle);

            link.transform.localPosition = new Vector3(link.transform.localPosition.x, link.transform.localPosition.y, 0f);         
        }

        // Add Fixed Joint for the first and last links  

        if (rope.transform.childCount > 0)
        {
            for (int i = 0; i < rope.transform.childCount; i++)
            {
                Transform child = rope.transform.GetChild(i);

                if (i == 0 || i == rope.transform.childCount - 1)
                {
                    AddFixedJoint(a.gameObject, child.gameObject);
                }
                else
                {
                    if (child.TryGetComponent(out FixedJoint2D fixedJoint))
                    {
                        fixedJoint.enabled = false;
                    }
                }
            }
        }
    }

    private void AddHingeJoint(GameObject previousLink, GameObject currentLink)
    {
        if (currentLink.TryGetComponent(out HingeJoint2D hingeJoint))
        {
            if (previousLink == null)
            {
                hingeJoint.enabled = false;
                return;
            }
            else
            {
                //hingeJoint.enabled = true;
            }
        }
        else
        {
            if (previousLink == null) return;

            hingeJoint = currentLink.AddComponent<HingeJoint2D>();
        }

        hingeJoint.connectedBody = previousLink.GetComponent<Rigidbody2D>();
        hingeJoint.useLimits = true;
        hingeJoint.limits = new JointAngleLimits2D { min = -angle, max = angle };
        hingeJoint.anchor = new Vector2(-linkLength / 2, 0);

        hingeJoint.enabled = true;
    }

    private void AddFixedJoint(GameObject targetObject, GameObject currentLink)
    {       
        if (currentLink.TryGetComponent(out FixedJoint2D fixedJoint))
        {
            //fixedJoint.enabled = true;
        }
        else
        {
            fixedJoint = currentLink.AddComponent<FixedJoint2D>();
        }

        fixedJoint.connectedBody = targetObject.GetComponent<Rigidbody2D>();

        fixedJoint.enabled = true;
    }

    private void RemoveFixedJoints(Chain rope)
    {
        foreach (Transform link in rope.transform)
        {
            FixedJoint2D fixedJoint = link.GetComponent<FixedJoint2D>();
            if (fixedJoint != null)
            {
                DestroyImmediate(fixedJoint);
            }
        }
    }

    public void ClearData()
    {
        //foreach (Chain chain in chains)
        //{
        //    chain.Clear();
        //}

        for (int i = 0; i < chains.Count; i++)
        {
            chains[i].Clear();
            i--;
        }

        chains.Clear();
    }

    public void RemoveChain(Screw linkedScrew)
    {
        Chain chain = chains.Find(c => c.linkedScrew == linkedScrew);

        if (chain != null)
        {
            chain.Cut();
        }
    }
}
